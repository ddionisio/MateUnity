using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Resource Manager")]
    public class ResourceManager : SingletonBehaviour<ResourceManager> {
        public enum LoaderType {
            Internal, //load from Resources
            LocalBundle, //load from asset bundle locally
            OnlineBundle //load from asset bundle url
        }

        [System.Serializable]
        public struct RootPackage {
            public LoaderType type;
            public string path;
        }

        public delegate void RequestCallback(ResourceLoader.Request request, object param);

        //Used for loading up root packages at start
        public RootPackage[] rootPackages;
        public bool loadAtStart = true;

        private class Package {
            public ResourceLoader loader { get; private set; }
            public int counter; //inc by groups using this package, once 0, it will be deleted
            public bool loadQueue; //set to true when in queue
            public Dictionary<string, ResourceLoader.Request> processedRequests { get; private set; } //files requested and being processed or done

            public void Unload() {
                //unload requested resources
                foreach(KeyValuePair<string, ResourceLoader.Request> pair in processedRequests) {
                    ResourceLoader.Request req = pair.Value;
                    if(req.isDone && req.data != null)
                        loader.UnloadResource(req.data);
                }

                processedRequests.Clear();
            }

            public Package(string path, LoaderType type) {
                switch(type) {
                    case LoaderType.Internal:
                        loader = new ResourceLoaderInternal(path);
                        break;
                    case LoaderType.LocalBundle:
                        loader = new ResourceLoaderLocalBundle(path);
                        break;
                    case LoaderType.OnlineBundle:
                        loader = new ResourceLoaderOnlineBundle(path);
                        break;
                }

                counter = 1;
                loadQueue = false;
                processedRequests = new Dictionary<string, ResourceLoader.Request>();
            }
        }

        private struct RequestProcess {
            public string group; //if empty, find appropriate group
            public ResourceLoader.Request request;
        }

        private List<Package> mPackages = new List<Package>(); //all unique packages loaded

        private Dictionary<string, List<Package>> mGroups = new Dictionary<string,List<Package>>(); //list of reference to packages

        private List<Package> mRoot = new List<Package>(); //list of reference to packages

        private Queue<Package> mLoadPackageQueue = new Queue<Package>();
        private Queue<RequestProcess> mRequestQueue = new Queue<RequestProcess>();

        private IEnumerator mLoadAct;

        /// <summary>
        /// Add a package/path for group, group is created if it doesn't exist.  For Internal or LocalStream, this is a directory.
        /// </summary>
        public void AddToGroup(string group, string path, LoaderType type) {
            List<Package> packageRefs;
            if(!mGroups.TryGetValue(group, out packageRefs)) {
                packageRefs = new List<Package>();
                mGroups.Add(group, packageRefs);
            }

            //ensure path hasn't been added for this group
            for(int i = 0; i < packageRefs.Count; i++) {
                if(packageRefs[i].loader.rootPath == path)
                    return;
            }

            //get package from global
            for(int i = 0; i < mPackages.Count; i++) {
                if(mPackages[i].loader.rootPath == path) {
                    mPackages[i].counter++;
                    packageRefs.Add(mPackages[i]);
                    return;
                }
            }

            //add new package
            Package newPack = new Package(path, type);
            
            //add to global packages
            mPackages.Add(newPack);

            //add to package reference
            packageRefs.Add(newPack);

        }

        /// <summary>
        /// Start loading this group
        /// </summary>
        public void LoadGroup(string group) {
            List<Package> packageRefs;
            if(!mGroups.TryGetValue(group, out packageRefs)) {
                Debug.LogError("Group not found: "+group);
                return;
            }

            for(int i = 0; i < packageRefs.Count; i++) {
                Package pkg = packageRefs[i];

                //already loading or loaded?
                if(pkg.loadQueue || pkg.loader.status == ResourceLoader.Status.Loaded || pkg.loader.status == ResourceLoader.Status.Loading)
                    continue;

                pkg.loadQueue = true;
                mLoadPackageQueue.Enqueue(pkg);
            }

            if(mLoadAct == null)
                StartCoroutine(mLoadAct = DoLoading());
        }

        /// <summary>
        /// Load all packages that are not loaded, including root
        /// </summary>
        public void LoadAll() {
            for(int i = 0; i < mPackages.Count; i++) {
                Package pkg = mPackages[i];

                //already loading or loaded?
                if(pkg.loadQueue || pkg.loader.status == ResourceLoader.Status.Loaded || pkg.loader.status == ResourceLoader.Status.Loading)
                    continue;

                pkg.loadQueue = true;
                mLoadPackageQueue.Enqueue(pkg);
            }

            if(mLoadAct == null)
                StartCoroutine(mLoadAct = DoLoading());
        }

        /// <summary>
        /// Check to see if group is completely loaded
        /// </summary>
        public bool IsGroupLoaded(string group) {
            List<Package> packageRefs;
            if(!mGroups.TryGetValue(group, out packageRefs)) {
                Debug.LogError("Group not found: "+group);
                return false;
            }

            for(int i = 0; i < packageRefs.Count; i++) {
                Package pkg = packageRefs[i];
                if(pkg.loadQueue || pkg.loader.status == ResourceLoader.Status.Loading)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Unloads and remove group.
        /// </summary>
        public void RemoveGroup(string group) {
            List<Package> packageRefs;
            if(!mGroups.TryGetValue(group, out packageRefs)) {
                Debug.LogError("Group not found: "+group);
                return;
            }

            for(int i = 0; i < packageRefs.Count; i++) {
                Package pkg = packageRefs[i];

                pkg.loadQueue = false;
                
                //unload and remove from global if counter is 0
                pkg.counter--;
                if(pkg.counter <= 0) {
                    //if it's currently loading, then unload it later
                    if(pkg.loader.status != ResourceLoader.Status.Loading) {
                        pkg.Unload();
                        mPackages.Remove(pkg);
                    }
                }
            }

            mGroups.Remove(group);
        }

        /// <summary>
        /// Unload and remove all groups, except for root
        /// </summary>
        public void RemoveAllGroups() {
            string[] groups = new string[mGroups.Count];
            mGroups.Keys.CopyTo(groups, 0);

            for(int i = 0; i < groups.Length; i++) {
                RemoveGroup(groups[i]);
            }
        }

        public ResourceLoader.Request RequestResourceFrom(string group, string path, System.Type type) {
            List<Package> packageRefs;
            if(mGroups.TryGetValue(group, out packageRefs)) {
                //check each package to see if this path is already processed
                for(int i = packageRefs.Count-1; i >= 0; i--) {
                    Package pkg = packageRefs[i];

                    if(pkg.processedRequests.ContainsKey(path))
                        return pkg.processedRequests[path];
                }

                //check if already queued
                foreach(RequestProcess rp in mRequestQueue) {
                    if(rp.request.path == path && rp.group == group)
                        return rp.request;
                }

                //add to process queue
                ResourceLoader.Request request = ResourceLoader.CreateRequest(path, type);
                mRequestQueue.Enqueue(new RequestProcess() { group=group, request=request });

                if(mLoadAct == null)
                    StartCoroutine(mLoadAct = DoLoading());

                return request;
            }
            else
                Debug.LogError("Group not found: "+group);
            
            return null;
        }

        public ResourceLoader.Request RequestResource(string path, System.Type type) {
            //check each package to see if this path is already processed
            for(int i = mPackages.Count-1; i >= 0; i--) {
                Package pkg = mPackages[i];

                if(pkg.processedRequests.ContainsKey(path))
                    return pkg.processedRequests[path];
            }

            //check if already queued
            foreach(RequestProcess rp in mRequestQueue) {
                if(rp.request.path == path)
                    return rp.request;
            }

            //add to process queue
            ResourceLoader.Request request = ResourceLoader.CreateRequest(path, type);
            mRequestQueue.Enqueue(new RequestProcess() { group="", request=request });

            if(mLoadAct == null)
                StartCoroutine(mLoadAct = DoLoading());

            return request;
        }

        protected override void OnDestroy() {
            RemoveAllGroups();
            base.OnDestroy();
        }

        void OnEnable() {
            if(mLoadPackageQueue.Count > 0)
                StartCoroutine(mLoadAct = DoLoading());
        }

        void OnDisable() {
            mLoadAct = null;
        }

        protected override void Awake() {
            base.Awake();

            //add roots
            if(rootPackages != null) {
                for(int i = 0; i < rootPackages.Length; i++) {
                    //add new package
                    Package newPack = new Package(rootPackages[i].path, rootPackages[i].type);

                    //add to global packages
                    mPackages.Add(newPack);

                    //add to root
                    mRoot.Add(newPack);
                }
            }

            //other groups
        }

        void Start() {
            if(loadAtStart)
                LoadAll();
        }

        IEnumerator DoLoading() {
            while(mLoadPackageQueue.Count > 0 || mRequestQueue.Count > 0) {
                //load all packages
                if(mLoadPackageQueue.Count > 0) {
                    Package pkg = mLoadPackageQueue.Dequeue();

                    if(pkg.loadQueue) {
                        pkg.loadQueue = false;

                        //wait loading
                        yield return StartCoroutine(mLoadAct = pkg.loader.Load());

                        //done loading
                        if(pkg.loader.status == ResourceLoader.Status.Error) {
                            Debug.LogError("Error loading: "+pkg.loader.rootPath+" msg: "+pkg.loader.error);
                            mPackages.Remove(pkg);
                        }
                        else if(pkg.counter <= 0) {
                            //must have been requested to be removed
                            pkg.Unload();
                            mPackages.Remove(pkg);
                        }
                    }
                }
                //load requests
                else if(mRequestQueue.Count > 0) { 
                    RequestProcess requestProc = mRequestQueue.Dequeue();
                    ResourceLoader.Request request = requestProc.request;
                    bool processed = false;

                    //grab appropriate package
                    if(string.IsNullOrEmpty(requestProc.group)) {
                        int errorCount = 0;
                        for(int i = mPackages.Count-1; i >= 0; i--) {
                            Package pkg = mPackages[i];

                            //make sure package is loaded
                            if(pkg.loader.status == ResourceLoader.Status.Loaded) {
                                if(pkg.loader.ProcessRequest(request)) {
                                    pkg.processedRequests.Add(request.path, request); //done
                                    processed = true;
                                    break;
                                }
                                else
                                    errorCount++;
                            }
                            else if(pkg.loader.status != ResourceLoader.Status.Loading)
                                errorCount++; //package is unloaded or is somewhat invalid
                        }

                        //not yet processed, try again later once everything is loaded
                        if(errorCount < mPackages.Count) {
                            if(!processed)
                                mRequestQueue.Enqueue(requestProc);
                        }
                        else
                            Debug.LogError(request.error);
                    }
                    else {
                        List<Package> packageRefs;
                        if(mGroups.TryGetValue(requestProc.group, out packageRefs)) {
                            int errorCount = 0;
                            for(int i = packageRefs.Count-1; i >= 0; i--) {
                                Package pkg = packageRefs[i];

                                //make sure package is loaded
                                if(pkg.loader.status == ResourceLoader.Status.Loaded) {
                                    if(pkg.loader.ProcessRequest(request)) {
                                        pkg.processedRequests.Add(request.path, request); //done
                                        processed = true;
                                        break;
                                    }
                                    else
                                        errorCount++;
                                }
                                else if(pkg.loader.status != ResourceLoader.Status.Loading)
                                    errorCount++; //package is unloaded or is somewhat invalid
                            }

                            //not yet processed, try again later once everything is loaded
                            if(errorCount < packageRefs.Count) {
                                if(!processed)
                                    mRequestQueue.Enqueue(requestProc);
                            }
                            else
                                Debug.LogError(request.error);
                        }
                        else { //no longer exists??
                            Debug.LogError("Group not found: "+requestProc.group);
                            ResourceLoader.RequestError(request, ResourceLoader.ErrorCode.InvalidPackage);
                            Debug.LogError(request.error);
                        }
                    }
                }

                yield return null;
            }
			
			mLoadAct = null;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class ResourceManager : MonoBehaviour {
        public enum LoaderType {
            Internal, //load from Resources
            LocalStream, //load from file
            LocalBundle, //load from asset bundle locally
            OnlineBundle //load from asset bundle url
        }

        [System.Serializable]
        public struct RootPackage {
            public LoaderType type;
            public string path;
        }

        //Used for loading up root packages at start
        public RootPackage[] rootPackages;
        public bool loadAtStart = true;

        private class Package {
            public ResourceLoader loader { get; private set; }
            public int counter; //inc by groups using this package, once 0, it will be deleted
            public bool loadQueue; //set to true when in queue
            public Dictionary<string, ResourceLoader.Request> requests { get; private set; } //files requested

            public void Unload() {
                //unload requested resources
                foreach(KeyValuePair<string, ResourceLoader.Request> pair in requests) {
                    ResourceLoader.Request req = pair.Value;
                    if(req.isDone && req.data != null)
                        loader.UnloadResource(req.data);
                }

                requests.Clear();
            }

            public Package(string path, LoaderType type) {
                switch(type) {
                    case LoaderType.Internal:
                        loader = new ResourceLoaderInternal(path);
                        break;
                    case LoaderType.LocalStream:
                        loader = new ResourceLoaderLocalStream(path);
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
                requests = new Dictionary<string, ResourceLoader.Request>();
            }
        }
                
        private static ResourceManager mInstance;

        private List<Package> mPackages = new List<Package>(); //all unique packages loaded

        private Dictionary<string, List<Package>> mGroups = new Dictionary<string,List<Package>>(); //list of reference to packages

        private List<Package> mRoot = new List<Package>(); //list of reference to packages

        private Queue<Package> mLoadPackageQueue = new Queue<Package>();

        private IEnumerator mPackageLoadAct;

        public static ResourceManager instance { get { return mInstance; } }

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

            if(mPackageLoadAct == null)
                StartCoroutine(mPackageLoadAct = DoPackageLoading());
        }

        public void LoadAllGroups() {
            string[] groups = new string[mGroups.Count];
            mGroups.Keys.CopyTo(groups, 0);

            for(int i = 0; i < groups.Length; i++)
                LoadGroup(groups[i]);
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
                //check each package to see if this path exists
                for(int i = packageRefs.Count-1; i >= 0; i--) {
                    Package pkg = packageRefs[i];

                    //check if file is already requested
                    if(pkg.requests.ContainsKey(path))
                        return pkg.requests[path];

                    //make sure it actually exists for this package
                    if(pkg.loader.ResourceExists(path)) {
                        ResourceLoader.Request request = pkg.loader.RequestResource(path, type);
                        if(request != null) {
                            pkg.requests.Add(path, request);
                            return request;
                        }
                    }
                }

                Debug.LogError("Path not found: "+path+" in group: "+group);
            }
            else
                Debug.LogError("Group not found: "+group);
            
            return null;
        }

        public ResourceLoader.Request RequestResource(string path, System.Type type) {
            //check each package to see if this path exists
            for(int i = mPackages.Count-1; i >= 0; i--) {
                Package pkg = mPackages[i];

                //check if file is already requested
                if(pkg.requests.ContainsKey(path))
                    return pkg.requests[path];

                //make sure it actually exists for this package
                if(pkg.loader.ResourceExists(path)) {
                    ResourceLoader.Request request = pkg.loader.RequestResource(path, type);
                    if(request != null) {
                        pkg.requests.Add(path, request);
                        return request;
                    }
                }
            }

            Debug.LogError("Path not found: "+path);
            return null;
        }

        void OnDestroy() {
            RemoveAllGroups();

            if(mInstance == this)
                mInstance = null;
        }

        void OnEnable() {
            if(mLoadPackageQueue.Count > 0)
                StartCoroutine(mPackageLoadAct = DoPackageLoading());
        }

        void OnDisable() {
            mPackageLoadAct = null;
        }

        void Awake() {
            if(mInstance == null) {
                mInstance = this;

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
            else
                DestroyImmediate(gameObject);
        }

        void Start() {
            if(loadAtStart) {
                //load root
                for(int i = 0; i < mRoot.Count; i++) {
                    Package pkg = mRoot[i];

                    //already loading or loaded?
                    if(pkg.loadQueue || pkg.loader.status == ResourceLoader.Status.Loaded || pkg.loader.status == ResourceLoader.Status.Loading)
                        continue;

                    pkg.loadQueue = true;
                    mLoadPackageQueue.Enqueue(pkg);
                }

                if(mPackageLoadAct == null)
                    StartCoroutine(mPackageLoadAct = DoPackageLoading());

                //load groups
                LoadAllGroups();
            }
        }

        IEnumerator DoPackageLoading() {
            while(mLoadPackageQueue.Count > 0) {
                Package pkg = mLoadPackageQueue.Peek();

                if(pkg.loadQueue) {
                    pkg.loadQueue = false;

                    yield return StartCoroutine(pkg.loader.Load());

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

                mLoadPackageQueue.Dequeue();

                yield return null;
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    public class ResourceLoaderLocalBundle : ResourceLoader {
        private class RequestInternal : Request {
            private AssetBundleRequest mReq;

            public RequestInternal(AssetBundle bundle, string path, System.Type type)
                : base(path) {
                mReq = bundle.LoadAsync(path, type);
            }
            public override bool isDone { get { return mReq.isDone; } }
            public override object data { get { return mReq.asset; } }
            public override string error { get { return ""; } }
        }

        private AssetBundle mAssetBundle;

        public ResourceLoaderLocalBundle(string aRootPath) : base(aRootPath) {
        }

        public override bool ResourceExists(string path) {
            if(mAssetBundle == null) {
                Debug.LogError("Asset Bundle is not loaded, error loading: "+path);
                return false;
            }

            return mAssetBundle.Contains(GetUnityPath(path, false));
        }

        public override IEnumerator Load() {
            mAssetBundle = AssetBundle.CreateFromFile(rootPath); //why is there no async for this, and yet there is one for memory load?
            status = Status.Loaded;
            yield return null;
        }

        public override void Unload() {
            mAssetBundle.Unload(true);
            mAssetBundle = null;
            status = Status.Unloaded;
        }

        public override Request RequestResource(string path, System.Type type) {
            if(mAssetBundle == null) {
                Debug.LogError("Asset Bundle is not loaded, error loading: "+path);
                return null;
            }

            return new RequestInternal(mAssetBundle, GetUnityPath(path, false), type);
        }

        public override void UnloadResource(object obj) {
        }
    }
}
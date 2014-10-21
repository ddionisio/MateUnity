using UnityEngine;
using System.Collections;

namespace M8 {
    public class ResourceLoaderOnlineBundle : ResourceLoader {
        private AssetBundle mAssetBundle;

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

        public ResourceLoaderOnlineBundle(string aRootPath) : base(aRootPath) {
        }

        public override bool ResourceExists(string path) {
            if(mAssetBundle == null) {
                Debug.LogError("Asset Bundle is not loaded, error checking: "+path);
                return false;
            }

            return mAssetBundle.Contains(GetUnityPath(path, false));
        }

        public override IEnumerator Load() {
            status = Status.Loading;

            //TODO: add version and crc
            using(WWW web = WWW.LoadFromCacheOrDownload(rootPath, 1)) {
                yield return web;

                error = web.error;
                if(!string.IsNullOrEmpty(error))
                    status = Status.Error;
                else {
                    mAssetBundle = web.assetBundle;
                    status = Status.Loaded;
                }
            }
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
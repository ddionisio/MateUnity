using UnityEngine;
using System.Collections;

namespace M8 {
    public class ResourceLoaderOnlineBundle : ResourceLoader {
        private AssetBundle mAssetBundle;

        public ResourceLoaderOnlineBundle(string aRootPath) : base(aRootPath) {
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

        public override void UnloadResource(object obj) {
        }

        public override bool ProcessRequest(Request req) {
            RequestInternal ireq = (RequestInternal)req;

            string path = GetUnityPath(req.path, false);

            if(!mAssetBundle.Contains(path)) {
                ireq.Error(ErrorCode.FileNotExist);
                return false;
            }

            AssetBundleRequest resReq = mAssetBundle.LoadAsync(path, req.type);
            ireq.processor = new RequestProcessBundle(resReq);
            return true;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    public class ResourceLoaderLocalBundle : ResourceLoader {
        private AssetBundle mAssetBundle;

        public ResourceLoaderLocalBundle(string aRootPath) : base(aRootPath) {
        }

        public override IEnumerator Load() {
            var request = AssetBundle.LoadFromFileAsync(rootPath);
            yield return request;

            mAssetBundle = request.assetBundle;
            status = Status.Loaded;
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

            AssetBundleRequest resReq = mAssetBundle.LoadAssetAsync(path, req.type);
            ireq.processor = new RequestProcessBundle(resReq);
            return true;
        }
    }
}
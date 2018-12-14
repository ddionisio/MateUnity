using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace M8 {
    public class ResourceLoaderOnlineBundle : ResourceLoader {
        private AssetBundle mAssetBundle;

        public ResourceLoaderOnlineBundle(string aRootPath) : base(aRootPath) {
        }

        public override IEnumerator Load() {
            status = Status.Loading;

            //TODO: add version and crc
            var request = UnityWebRequestAssetBundle.GetAssetBundle(rootPath);

            yield return request.SendWebRequest();

            if(request.isNetworkError) {
                error = request.error;
                status = Status.Error;
            }
            else {
                mAssetBundle = DownloadHandlerAssetBundle.GetContent(request);
                status = Status.Loaded;
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

            AssetBundleRequest resReq = mAssetBundle.LoadAssetAsync(path, req.type);
            ireq.processor = new RequestProcessBundle(resReq);
            return true;
        }
    }
}
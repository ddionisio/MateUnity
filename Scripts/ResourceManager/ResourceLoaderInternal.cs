using UnityEngine;
using System.Text;
using System.Collections;

namespace M8 {
    public class ResourceLoaderInternal : ResourceLoader {
        public ResourceLoaderInternal(string aRootPath) : base(aRootPath) {
        }

        public override IEnumerator Load() {
            status = Status.Loaded;
            yield return null;
        }

        public override void Unload() {
            status = Status.Unloaded;
        }

        public override void UnloadResource(object obj) {
            if(obj is GameObject || obj is Component || obj is ScriptableObject || obj is AssetBundle)
                return;
            else
                Resources.UnloadAsset(obj as Object);
        }

        public override bool ProcessRequest(Request req) {
            RequestInternal ireq = (RequestInternal)req;

            string path = GetUnityPath(req.path, true);

            ResourceRequest resReq = Resources.LoadAsync(path, req.type);
            if(resReq.isDone && resReq.asset == null) {
                ireq.Error(ErrorCode.FileNotExist);
                return false;
            }

            ireq.processor = new RequestProcessResource(resReq);
            return true;
        }
    }
}
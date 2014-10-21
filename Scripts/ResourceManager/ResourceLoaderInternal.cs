using UnityEngine;
using System.Text;
using System.Collections;

namespace M8 {
    public class ResourceLoaderInternal : ResourceLoader {
        private class RequestInternal : Request {
            private ResourceRequest mReq;

            public RequestInternal(string path, System.Type type) : base(path) {
                mReq = Resources.LoadAsync(path, type);
            }
            public override bool isDone { get { return mReq.isDone; } }
            public override object data { get { return mReq.asset; } }
            public override string error { get { return ""; } }
        }

        public ResourceLoaderInternal(string aRootPath) : base(aRootPath) {
        }

        public override bool ResourceExists(string path) {
            return true; //why the fuck is there no Resources.Exists???
        }

        public override IEnumerator Load() {
            status = Status.Loaded;
            yield return null;
        }

        public override void Unload() {
            status = Status.Unloaded;
        }

        public override Request RequestResource(string path, System.Type type) {
            if(status == Status.Unloaded) return null;

            return new RequestInternal(GetUnityPath(path, true), type);
        }

        public override void UnloadResource(object obj) {
            if(status == Status.Loaded)
                Resources.UnloadAsset(obj as Object);
        }
    }
}
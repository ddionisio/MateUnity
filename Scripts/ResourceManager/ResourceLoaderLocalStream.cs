using UnityEngine;
using System.Collections;

namespace M8 {
    public class ResourceLoaderLocalStream : ResourceLoader {
        public ResourceLoaderLocalStream(string aRootPath) : base(aRootPath) {
        }

        public override IEnumerator Load() {
            throw new System.NotImplementedException("Coming Soon!");
        }

        public override void Unload() {
            throw new System.NotImplementedException("Coming Soon!");
        }

        public override bool ResourceExists(string path) {
            throw new System.NotImplementedException("Coming Soon!");
        }

        public override Request RequestResource(string path, System.Type type) {
            throw new System.NotImplementedException("Coming Soon!");
        }

        public override void UnloadResource(object obj) {
            throw new System.NotImplementedException("Coming Soon!");
        }
    }
}
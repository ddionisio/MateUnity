using UnityEngine;

namespace M8.Auxiliary {
    [AddComponentMenu("M8/Auxiliary/Update")]
    public class AuxUpdate : MonoBehaviour {
        public delegate void Callback();

        public event Callback callback;

        void OnDestroy() {
            callback = null;
        }

        void Update() {
            if(callback != null)
                callback();
        }
    }
}
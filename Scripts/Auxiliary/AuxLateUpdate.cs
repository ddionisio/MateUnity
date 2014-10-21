using UnityEngine;

namespace M8.Auxiliary {
    [AddComponentMenu("M8/Auxiliary/LateUpdate")]
    public class AuxLateUpdate : MonoBehaviour {
        public delegate void Callback();

        public event Callback callback;

        void OnDestroy() {
            callback = null;
        }

        void LateUpdate() {
            if(callback != null)
                callback();
        }
    }
}
using UnityEngine;

namespace M8.Auxiliary {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Auxiliary/Trigger2D")]
    public class AuxTrigger2D : MonoBehaviour {
        public delegate void Callback(Collider2D other);

        public event Callback enterCallback;
        public event Callback stayCallback;
        public event Callback exitCallback;
        
        void OnTriggerEnter2D(Collider2D other) {
            if(enterCallback != null)
                enterCallback(other);
        }

        void OnTriggerStay2D(Collider2D other) {
            if(stayCallback != null)
                stayCallback(other);
        }

        void OnTriggerExit2D(Collider2D other) {
            if(exitCallback != null)
                exitCallback(other);
        }
    }
#endif
}
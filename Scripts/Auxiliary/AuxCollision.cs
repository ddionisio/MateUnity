using UnityEngine;

namespace M8.Auxiliary {
    [AddComponentMenu("M8/Auxiliary/Collision")]
    public class AuxCollision : MonoBehaviour {
#if !M8_PHYSICS_DISABLED
        public delegate void Callback(Collision coll);

        public event Callback enterCallback;
        public event Callback stayCallback;
        public event Callback exitCallback;
        
        void OnCollisionEnter(Collision coll) {
            if(enterCallback != null)
                enterCallback(coll);
        }

        void OnCollisionStay(Collision coll) {
            if(stayCallback != null)
                stayCallback(coll);
        }

        void OnCollisionExit(Collision coll) {
            if(exitCallback != null)
                exitCallback(coll);
        }
#endif
    }
}
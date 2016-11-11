using UnityEngine;

namespace M8.Auxiliary {
    [AddComponentMenu("M8/Auxiliary/Collision2D")]
    public class AuxCollision2D : MonoBehaviour {
        public delegate void Callback(Collision2D coll);

        public event Callback enterCallback;
        public event Callback stayCallback;
        public event Callback exitCallback;
        
        void OnCollisionEnter2D(Collision2D coll) {
            if(enterCallback != null)
                enterCallback(coll);
        }

        void OnCollisionStay2D(Collision2D coll) {
            if(stayCallback != null)
                stayCallback(coll);
        }

        void OnCollisionExit2D(Collision2D coll) {
            if(exitCallback != null)
                exitCallback(coll);
        }
    }
}
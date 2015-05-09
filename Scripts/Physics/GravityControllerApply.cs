using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Physics/GravityControllerApply")]
    public class GravityControllerApply : MonoBehaviour {
        public GravityController target;

        public bool overrideGravity = true;
        public float gravity = -9.8f;

        private Rigidbody mBody;

        void Awake() {
            mBody = GetComponent<Rigidbody>();
        }

        void FixedUpdate() {
            if(target && !mBody.isKinematic) {
                mBody.AddForce(target.up * (overrideGravity ? gravity : target.gravity) * mBody.mass, ForceMode.Force);
            }
        }
    }
}
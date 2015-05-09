using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Physics/ColliderForce")]
    public class ColliderForce : MonoBehaviour {
        public float force = 30.0f;
        public ForceMode mode = ForceMode.Impulse;
        public bool atPoint = false;

        void OnCollisionEnter(Collision col) {
            foreach(ContactPoint contact in col.contacts) {
                Rigidbody body = contact.otherCollider.GetComponent<Rigidbody>();

                if(body != null && !body.isKinematic) {
                    if(atPoint)
                        body.AddForceAtPosition(-contact.normal * force, contact.point, mode);
                    else
                        body.AddForce(-contact.normal * force, mode);
                }
            }
        }
    }
}
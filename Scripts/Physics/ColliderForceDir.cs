using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Physics/ColliderForceDir")]
    public class ColliderForceDir : MonoBehaviour {
        public enum Axis {
            Up,
            Forward,
            Right
        }

        [SerializeField]
        Axis _dir;

        public bool inverse;

        public float force = 30.0f;
        public ForceMode mode = ForceMode.Impulse;
        public bool atPoint = false;

        void OnCollisionEnter(Collision col) {
            Vector3 dir;
            switch(_dir) {
                case Axis.Right:
                    dir = Vector3.right;
                    break;
                case Axis.Forward:
                    dir = Vector3.forward;
                    break;
                default:
                    dir = Vector3.up;
                    break;
            }

            dir = transform.rotation * (inverse ? -dir : dir);

            foreach(ContactPoint contact in col.contacts) {
                Rigidbody body = contact.otherCollider.GetComponent<Rigidbody>();

                if(body != null && !body.isKinematic) {
                    if(atPoint)
                        body.AddForceAtPosition(dir * force, contact.point, mode);
                    else
                        body.AddForce(dir * force, mode);
                }
            }
        }
    }
}
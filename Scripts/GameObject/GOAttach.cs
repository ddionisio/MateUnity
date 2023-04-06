using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Make sure this is on an object with a rigidbody!
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Game Object/Attach")]
    public class GOAttach : MonoBehaviour {
        public Transform target;
        public Vector3 offset;

#if !M8_PHYSICS_DISABLED
        private Collider mCollider;

        void Awake() {
            mCollider = GetComponent<Collider>();
        }
#endif

        void Update() {
            if(target != null) {
#if !M8_PHYSICS_DISABLED
                if(mCollider != null) {
                    Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(mCollider.bounds.center);

                    transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
                }
                else 
#endif
                {
                    transform.position = target.position + target.rotation * offset;
                }

                transform.rotation = target.rotation;
            }
        }
    }
}
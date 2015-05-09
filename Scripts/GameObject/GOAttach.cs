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

        private Collider mCollider;

        void Awake() {
            mCollider = GetComponent<Collider>();
        }

        void Update() {
            if(target != null) {
                if(mCollider != null) {
                    Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(mCollider.bounds.center);

                    transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
                }
                else {
                    transform.position = target.position + target.rotation * offset;
                }

                transform.rotation = target.rotation;
            }
        }
    }
}
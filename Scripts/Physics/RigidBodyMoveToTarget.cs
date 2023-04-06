using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Make sure this is on an object with a rigidbody!
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Physics/RigidBodyMoveToTarget")]
    public class RigidBodyMoveToTarget : MonoBehaviour {
        public Transform target;
#if !M8_PHYSICS_DISABLED
        public Collider thisCollider;
#endif
        public Vector3 offset;

        public bool ignoreRotation = false;
        public Vector3 rotOfs;

#if !M8_PHYSICS_DISABLED
        private Rigidbody mBody;
#endif

        private Quaternion mRotQ;

#if UNITY_EDITOR
        // Update is called once per frame
        void Update() {
            if(!Application.isPlaying && target != null) {
#if !M8_PHYSICS_DISABLED
                Collider col = thisCollider != null ? thisCollider : GetComponent<Collider>();
                if(col != null) {
                    Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(col.bounds.center);

                    transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
                }
                else 
#endif
                {
                    transform.position = target.position + target.rotation*offset;
                }

                if(!ignoreRotation)
                    transform.rotation = Quaternion.Euler(rotOfs) * target.rotation;
            }
        }
#endif

                void FixedUpdate() {
            if(target) {
                Vector3 newPos;

#if !M8_PHYSICS_DISABLED
                if(thisCollider != null) {
                    Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(thisCollider.bounds.center);
                    newPos = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
                }
                else
#endif
                    newPos = target.position + target.rotation * offset;

#if !M8_PHYSICS_DISABLED
                if(transform.position != newPos)
                    mBody.MovePosition(newPos);

                if(!ignoreRotation)
                    mBody.MoveRotation(mRotQ * target.rotation);
#endif
            }
        }

        void Awake() {
#if !M8_PHYSICS_DISABLED
            if(thisCollider == null)
                thisCollider = GetComponent<Collider>();

            mBody = GetComponent<Rigidbody>();
#endif

            mRotQ = Quaternion.Euler(rotOfs);
        }
    }
}
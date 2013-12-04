using UnityEngine;
using System.Collections;

/// <summary>
/// Make sure this is on an object with a rigidbody!
/// </summary>
[ExecuteInEditMode]
[AddComponentMenu("M8/Physics/RigidBodyMoveToTarget")]
public class RigidBodyMoveToTarget : MonoBehaviour {
    public Transform target;
    public Vector3 offset;

    public Vector3 rotOfs;

    private Quaternion mRotQ;

#if UNITY_EDITOR
    // Update is called once per frame
    void Update() {
        if(!Application.isPlaying && target != null) {
            if(collider != null) {
                Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(collider.bounds.center);

                transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
            }
            else {
                transform.position = target.position + target.rotation*offset;
            }

            transform.rotation = Quaternion.Euler(rotOfs) * target.rotation;
        }
    }
#endif

    void FixedUpdate() {
        if(target) {
            Vector3 newPos;

            if(collider != null) {
                Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(collider.bounds.center);
                newPos = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
            }
            else
                newPos = target.position + target.rotation * offset;

            if(transform.position != newPos)
                rigidbody.MovePosition(newPos);

            rigidbody.MoveRotation(mRotQ * target.rotation);
        }
    }

    void Awake() {
        mRotQ = Quaternion.Euler(rotOfs);
    }
}

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

#if UNITY_EDITOR
    // Update is called once per frame
    void Update() {
        if(!Application.isPlaying && target != null) {
            if(collider != null) {
                Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(collider.bounds.center);

                transform.position = target.localToWorldMatrix.MultiplyPoint(offset - ofs);
            }
            else {
                transform.position = target.position;
            }

            transform.rotation = target.rotation;
        }
    }
#endif

    void FixedUpdate() {
        if(collider != null) {
            Vector3 ofs = transform.worldToLocalMatrix.MultiplyPoint(collider.bounds.center);
            rigidbody.MovePosition(target.localToWorldMatrix.MultiplyPoint(offset - ofs));
        }
        else
            rigidbody.MovePosition(target.position);

        rigidbody.MoveRotation(target.rotation);
    }
}

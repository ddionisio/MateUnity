using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityControllerApply")]
public class GravityControllerApply : MonoBehaviour {
    public GravityController target;

    public bool overrideGravity = true;
    public float gravity = -9.8f;

    private Rigidbody mBody;

    void Awake() {
        mBody = rigidbody;
    }

    void FixedUpdate() {
        if(target && !mBody.isKinematic) {
            mBody.AddForce(target.up * (overrideGravity ? gravity : target.gravity) * mBody.mass, ForceMode.Force);
        }
    }
}

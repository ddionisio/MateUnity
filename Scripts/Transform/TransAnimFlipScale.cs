using UnityEngine;
using System.Collections;

/// <summary>
/// Toggle between original scale and (original scale)*mod
/// </summary>
[AddComponentMenu("M8/Transform/AnimFlipScale")]
public class TransAnimFlipScale : MonoBehaviour {
    public Transform target;

    public Vector3 mod = Vector3.one;

    public float delay;

    private Vector3 mPrevScale;
    private bool mIsPrev;

    void OnEnable() {
        mPrevScale = target.localScale;
        mIsPrev = true;

        InvokeRepeating("DoFlip", delay, delay);
    }

    void OnDisable() {
        if(!mIsPrev) {
            target.localScale = mPrevScale;
            mIsPrev = true;
        }
    }

    void Awake() {
        if(target == null)
            target = transform;
    }

    void DoFlip() {
        if(mIsPrev) {
            target.localScale = new Vector3(mPrevScale.x * mod.x, mPrevScale.y * mod.y, mPrevScale.z * mod.z);
            mIsPrev = false;
        }
        else {
            target.localScale = mPrevScale;
            mIsPrev = true;
        }
    }
}

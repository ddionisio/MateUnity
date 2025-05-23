using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformWanderRadius : MonoBehaviour {
    public Vector2 offset;
    public float radius;
    public Transform target;
    public float moveDelay;
    public M8.RangeFloat waitDelayRange;

    private float mLastWaitTime;
    private float mWaitDelay;

    private Vector2 mTargetLocalPos;
    private Vector2 mTargetVel;

	void OnEnable() {
		if(target) {
			mTargetLocalPos = transform.worldToLocalMatrix.MultiplyPoint3x4(target.position);

            mLastWaitTime = Time.time;
            mWaitDelay = waitDelayRange.random;
        }
	}

    const float approxDiff = 0.001f;

	void Update() {
        if(target) {
            Vector2 targetCurPos = target.position;
            Vector2 targetPos = transform.TransformPoint(mTargetLocalPos);

            if(M8.MathUtil.Approx(targetCurPos, targetPos, approxDiff)) {
                var t = Time.time;
                if(t - mLastWaitTime >= mWaitDelay) {
                    mTargetLocalPos = offset + Random.insideUnitCircle * radius;
                    mTargetVel = Vector2.zero;
                }
            }
            else {
                targetCurPos = Vector2.SmoothDamp(targetCurPos, targetPos, ref mTargetVel, moveDelay);

				if(M8.MathUtil.Approx(targetCurPos, targetPos, approxDiff)) {
					mLastWaitTime = Time.time;
					mWaitDelay = waitDelayRange.random;
				}

				target.position = targetCurPos;
            }
        }
    }

	void OnDrawGizmos() {
        Gizmos.color = Color.white;

        var pos = transform.TransformPoint(offset);

        Gizmos.DrawWireSphere(pos, radius);
	}
}

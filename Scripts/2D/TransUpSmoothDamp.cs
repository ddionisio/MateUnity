using UnityEngine;
using System.Collections;

public class TransUpSmoothDamp : MonoBehaviour {
    public Vector3 up = Vector3.up;
    public float delay = 1.0f;
    public float maxSpeed = Mathf.Infinity;

    public Transform target; //optional

    private float mTime;
    private Vector3 mCurVel = Vector3.zero;

    private WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

    public void Go() {
        StopAllCoroutines();
        StartCoroutine(DoIt());
    }

    IEnumerator DoIt() {
        mTime = 0.0f;

        while(mTime <= delay) {
            float delta = Time.fixedDeltaTime;

            //assuming up property normalizes
            target.up = Vector3.SmoothDamp(target.up, up, ref mCurVel, delay, maxSpeed, delta);

            mTime += delta;

            yield return mWaitUpdate;
        }
    }

    void Awake() {
        if(target == null)
            target = transform;
    }
}

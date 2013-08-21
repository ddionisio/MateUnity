using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/AnimPulseDelayScale")]
public class TransAnimPulseDelayScale : MonoBehaviour {
    public float pauseStart;
    public float pulseDelay;

    public Vector3 ofs;

    public bool roundOutput = true; //only works for pixel-coordinate
    public bool squared = false;

    public Transform target; //optional

    private enum State {
        Pause,
        Pulse
    }

    private float mCurPulseTime = 0;

    private Vector3 mStart;
    private Vector3 mEnd;

    private State mState;
    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            target.localScale = mStart;
            mState = State.Pause;
            mCurPulseTime = 0;
        }
    }

    void OnDisable() {
        if(mStarted && target != null) {
            target.localScale = mStart;
        }
    }

    void Awake() {
        if(target == null)
            target = transform;

        mStart = target.localScale;
        mEnd = mStart + ofs;
    }

    // Use this for initialization
    void Start() {
        mState = pauseStart > 0.0f ? State.Pause : State.Pulse;
        mCurPulseTime = 0;
        mStarted = true;
    }

    void Update() {
        mCurPulseTime += Time.deltaTime;

        switch(mState) {
            case State.Pause:
                if(mCurPulseTime >= pauseStart) {
                    mState = State.Pulse;
                    mCurPulseTime = 0.0f;
                }
                break;

            case State.Pulse:
                if(mCurPulseTime < pulseDelay) {
                    float t = Mathf.Sin(Mathf.PI * (mCurPulseTime / pulseDelay));

                    Vector3 newPos = Vector3.Lerp(mStart, mEnd, squared ? t * t : t);

                    target.localScale = roundOutput ?
                        new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), Mathf.Round(newPos.z))
                        : newPos;
                }
                else {
                    target.localScale = mStart;

                    if(pauseStart > 0.0f) {
                        mState = State.Pause;
                        mCurPulseTime = 0.0f;
                    }
                    else {
                        mCurPulseTime -= pulseDelay;
                    }
                }
                break;
        }
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransAnimPulseDelayScale")]
public class TransAnimPulseDelayScale : MonoBehaviour {
    public float pauseStart;
    public float pulseDelay;

    public Vector2 ofs;

    public bool roundOutput = true; //only works for pixel-coordinate

    public Transform target; //optional

    private enum State {
        Pause,
        Pulse
    }

    private float mCurPulseTime = 0;

    private Vector2 mStart;
    private Vector2 mEnd;

    private State mState;
    private bool mStarted = false;

    void ApplyScale(Vector2 s) {
        Vector3 ls = target.localScale;
        ls.x = s.x; ls.y = s.y;
        target.localScale = ls;
    }

    void OnEnable() {
        if(mStarted) {
            ApplyScale(mStart);
            mState = State.Pause;
            mCurPulseTime = 0;
        }
    }

    void OnDisable() {
        if(mStarted && target != null) {
            ApplyScale(mStart);
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
        mState = State.Pause;
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
                    float t = Mathf.Sin(M8.MathUtil.TwoPI * (mCurPulseTime / pulseDelay));

                    Vector2 newPos = Vector2.Lerp(mStart, mEnd, t);

                    ApplyScale(roundOutput ?
                        new Vector2(Mathf.Round(newPos.x), Mathf.Round(newPos.y))
                        : newPos);
                }
                else {
                    ApplyScale(mStart);

                    mState = State.Pause;
                    mCurPulseTime = 0.0f;
                }
                break;
        }
    }
}

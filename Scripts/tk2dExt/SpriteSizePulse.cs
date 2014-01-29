using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/SpriteSizePulse")]
public class SpriteSizePulse : MonoBehaviour {
    public float pauseStart;
    public float pulseDelay;

    public Vector3 ofs;

    public bool squared;

    public tk2dBaseSprite target; //optional

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
            target.scale = mStart;
            mState = State.Pause;
            mCurPulseTime = 0;
        }
    }

    void OnDisable() {
        if(mStarted && target != null) {
            target.scale = mStart;
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<tk2dBaseSprite>();

        mStart = target.scale;
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
                    float t = Mathf.Sin(Mathf.PI * (mCurPulseTime / pulseDelay));

                    target.scale = Vector3.Lerp(mStart, mEnd, squared ? t * t : t);
                }
                else {
                    target.scale = mStart;

                    mState = State.Pause;
                    mCurPulseTime = 0.0f;
                }
                break;
        }
    }
}

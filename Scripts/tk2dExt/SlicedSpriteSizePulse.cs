using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/SlicedSpriteSizePulse")]
public class SlicedSpriteSizePulse : MonoBehaviour {
    public float pauseStart;
    public float pulseDelay;

    public Vector2 ofs;

    public bool roundOutput = true; //only works for pixel-coordinate
    public bool squared;

    public tk2dSlicedSprite target; //optional

    private enum State {
        Pause,
        Pulse
    }

    private float mCurPulseTime = 0;

    private Vector2 mStart;
    private Vector2 mEnd;

    private State mState;
    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            target.dimensions = mStart;
            mState = State.Pause;
            mCurPulseTime = 0;
        }
    }

    void OnDisable() {
        if(mStarted && target != null) {
            target.dimensions = mStart;
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<tk2dSlicedSprite>();

        mStart = target.dimensions;
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

                    Vector2 newPos = Vector2.Lerp(mStart, mEnd, squared ? t * t : t);

                    target.dimensions = roundOutput ?
                        new Vector2(Mathf.Round(newPos.x), Mathf.Round(newPos.y))
                        : newPos;
                }
                else {
                    target.dimensions = mStart;

                    mState = State.Pause;
                    mCurPulseTime = 0.0f;
                }
                break;
        }
    }
}

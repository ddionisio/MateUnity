using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/FillAnimation")]
public class NGUIFillAnim : MonoBehaviour {
    public enum Mode {
        Once,
        Repeat,
        PingPong
    }

    public delegate void GenericCall(NGUIFillAnim anim);

    public Mode mode = Mode.Once;
    public UISprite target; //NOTE: assumes type = Fill

    public float startDelay = 0.0f;
    public float delay = 1.0f;
    public float start = 0.0f;
    public float end = 1.0f;

    //TODO: tween type

    public bool autoPlay = false;
    public bool resetOnStop = true;
    public bool realtime = false;

    public event GenericCall finishCallback; //this is only called when mode = Mode.Once

    private enum State {
        None,
        Start,
        Playing
    }

    private State mState = State.None;
    private bool mStarted = false;
    private float mLastTime = 0;
    private bool mInvert;

    public bool isPlaying {
        get {
            return mState != State.None;
        }
    }

    public void Fill() {
        target.fillAmount = end;
        mInvert = false;
        mState = State.None;
    }

    public void Play() {
        if(mState == State.None) {
            mState = State.Start;
            mLastTime = realtime ? Time.realtimeSinceStartup : Time.time;
        }
    }

    public void Stop() {
        if(resetOnStop) {
            target.fillAmount = start;
            mInvert = false;
        }

        mState = State.None;
    }

    void OnDestroy() {
        finishCallback = null;
    }

    void OnEnable() {
        if(mStarted && autoPlay)
            Play();
    }

    void OnDisable() {
        Stop();
    }

    void Awake() {
        if(target == null)
            target = GetComponent<UISprite>();
    }

	// Use this for initialization
	void Start() {
        mStarted = true;

        if(autoPlay)
            Play();
	}
	
	// Update is called once per frame
	void Update() {
        float curTime;

	    switch(mState) {
            case State.Start:
                curTime = realtime ? Time.realtimeSinceStartup : Time.time;
                if(curTime - mLastTime >= startDelay) {
                    mLastTime = curTime;
                    mState = State.Playing;
                }
                break;

            case State.Playing:
                curTime = realtime ? Time.realtimeSinceStartup : Time.time;

                float t = Mathf.Clamp((curTime - mLastTime)/delay, 0.0f, 1.0f);

                //TODO: tween types
                target.fillAmount = mInvert ? Mathf.Lerp(end, start, t) : Mathf.Lerp(start, end, t);

                if(t >= 1.0f) {
                    mLastTime = curTime;

                    switch(mode) {
                        case Mode.Once:
                            mState = State.None;

                            if(finishCallback != null)
                                finishCallback(this);
                            break;

                        case Mode.PingPong:
                            mInvert = !mInvert;
                            break;
                    }
                }
                break;
        }
	}
}

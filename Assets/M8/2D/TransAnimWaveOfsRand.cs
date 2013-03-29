using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransAnimWaveOfsRand")]
public class TransAnimWaveOfsRand : MonoBehaviour {
    public float pauseStart;

    public float pauseMin;
    public float pauseMax;

	public float pulseDelayMin;
    public float pulseDelayMax;
	
	public Vector2 ofsMin;
    public Vector2 ofsMax;

    private enum State {
        Pause,
        Pulse
    }

	private float mCurPulseTime = 0;
    private float mPauseDelay;

    private float mPulseDelay;
	private Vector2 mStartPos;
	private Vector2 mEndPos;

    private State mState;
    private bool mStarted = false;

	void OnEnable() {
        if(mStarted) {
            transform.localPosition = new Vector3(mStartPos.x, mStartPos.y, transform.localPosition.z);
            mState = State.Pause;
            mCurPulseTime = 0;
            mPauseDelay = Random.Range(pauseMin, pauseMax);
        }
	}
	
	void Awake() {
		mStartPos = transform.localPosition;
	}
	
	// Use this for initialization
	void Start () {
        mState = State.Pause;
        mCurPulseTime = 0;
        mPauseDelay = pauseStart;
        mStarted = true;
	}
	
	// Update is called once per frame
	void Update () {
		mCurPulseTime += Time.deltaTime;

        switch(mState) {
            case State.Pause:
                if(mCurPulseTime >= mPauseDelay) {
                    mState = State.Pulse;
                    mCurPulseTime = 0.0f;
                    mPulseDelay = Random.Range(pulseDelayMin, pulseDelayMax);

                    mEndPos = mStartPos;
                    mEndPos.x += Random.Range(ofsMin.x, ofsMax.x);
                    mEndPos.y += Random.Range(ofsMin.y, ofsMax.y);
                }
                break;

            case State.Pulse:
                if(mCurPulseTime < mPulseDelay) {
                    float t = Mathf.Sin(M8.MathUtil.TwoPI * (mCurPulseTime / mPulseDelay));

		            Vector2 newPos = Vector2.Lerp(mStartPos, mEndPos, t);
		
		            transform.localPosition = new Vector3(newPos.x, newPos.y, transform.localPosition.z);
                }
                else {
                    transform.localPosition = new Vector3(mStartPos.x, mStartPos.y, transform.localPosition.z);

                    mState = State.Pause;
                    mCurPulseTime = 0.0f;
                    mPauseDelay = Random.Range(pauseMin, pauseMax);
                }
                break;
        }
	}
}

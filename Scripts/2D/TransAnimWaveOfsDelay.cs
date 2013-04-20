using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransAnimWaveOfsDelay")]
public class TransAnimWaveOfsDelay : MonoBehaviour {
    public float pauseStart;
    public float pulseDelay;

    public Vector2 ofs;

    public bool roundOutput; //only works for pixel-coordinate

    public Transform target; //optional

    private enum State {
        Pause,
        Pulse
    }

	private float mCurPulseTime = 0;

	private Vector2 mStartPos;
	private Vector2 mEndPos;

    private State mState;
    private bool mStarted = false;

	void OnEnable() {
        if(mStarted) {
            target.localPosition = new Vector3(mStartPos.x, mStartPos.y, target.localPosition.z);
            mState = State.Pause;
            mCurPulseTime = 0;
        }
	}
	
	void Awake() {
        if(target == null)
            target = transform;

        mStartPos = target.localPosition;
        mEndPos = mStartPos + ofs;
	}
	
	// Use this for initialization
	void Start () {
        mState = State.Pause;
        mCurPulseTime = 0;
        mStarted = true;
	}
	
	// Update is called once per frame
	void Update () {
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

		            Vector2 newPos = Vector2.Lerp(mStartPos, mEndPos, t);

                    target.localPosition = roundOutput ?
                        new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), target.localPosition.z)
                        : new Vector3(newPos.x, newPos.y, target.localPosition.z);
                }
                else {
                    target.localPosition = new Vector3(mStartPos.x, mStartPos.y, target.localPosition.z);

                    mState = State.Pause;
                    mCurPulseTime = 0.0f;
                }
                break;
        }
	}
}

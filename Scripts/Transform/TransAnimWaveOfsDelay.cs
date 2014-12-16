using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimWaveOfsDelay")]
    public class TransAnimWaveOfsDelay : MonoBehaviour {
        public float pauseStart;
        public float pulseDelay;

        public Vector3 ofs;

        public bool roundOutput; //only works for pixel-coordinate
        public bool squared;

        public Transform target; //optional

        public bool local = true;

        private enum State {
            Pause,
            Pulse
        }

        private float mCurPulseTime = 0;

        private Vector3 mStartPos;
        private Vector3 mEndPos;

        private State mState;
        private bool mStarted = false;

        void OnEnable() {
            if(mStarted) {
                if(local)
                    target.localPosition = mStartPos;
                else
                    target.position = mStartPos;

                mState = State.Pause;
                mCurPulseTime = 0;
            }
        }

        void Awake() {
            if(target == null)
                target = transform;

            mStartPos = local ? target.localPosition : target.position;
            mEndPos = mStartPos + ofs;
        }

        // Use this for initialization
        void Start() {
            mState = State.Pause;
            mCurPulseTime = 0;
            mStarted = true;
        }

        // Update is called once per frame
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

                        Vector3 newPos = Vector3.Lerp(mStartPos, mEndPos, squared ? t * t : t);

                        if(roundOutput)
                            newPos.Set(Mathf.Round(newPos.x), Mathf.Round(newPos.y), Mathf.Round(newPos.z));

                        if(local)
                            target.localPosition = newPos;
                        else
                            target.position = newPos;
                    }
                    else {
                        if(local)
                            target.localPosition = mStartPos;
                        else
                            target.position = mStartPos;

                        mState = State.Pause;
                        mCurPulseTime = 0.0f;
                    }
                    break;
            }
        }
    }
}
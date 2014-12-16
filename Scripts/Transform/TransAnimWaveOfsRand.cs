using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimWaveOfsRand")]
    public class TransAnimWaveOfsRand : MonoBehaviour {
        public Transform target;

        public float pauseStart;

        public float pauseMin;
        public float pauseMax;

        public float pulseDelayMin;
        public float pulseDelayMax;

        public Vector3 ofsMin;
        public Vector3 ofsMax;

        public bool squared;

        public bool local = true;

        private enum State {
            Pause,
            Pulse
        }

        private float mCurPulseTime = 0;
        private float mPauseDelay;

        private float mPulseDelay;
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
                mPauseDelay = Random.Range(pauseMin, pauseMax);
            }
        }

        void Awake() {
            if(target == null)
                target = transform;

            mStartPos = local ? target.localPosition : target.position;
        }

        // Use this for initialization
        void Start() {
            mState = State.Pause;
            mCurPulseTime = 0;
            mPauseDelay = pauseStart;
            mStarted = true;
        }

        // Update is called once per frame
        void Update() {
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
                        mEndPos.z += Random.Range(ofsMin.z, ofsMax.z);
                    }
                    break;

                case State.Pulse:
                    if(mCurPulseTime < mPulseDelay) {
                        float t = Mathf.Sin(Mathf.PI * (mCurPulseTime / mPulseDelay));

                        Vector3 newPos = Vector3.Lerp(mStartPos, mEndPos, squared ? t * t : t);

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
                        mPauseDelay = Random.Range(pauseMin, pauseMax);
                    }
                    break;
            }
        }
    }
}
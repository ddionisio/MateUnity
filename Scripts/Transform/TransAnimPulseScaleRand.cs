using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimPulseScaleRand")]
    public class TransAnimPulseScaleRand : MonoBehaviour {
        public enum UpdateType {
            Update,
            FixedUpdate,
            Realtime
        }

        public Transform target; //optional

        public UpdateType updateType = UpdateType.Update;

        public RangeFloat pauseDelay;
        public RangeFloat pulseDelay;

        public Vector3 ofsMin;
        public Vector3 ofsMax;

        public bool startPause;
        public bool roundOutput = false; //only works for pixel-coordinate
        public bool squared = true;
                
        private enum State {
            Pause,
            Pulse
        }

        private float mDelay;
        private float mCurPulseTime = 0;
        private float mLastTime = 0;

        private Vector3 mStart;
        private Vector3 mEnd;

        private State mState;
        private bool mStarted = false;
                
        void OnEnable() {
            if(mStarted) {
                target.localScale = mStart;
                                
                if(startPause) {
                    mState = State.Pause;
                    mDelay = pauseDelay.random;
                }
                else {
                    mState = State.Pulse;
                    mDelay = pulseDelay.random;

                    RefreshEnd();
                }

                mCurPulseTime = 0;
                mLastTime = GetTime();
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
        }

        // Use this for initialization
        void Start() {
            mStarted = true;
            mStart = target.localScale;
            OnEnable();
        }

        float GetTime() {
            switch(updateType) {
                case UpdateType.Update:
                    return Time.time;
                case UpdateType.FixedUpdate:
                    return Time.fixedTime;
                case UpdateType.Realtime:
                    return Time.realtimeSinceStartup;
            }
            return 0.0f;
        }

        void Update() {
            float time = GetTime();
            mCurPulseTime += time - mLastTime;
            mLastTime = time;

            switch(mState) {
                case State.Pause:
                    if(mCurPulseTime >= mDelay) {
                        mState = State.Pulse;
                                                
                        mCurPulseTime = 0.0f;

                        mDelay = pulseDelay.random;
                        RefreshEnd();
                    }
                    break;

                case State.Pulse:
                    if(mCurPulseTime < mDelay) {
                        float t = Mathf.Sin(Mathf.PI * (mCurPulseTime / mDelay));

                        Vector3 newPos = Vector3.Lerp(mStart, mEnd, squared ? t * t : t);

                        target.localScale = roundOutput ?
                        new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), Mathf.Round(newPos.z))
                        : newPos;
                    }
                    else {
                        target.localScale = mStart;

                        var _pauseDelay = pauseDelay.random;
                        if(_pauseDelay > 0.0f) {
                            mState = State.Pause;

                            mCurPulseTime = 0.0f;

                            mDelay = _pauseDelay;
                        }
                        else {
                            mCurPulseTime -= mDelay;

                            mDelay = pulseDelay.random;
                            RefreshEnd();
                        }
                    }
                    break;
            }
        }

        private void RefreshEnd() {
            mEnd = new Vector3(mStart.x + Random.Range(ofsMin.x, ofsMax.x), mStart.y + Random.Range(ofsMin.y, ofsMax.y), mStart.z + Random.Range(ofsMin.z, ofsMax.z));
        }
    }
}
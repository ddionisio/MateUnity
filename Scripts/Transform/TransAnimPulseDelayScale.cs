using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimPulseDelayScale")]
    public class TransAnimPulseDelayScale : MonoBehaviour {
        public enum UpdateType {
            Update,
            FixedUpdate,
            Realtime
        }

        public UpdateType updateType = UpdateType.Update;

        public float pauseDelay;
        public float pulseDelay;

        public Vector3 ofs;

        public bool startPause;
        public bool roundOutput = true; //only works for pixel-coordinate
        public bool squared = false;

        public Transform target; //optional

        private enum State {
            Pause,
            Pulse
        }

        private float mCurPulseTime = 0;
        private float mLastTime = 0;

        private Vector3 mStart;
        private Vector3 mEnd;

        private State mState;
        private bool mStarted = false;

        void OnEnable() {
            if(mStarted) {
                target.localScale = mStart;
                mState = startPause ? State.Pause : State.Pulse;
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

            mStart = target.localScale;
            mEnd = mStart + ofs;
        }

        // Use this for initialization
        void Start() {
            mStarted = true;
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
                    if(mCurPulseTime >= pauseDelay) {
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

                        if(pauseDelay > 0.0f) {
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
}
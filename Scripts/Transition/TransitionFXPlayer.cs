using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Wrapper for TransitionFX to play during runtime
    /// </summary>
    [AddComponentMenu("M8/TransitionFX/Player")]
    public class TransitionFXPlayer : MonoBehaviour {

        public TransitionFX fx;
        public float delay;
        public bool isTimeScaled;
        public bool playOnStart;

        public Signal signalEnded;

        public System.Action endCallback;

        /// <summary>
        /// Grab current progress. If setting this manually, make sure to call End once you're done.
        /// </summary>
        public float curT {
            get { return mCurT; }

            set {
                if(mPlayRout != null) { //fail-safe
                    StopCoroutine(mPlayRout);
                    mPlayRout = null;
                }

                mCurT = Mathf.Clamp01(value);

                if(fx)
                    fx.UpdateTime(mCurT);
            }
        }

        public bool isPlaying { get { return mPlayRout != null; } }

        private Coroutine mPlayRout;
        private float mCurT;

        public void Play() {
            if(mPlayRout != null || !fx)
                return;

            mPlayRout = StartCoroutine(DoPlay());
        }

        public void End() {
            if(mPlayRout != null) {
                StopCoroutine(mPlayRout);
                mPlayRout = null;
            }

            if(fx)
                fx.End();
        }
                
        void OnDisable() {
            End();
        }

        void OnDestroy() {
            if(fx)
                fx.Deinit();
        }

        void Start() {
            if(playOnStart)
                Play();
        }

        IEnumerator DoPlay() {
            fx.UpdateTime(0f);

            float curTime = 0f;
            while(curTime < delay) {
                yield return null;

                curTime += isTimeScaled ? Time.deltaTime : Time.unscaledDeltaTime;

                mCurT = Mathf.Clamp01(curTime / delay);

                fx.UpdateTime(mCurT);
            }

            if(signalEnded)
                signalEnded.Invoke();

            if(endCallback != null)
                endCallback();

            yield return null;

            fx.End();

            mPlayRout = null;
        }
    }
}
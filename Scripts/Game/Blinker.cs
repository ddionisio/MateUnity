using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("")]
    public class Blinker : MonoBehaviour {
        public delegate void Callback(bool aActive);

        [SerializeField]
        float _delay = 0.2f;

        [SerializeField]
        float _interval = 0.01f;

        public event Callback activeCallback;

        private bool mBlinking;
        private bool mYes;
        private float mDuration;
        private float mBlinkTime;

        public bool isBlinking { get { return mBlinking; } }

        void OnDisable() {
            if(mBlinking) {
                mBlinking = false;
                mDuration = 0.0f;
                mBlinkTime = 0.0f;
                mYes = false;
                OnBlink(false);
            }
        }

        void OnDestroy() {
            activeCallback = null;
        }

        public void Blink(float delay) {
            if(delay > 0.0f) {
                if(mBlinking)
                    mDuration = delay;
                else {
                    mDuration = delay;
                    StartCoroutine(DoBlink());
                }
            }
            else {
                StopAllCoroutines();
                BlinkStateSet(false);
            }
        }

        public void Blink() {
            Blink(_delay);
        }

        public void Stop() {
            Blink(0.0f);
        }

        protected virtual void OnBlink(bool yes) {
            //implement
        }

        protected virtual void OnBlinkStateChanged() {
            //implement
        }

        void BlinkStateSet(bool blink) {
            if(mBlinking != blink) {
                mBlinking = blink;
                if(activeCallback != null)
                    activeCallback(blink);

                if(!mBlinking) {
                    mYes = false;
                    OnBlink(false);
                }

                OnBlinkStateChanged();
            }
        }

        IEnumerator DoBlink() {
            BlinkStateSet(true);

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while(mDuration > 0.0f) {
                yield return wait;

                float dt = Time.fixedDeltaTime;

                mBlinkTime += dt;
                if(mBlinkTime >= _interval) {
                    OnBlink(mYes = !mYes);
                    mBlinkTime -= _interval;
                }

                mDuration -= Time.fixedDeltaTime;
            }

            BlinkStateSet(false);
        }
    }
}
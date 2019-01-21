using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    /// <summary>
    /// Hold for a given duration, then click.
    /// Sends event for value [0, 1], and then click once value reaches 1
    /// </summary>
    [AddComponentMenu("M8/UI/Events/ClickHold")]
    public class EventClickHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [Header("Config")]
        public float delay = 2f;
        public bool isRealtime;

        [Header("Events")]
        public Slider.SliderEvent valueEvent;
        public UnityEvent clickEvent;

        private float mCurVal = 0f;
        private float mLastTime;
        private bool mIsHold = false;

        void OnDisable() {
            mCurVal = 0f;
            mIsHold = false;
        }

        void OnEnable() {
            valueEvent.Invoke(mCurVal);
        }

        void Update() {
            if(mIsHold) {
                float curTime = isRealtime ? Time.realtimeSinceStartup : Time.time;
                float dt = curTime - mLastTime;
                float val = Mathf.Clamp01(dt / delay);
                if(mCurVal != val) {
                    mCurVal = val;                   

                    if(mCurVal == 1.0f)
                        Click();
                    else
                        valueEvent.Invoke(mCurVal);
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(!mIsHold) {
                mIsHold = true;
                mLastTime = isRealtime ? Time.realtimeSinceStartup : Time.time;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            Release();
        }

        private void Click() {
            Release();

            clickEvent.Invoke();
        }

        private void Release() {
            if(mCurVal != 0f) {
                mCurVal = 0f;
                valueEvent.Invoke(mCurVal);
            }

            mIsHold = false;
        }
    }
}
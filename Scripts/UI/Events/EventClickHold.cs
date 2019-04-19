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
    public class EventClickHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler {
        [Header("Config")]
        public float delay = 2f;
        public bool isRealtime;
        public bool ignoreDrag = true; //if false, cancel hold if drag begins

        [Header("Input")]
        public InputAction input; //if this is selected and input is "down", consider it as a hold

        [Header("Events")]
        public Slider.SliderEvent valueEvent;
        public UnityEvent clickEvent;

        private float mCurVal = 0f;
        private float mLastTime;
        private bool mIsHold = false;

        private bool mIsPointerDown = false;
        private bool mIsInputDown = false;

        void OnDisable() {
            mCurVal = 0f;
            mIsHold = false;
            mIsPointerDown = false;
            mIsInputDown = false;
        }

        void OnEnable() {
            valueEvent.Invoke(mCurVal);
        }

        void Update() {
            InputUpdate();

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

        void InputUpdate() {
            var inputIsDown = false;

            if(input) {
                var es = EventSystem.current;
                bool isSelected = es ? es.currentSelectedGameObject == gameObject : false;
                if(isSelected)
                    inputIsDown = input.IsDown();
                else
                    inputIsDown = false;
            }

            if(mIsInputDown != inputIsDown) {
                mIsInputDown = inputIsDown;
                UpdateHold();
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            if(!ignoreDrag) {
                mIsPointerDown = false;
                UpdateHold();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            mIsPointerDown = true;
            UpdateHold();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            mIsPointerDown = false;
            UpdateHold();
        }

        void UpdateHold() {
            bool hold = mIsPointerDown || mIsInputDown;
            if(mIsHold != hold) {
                mIsHold = hold;
                if(mIsHold) {
                    mLastTime = isRealtime ? Time.realtimeSinceStartup : Time.time;
                }
                else
                    Release();
            }
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
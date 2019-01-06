using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    /// <summary>
    /// Use this to allow callback for pressed and released, mostly for button type actions.
    /// </summary>
    [AddComponentMenu("M8/Input/Action Event")]
    public class InputActionEvent : MonoBehaviour {
        [System.Serializable]
        public struct Data {
            public InputAction action;
            public UnityEventInputAction pressedCallback;
            public UnityEventInputAction releasedCallback;

            public InputAction.ButtonState curState { get { return mCurState; } }

            private InputAction.ButtonState mCurState;

            public void Reset() {
                mCurState = InputAction.ButtonState.None;
            }

            public void Update() {
                var newState = action.GetButtonState();

                //state has changed
                if(mCurState != newState) {
                    mCurState = newState;

                    switch(mCurState) {
                        case InputAction.ButtonState.Pressed:
                            if(pressedCallback != null)
                                pressedCallback.Invoke(action);
                            break;
                        case InputAction.ButtonState.Released:
                            if(releasedCallback != null)
                                releasedCallback.Invoke(action);
                            break;
                    }
                }
            }
        }

        public Data[] actions;

        void OnDisable() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].Reset();
        }

        void Update() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].Update();
        }
    }
}
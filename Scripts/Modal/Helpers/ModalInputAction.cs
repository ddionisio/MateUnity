using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    /// <summary>
    /// Use this to allow callback for pressed and released, mostly for button type actions.
    /// </summary>
    [AddComponentMenu("M8/Modal/Helpers/Input Action")]
    public class ModalInputAction : MonoBehaviour, IModalActive {
        [System.Serializable]
        public struct Data {
            public InputAction action;
            public UnityEventInputAction callback;

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
                            if(callback != null)
                                callback.Invoke(action);
                            break;
                    }
                }
            }
        }

        public Data[] actions;

        private bool mIsActive;

        void IModalActive.SetActive(bool aActive) {
            mIsActive = aActive;
            if(!mIsActive) {
                for(int i = 0; i < actions.Length; i++)
                    actions[i].Reset();
            }
        }

        void OnDisable() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].Reset();
        }

        void Update() {
            if(SceneManager.isInstantiated && SceneManager.instance.isLoading)
                return;

            for(int i = 0; i < actions.Length; i++)
                actions[i].Update();
        }
    }
}
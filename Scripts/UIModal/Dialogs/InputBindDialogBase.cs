using UnityEngine;
using System.Collections;

namespace M8.UIModal.Dialogs {
    public abstract class InputBindDialogBase : Controller {
        public delegate void BindFinishCall(InputBindDialogBase ctrl, int actionIndex, int actionKeyIndex);

        [System.Serializable]
        public class ActionData {
            public string name;
            public int index;
            public bool localized;
            public int[] keys;
        }

        public ActionData[] actions;

        public event BindFinishCall bindFinishCallback;

        private int mPlayerInd = 0;

        private bool mBindActive;
        private int mActionBindInd = -1;
        private int mActionBindKeyInd = -1;

        private int mLastActionBindInd = -1;
        private int mLastActionBindKeyInd = -1;
        private float mLastTime;

        private bool mIsDirty;

        private System.Array mKeyCodes;

        public bool isWaitingInput {
            get { return mBindActive || (mActionBindInd != -1 && mActionBindKeyInd != -1); }
        }

        public int playerIndex {
            get { return mPlayerInd; }
            set { mPlayerInd = value; }
        }

        public bool isDirty {
            get { return mIsDirty; }
        }

        public void Bind(int actionIndex, int actionKeyIndex) {
            mActionBindInd = actionIndex;
            mActionBindKeyInd = actionKeyIndex;
            mBindActive = true;
        }

        public void Save() {
            UserSettingInput.instance.Apply();
            UserSettingInput.instance.Save();
            mIsDirty = false;
            Localize.instance.Refresh(); //for labels using input stuff
        }

        public void Revert(bool toDefault) {
            UserSettingInput.instance.Revert(toDefault);
            mIsDirty = false;
            Localize.instance.Refresh(); //for labels using input stuff
        }

        protected virtual void OnDestroy() {
            bindFinishCallback = null;
        }

        protected virtual void Awake() {
            mKeyCodes = System.Enum.GetValues(typeof(KeyCode));
        }

        protected virtual void BindFinish(int actionIndex, int actionKeyIndex) {
        }

        void Update() {
            if(mBindActive) {
                if(mActionBindInd != -1 && mActionBindKeyInd != -1) {
                    InputManager input = InputManager.instance;

                    bool done = false;

                    if(UnityEngine.Input.GetKeyDown(KeyCode.Escape)) {
                        done = true;
                    }
                    else {
                        //hm
                        for(int i = 0; i < mKeyCodes.Length; i++) {
                            KeyCode key = (KeyCode)mKeyCodes.GetValue(i);
                            if(UnityEngine.Input.GetKeyDown(key)) {
                                ActionData dat = actions[mActionBindInd];
                                InputManager.Key keyDat = input.GetBindKey(mPlayerInd, dat.index, dat.keys[mActionBindKeyInd]);

                                //check if this key is already bound,
                                //if so, then just swap keycode
                                InputManager.Key conflictKeyDat = null;
                                for(int a = 0; a < actions.Length; a++) {
                                    if(a != mActionBindInd) {
                                        for(int k = 0; k < actions[a].keys.Length; k++) {
                                            InputManager.Key kDat = input.GetBindKey(mPlayerInd, actions[a].index, actions[a].keys[k]);
                                            if(kDat.code == key) {
                                                conflictKeyDat = kDat;
                                                break;
                                            }
                                        }
                                    }
                                    else {
                                        for(int k = 0; k < actions[a].keys.Length; k++) {
                                            if(k != mActionBindKeyInd) {
                                                InputManager.Key kDat = input.GetBindKey(mPlayerInd, actions[a].index, actions[a].keys[k]);
                                                if(kDat.code == key) {
                                                    conflictKeyDat = kDat;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if(conflictKeyDat != null)
                                        break;
                                }

                                if(conflictKeyDat != null) {
                                    conflictKeyDat.SetAsKey(keyDat.code);
                                    conflictKeyDat.SetDirty(true);
                                }

                                keyDat.SetAsKey(key);
                                keyDat.SetDirty(true);
                                done = true;

                                mIsDirty = true;
                                break;
                            }
                        }
                    }

                    if(done) {
                        mLastActionBindInd = mActionBindInd;
                        mLastActionBindKeyInd = mActionBindKeyInd;
                        mActionBindInd = -1;
                        mActionBindKeyInd = -1;

                        mLastTime = Time.realtimeSinceStartup;

                        Localize.instance.Refresh(); //for labels using input stuff
                    }
                }
                else if(Time.realtimeSinceStartup - mLastTime >= 0.2f) {
                    mBindActive = false;

                    BindFinish(mLastActionBindInd, mLastActionBindKeyInd);

                    if(bindFinishCallback != null)
                        bindFinishCallback(this, mLastActionBindInd, mLastActionBindKeyInd);
                }
            }
        }
    }
}
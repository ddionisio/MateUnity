using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//generalized input handling, useful for porting to non-unity conventions (e.g. Ouya)
[AddComponentMenu("M8/Core/InputManager")]
public class InputManager : MonoBehaviour {
    public delegate void OnButton(Info data);

    public enum State {
        None,
        Pressed,
        Released
    }

    public enum Control {
        Button,
        Axis
    }

    public enum ButtonAxis {
        None,
        Plus,
        Minus
    }

    public struct Info {
        public State state;
        public float axis;
        public int index;
    }

    public class Key {
        public string input = ""; //for use with unity's input
        public KeyCode code = KeyCode.None; //unity
        public InputKeyMap map = InputKeyMap.None; //for external (like ouya!)
        public ButtonAxis axis = ButtonAxis.None; //for buttons as axis
        public int index = 0; //which index this key refers to

        public float GetAxisValue() {
            float ret = 0.0f;

            switch(axis) {
                case ButtonAxis.Plus:
                    ret = 1.0f;
                    break;
                case ButtonAxis.Minus:
                    ret = -1.0f;
                    break;
            }

            return ret;
        }
    }

    public class Bind {
        public InputAction action = (InputAction)0;
        public Control control = InputManager.Control.Button;

        public float deadZone = 0.1f;

        public Key[] keys;
    }

    public TextAsset config;

    protected class BindData {
        public Info info;

        public Control control;

        public float deadZone;

        public Key[] keys;

        public event OnButton callback;

        public bool down;

        public BindData(Bind bind) {
            control = bind.control;
            deadZone = bind.deadZone;
            keys = bind.keys;

            down = false;
        }

        public void ClearCallback() {
            callback = null;
        }

        public void Call() {
            if(callback != null) {
                callback(info);
            }
        }
    }

    protected BindData[] mBinds = new BindData[(int)InputAction.NumAction];

    //interfaces (available after awake)

    public bool CheckBind(InputAction action) {
        return mBinds[(int)action] != null;
    }

    public float GetAxis(InputAction action) {
        return mBinds[(int)action].info.axis;
    }

    public State GetState(InputAction action) {
        return mBinds[(int)action].info.state;
    }

    public bool IsDown(InputAction action) {
        foreach(Key key in mBinds[(int)action].keys) {
            if(ProcessButtonDown(key)) {
                return true;
            }
        }

        return false;
    }

    public int GetIndex(InputAction action) {
        return mBinds[(int)action].info.index;
    }

    public void AddButtonCall(InputAction action, OnButton callback) {
        mBinds[(int)action].callback += callback;
    }

    public void RemoveButtonCall(InputAction action, OnButton callback) {
        mBinds[(int)action].callback -= callback;
    }

    public void ClearButtonCall(InputAction action) {
        mBinds[(int)action].ClearCallback();
    }

    public void ClearAllButtonCalls() {
        for(int i = 0; i < (int)InputAction.NumAction; i++) {
            mBinds[i].ClearCallback();
        }
    }

    //implements

    protected virtual float ProcessAxis(Key key, float deadZone) {
        if(key.input.Length > 0) {
            return Input.GetAxis(key.input);
        }
        else if(key.code != KeyCode.None) {
            if(Input.GetKey(key.code)) {
                return key.GetAxisValue();
            }
        }

        return 0.0f;
    }

    protected virtual bool ProcessButtonDown(Key key) {
        return
            key.input.Length > 0 ? Input.GetButton(key.input) :
            key.code != KeyCode.None ? Input.GetKey(key.code) :
            false;
    }

    //internal

    protected virtual void OnDestroy() {
        foreach(BindData bind in mBinds) {
            if(bind != null) {
                bind.ClearCallback();
            }
        }
    }

    protected virtual void Awake() {
        if(config != null) {
            fastJSON.JSON.Instance.Parameters.UseExtensions = false;
            List<Bind> keys = fastJSON.JSON.Instance.ToObject<List<Bind>>(config.text);

            foreach(Bind key in keys) {
                mBinds[(int)key.action] = new BindData(key);
            }
        }
    }

    protected virtual void FixedUpdate() {
        foreach(BindData bindData in mBinds) {
            if(bindData != null && bindData.keys != null) {
                switch(bindData.control) {
                    case Control.Axis:
                        bindData.info.axis = 0.0f;

                        foreach(Key key in bindData.keys) {
                            float axis = ProcessAxis(key, bindData.deadZone);
                            if(axis != 0.0f) {
                                bindData.info.axis = axis;
                                break;
                            }
                        }
                        break;

                    case Control.Button:
                        bindData.info.state = State.None;

                        foreach(Key key in bindData.keys) {
                            if(ProcessButtonDown(key)) {
                                if(!bindData.down) {
                                    bindData.down = true;

                                    bindData.info.axis = key.GetAxisValue();
                                    bindData.info.state = State.Pressed;
                                    bindData.info.index = key.index;

                                    bindData.Call();

                                    break;
                                }
                            }
                            else {
                                if(bindData.down) {
                                    bindData.down = false;

                                    bindData.info.axis = 0.0f;
                                    bindData.info.state = State.Released;
                                    bindData.info.index = key.index;

                                    bindData.Call();

                                    break;
                                }
                            }
                        }
                        break;
                }
            }
        }
    }
}

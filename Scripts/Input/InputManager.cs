using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//generalized input handling, useful for porting to non-unity conventions (e.g. Ouya)
[AddComponentMenu("M8/Core/InputManager")]
public class InputManager : MonoBehaviour {
    public const int ActionInvalid = -1;

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

        public void ResetKeys() {
            input = "";
            code = KeyCode.None;
            map = InputKeyMap.None;
        }

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
        public int action = 0;
        public Control control = InputManager.Control.Button;

        public float deadZone = 0.1f;

        public List<Key> keys = null;
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
            keys = bind.keys != null ? bind.keys.ToArray() : new Key[0];

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

    protected BindData[] mBinds;

    //interfaces (available after awake)

    public bool CheckBind(int action) {
        return mBinds[action] != null;
    }

    public float GetAxis(int action) {
        return mBinds[action].info.axis;
    }

    public State GetState(int action) {
        return mBinds[action].info.state;
    }

    public bool IsDown(int action) {
        foreach(Key key in mBinds[action].keys) {
            if(ProcessButtonDown(key)) {
                return true;
            }
        }

        return false;
    }

    public int GetIndex(int action) {
        return mBinds[action].info.index;
    }

    public void AddButtonCall(int action, OnButton callback) {
        mBinds[action].callback += callback;
    }

    public void RemoveButtonCall(int action, OnButton callback) {
        mBinds[action].callback -= callback;
    }

    public void ClearButtonCall(int action) {
        mBinds[action].ClearCallback();
    }

    public void ClearAllButtonCalls() {
        for(int i = 0; i < mBinds.Length; i++) {
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
            Dictionary<int, BindData> binds = new Dictionary<int, BindData>();

            fastJSON.JSON.Instance.Parameters.UseExtensions = false;
            List<Bind> keys = fastJSON.JSON.Instance.ToObject<List<Bind>>(config.text);

            int highestActionInd = 0;

            foreach(Bind key in keys) {
                binds.Add(key.action, new BindData(key));

                if(key.action > highestActionInd)
                    highestActionInd = key.action;
            }

            mBinds = new BindData[highestActionInd + 1];
            foreach(KeyValuePair<int, BindData> pair in binds) {
                mBinds[pair.Key] = pair.Value;
            }
        }
        else {
            mBinds = new BindData[0];
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

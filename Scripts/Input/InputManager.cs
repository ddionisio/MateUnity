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

    public struct PlayerRegistry {
    }

    public class Key {
        public int player = 0;

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

    public int numPlayers = 1;

    protected class PlayerData {
        public Info info;

        public bool down;

        public OnButton callback;

        public PlayerData() {
            down = false;
            callback = null;
        }
    }
    
    protected class BindData {
        public Control control;

        public float deadZone;

        public PlayerData[] players;

        public Key[] keys;

        public BindData(Bind bind, int numPlayers) {
            control = bind.control;
            deadZone = bind.deadZone;

            keys = bind.keys.ToArray();

            players = new PlayerData[numPlayers];
            for(int i = 0; i < numPlayers; i++) {
                players[i] = new PlayerData();
            }
        }

        public void ClearCallback() {
            foreach(PlayerData pd in players) {
                pd.callback = null;    
            }
        }
    }

    protected BindData[] mBinds;

    //interfaces (available after awake)

    public bool CheckBind(int action) {
        return mBinds[action] != null;
    }

    public float GetAxis(int player, int action) {
        return mBinds[action].players[player].info.axis;
    }

    public State GetState(int player, int action) {
        return mBinds[action].players[player].info.state;
    }

    public bool IsDown(int player, int action) {
        Key[] keys = mBinds[action].keys;

        foreach(Key key in keys) {
            if(key.player == player && ProcessButtonDown(key)) {
                return true;
            }
        }

        return false;
    }

    public int GetIndex(int player, int action) {
        return mBinds[action].players[player].info.index;
    }

    public void AddButtonCall(int player, int action, OnButton callback) {
        mBinds[action].players[player].callback += callback;
    }

    public void RemoveButtonCall(int player, int action, OnButton callback) {
        mBinds[action].players[player].callback -= callback;
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
            //float val = Input.GetAxis(key.input);
            //return Mathf.Abs(val) > deadZone ? val : 0.0f;
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
                binds.Add(key.action, new BindData(key, numPlayers));

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
                foreach(PlayerData pd in bindData.players) {
                    pd.info.state = State.None;
                    pd.info.axis = 0.0f;
                }

                switch(bindData.control) {
                    case Control.Axis:
                        foreach(Key key in bindData.keys) {
                            PlayerData pd = bindData.players[key.player];

                            float axis = ProcessAxis(key, bindData.deadZone);
                            if(axis != 0.0f) {
                                pd.info.axis = axis;
                                break;
                            }
                        }
                        break;

                    case Control.Button:
                        foreach(Key key in bindData.keys) {
                            PlayerData pd = bindData.players[key.player];

                            if(ProcessButtonDown(key)) {
                                if(!pd.down) {
                                    pd.down = true;

                                    pd.info.axis = key.GetAxisValue();
                                    pd.info.state = State.Pressed;
                                    pd.info.index = key.index;

                                    if(pd.callback != null)
                                        pd.callback(pd.info);

                                    break;
                                }
                            }
                            else {
                                if(pd.down) {
                                    pd.down = false;

                                    pd.info.axis = 0.0f;
                                    pd.info.state = State.Released;
                                    pd.info.index = key.index;

                                    if(pd.callback != null)
                                        pd.callback(pd.info);

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

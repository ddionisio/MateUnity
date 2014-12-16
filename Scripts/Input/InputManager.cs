using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    //generalized input handling, useful for porting to non-unity conventions (e.g. Ouya)
    [PrefabCore]
    [AddComponentMenu("M8/Core/InputManager")]
    public class InputManager : SingletonBehaviour<InputManager> {
        public const int PlayerDefault = 0;
        public const int MaxPlayers = 8;
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
            Minus,
            Both
        }

        public struct Info {
            public int action;
            public State state;
            public float axis;
            public int index;
        }

        public class Key {
            public int player = 0;

            public string input = ""; //for use with unity's input
            public KeyCode code = KeyCode.None; //unity
            public InputKeyMap map = InputKeyMap.None; //for external (like ouya!)

            public bool invert; //for axis, flip sign

            public ButtonAxis axis = ButtonAxis.None; //for buttons as axis
            public int index = 0; //which index this key refers to

            private bool mDirty = false;

            public bool isValid {
                get { return !string.IsNullOrEmpty(input) || code != KeyCode.None || map != InputKeyMap.None; }
            }

            public void SetAsInput(string input) {
                ResetKeys();

                this.input = input;
            }

            public void SetAsKey(KeyCode aCode) {
                ResetKeys();
                code = aCode;
            }

            public void SetDirty(bool dirty) {
                mDirty = dirty;
            }

            public string GetKeyString() {
                if(!string.IsNullOrEmpty(input))
                    return input;

                if(code != KeyCode.None) {
                    if(code == KeyCode.Escape)
                        return "ESC";
                    else {
                        string s = code.ToString();

                        int i = s.IndexOf("Joystick");
                        if(i != -1) {
                            int bInd = s.LastIndexOf('B');
                            if(bInd != -1) {
                                return s.Substring(bInd);
                            }
                        }

                        return s;
                    }
                }

                if(map != InputKeyMap.None && map != InputKeyMap.NumKeys) {
                    return map.ToString();
                }

                return "";
            }

            void _ApplyInfo(uint dataPak) {
                ushort s1 = M8.Util.GetHiWord(dataPak);

                axis = (ButtonAxis)M8.Util.GetHiByte(s1);
                index = M8.Util.GetLoByte(s1);
            }

            public int _CreateKeyCodeDataPak() {
                if(code != KeyCode.None)
                    return (int)M8.Util.MakeLong(M8.Util.MakeWord((byte)axis, (byte)index), (ushort)code);
                return 0;
            }

            public int _CreateMapDataPak() {
                if(map != InputKeyMap.None)
                    return (int)M8.Util.MakeLong(M8.Util.MakeWord((byte)axis, (byte)index), (ushort)map);
                return 0;
            }

            public void _SetAsKey(uint dataPak) {
                ResetKeys();

                _ApplyInfo(dataPak);

                code = (KeyCode)M8.Util.GetLoWord(dataPak);
            }

            public void _SetAsMap(uint dataPak) {
                ResetKeys();

                _ApplyInfo(dataPak);

                map = (InputKeyMap)M8.Util.GetLoWord(dataPak);
            }

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

            public bool IsDirty() {
                return mDirty;
            }
        }

        public class Bind {
            public int action = 0;
            public Control control = InputManager.Control.Button;

            public float deadZone = 0.1f;
            public bool forceRaw;

            public List<Key> keys = null;
        }

        public class PlayerData {
            public Info info;

            public bool down;

            public Key[] keys;

            public OnButton callback;

            public PlayerData(int action, List<Key> aKeys) {
                info.action = action;
                down = false;
                ApplyKeyList(aKeys);
            }

            public void ApplyKeyList(List<Key> aKeys) {
                keys = aKeys.ToArray();
            }
        }

        public class BindData {
            public Control control;
            public float deadZone;
            public bool forceRaw;

            public PlayerData[] players;

            public BindData(Bind bind) {
                control = bind.control;
                deadZone = bind.deadZone;
                forceRaw = bind.forceRaw;

                //construct player data, put in the keys
                ApplyKeys(bind);
            }

            public void ApplyKeys(Bind bind) {
                int numPlayers = 0;

                List<Key>[] playerKeys = new List<Key>[MaxPlayers];

                foreach(Key key in bind.keys) {
                    if(key.player + 1 > numPlayers)
                        numPlayers = key.player + 1;

                    if(playerKeys[key.player] == null)
                        playerKeys[key.player] = new List<Key>();

                    playerKeys[key.player].Add(key);
                }

                if(players == null)
                    players = new PlayerData[numPlayers];

                for(int i = 0; i < numPlayers; i++) {
                    if(playerKeys[i] != null) {
                        if(players[i] == null)
                            players[i] = new PlayerData(bind.action, playerKeys[i]);
                        else
                            players[i].ApplyKeyList(playerKeys[i]);
                    }
                }
            }
        }

        public TextAsset actionConfig;
        public TextAsset config;

        protected BindData[] mBinds;

        private const int buttonCallMax = 32;
        protected PlayerData[] mButtonCalls = new PlayerData[buttonCallMax];
        protected int mButtonCallsCount = 0;

        private struct ButtonCallSetData {
            public PlayerData pd;
            public OnButton cb;
            public bool add;
        }

        private string[] mActionNames;
        private ButtonCallSetData[] mButtonCallSetQueue = new ButtonCallSetData[buttonCallMax]; //prevent breaking enumeration during update when adding/removing
        private int mButtonCallSetQueueCount;

        //interfaces (available after awake)

        /// <summary>
        /// Number of input actions after loading
        /// </summary>
        public int actionCount { get { return mActionNames != null ? mActionNames.Length : 0; } }

        public int GetActionIndex(string actionName) {
            return System.Array.IndexOf(mActionNames, actionName);
        }

        public string GetActionName(int action) {
            return action >= 0 && action < mActionNames.Length ? mActionNames[action] : "";
        }

        public BindData GetBindData(int action) {
            return mBinds[action];
        }

        /// <summary>
        /// Reverts bind settings to loaded configuration
        /// </summary>
        public void RevertBinds() {
            fastJSON.JSON.Parameters.UseExtensions = false;
            List<Bind> keys = fastJSON.JSON.ToObject<List<Bind>>(config.text);

            foreach(Bind key in keys) {
                if(key != null && key.keys != null) {
                    if(mBinds[key.action] != null) {
                        BindData bindDat = mBinds[key.action];
                        bindDat.ApplyKeys(key);
                    }
                }
            }
        }

        public void UnBindKey(int player, int action, int index) {
            mBinds[action].players[player].keys[index].ResetKeys();
        }

        public void UnBindKey(int player, string action, int index) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) UnBindKey(player, actionInd, index);
        }

        public bool CheckBindKey(int player, int action, int index) {
            return mBinds[action] != null && mBinds[action].players[player] != null && mBinds[action].players[player].keys[index].isValid;
        }

        public bool CheckBindKey(int player, string action, int index) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return CheckBindKey(player, actionInd, index);
            return false;
        }

        public Key GetBindKey(int player, int action, int index) {
            return mBinds[action].players[player].keys[index];
        }

        public Key GetBindKey(int player, string action, int index) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return GetBindKey(player, actionInd, index);
            return null;
        }

        public bool CheckBind(int action) {
            return mBinds[action] != null;
        }

        public bool CheckBind(string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return CheckBind(actionInd);
            return false;
        }

        public Control GetControlType(int action) {
            return mBinds[action].control;
        }

        public Control GetControlType(string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return GetControlType(actionInd);
            return Control.Button;
        }

        public float GetAxis(int player, int action) {
            BindData bindData = mBinds[action];
            PlayerData pd = bindData.players[player];
            Key[] keys = pd.keys;

            pd.info.axis = 0.0f;

            foreach(Key key in keys) {
                if(key != null) {
                    float axis = ProcessAxis(key, bindData.deadZone, bindData.forceRaw);
                    if(axis != 0.0f) {
                        pd.info.axis = key.invert ? -axis : axis;
                        break;
                    }
                }
            }

            return pd.info.axis;
        }

        public float GetAxis(int player, string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return GetAxis(player, actionInd);
            return 0f;
        }

        public bool IsPressed(int player, string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return IsPressed(player, actionInd);
            return false;
        }

        public bool IsPressed(int player, int action) {
            if(action == ActionInvalid)
                return false;

            Key[] keys = mBinds[action].players[player].keys;

            foreach(Key key in keys) {
                if(key != null && ProcessButtonPressed(key)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsReleased(int player, string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return IsReleased(player, actionInd);
            return false;
        }

        public bool IsReleased(int player, int action) {
            if(action == ActionInvalid)
                return false;

            Key[] keys = mBinds[action].players[player].keys;

            foreach(Key key in keys) {
                if(key != null && ProcessButtonReleased(key)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsDown(int player, int action) {
            if(action == ActionInvalid)
                return false;

            Key[] keys = mBinds[action].players[player].keys;

            foreach(Key key in keys) {
                if(key != null && ProcessButtonDown(key)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsDown(int player, string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return IsDown(player, actionInd);
            return false;
        }

        public int GetIndex(int player, int action) {
            return mBinds[action].players[player].info.index;
        }

        public int GetIndex(int player, string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) return GetIndex(player, actionInd);
            return -1;
        }

        public void AddButtonCall(int player, int action, OnButton callback) {
            if(action < mBinds.Length && player < mBinds[action].players.Length) {
                PlayerData pd = mBinds[action].players[player];

                mButtonCallSetQueue[mButtonCallSetQueueCount] = new ButtonCallSetData() { pd = pd, cb = callback, add = true };
                mButtonCallSetQueueCount++;
            }
        }

        public void AddButtonCall(int player, string action, OnButton callback) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) AddButtonCall(player, actionInd, callback);
        }

        public void RemoveButtonCall(int player, int action, OnButton callback) {
            if(action < mBinds.Length && player < mBinds[action].players.Length) {
                PlayerData pd = mBinds[action].players[player];

                mButtonCallSetQueue[mButtonCallSetQueueCount] = new ButtonCallSetData() { pd = pd, cb = callback, add = false };
                mButtonCallSetQueueCount++;
            }
        }

        public void RemoveButtonCall(int player, string action, OnButton callback) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) RemoveButtonCall(player, actionInd, callback);
        }

        public void RemoveButtonCall(OnButton callback) {
            for(int i = 0; i < mBinds.Length; i++) {
                for(int j = 0; j < mBinds[i].players.Length; j++) {
                    PlayerData pd = mBinds[i].players[j];
                    if(pd.callback == callback) {
                        mButtonCallSetQueue[mButtonCallSetQueueCount] = new ButtonCallSetData() { pd = pd, cb = callback, add = false };
                        mButtonCallSetQueueCount++;
                    }
                }
            }
        }

        public void ClearButtonCall(int action) {
            foreach(PlayerData pd in mBinds[action].players) {
                pd.callback = null;

                mButtonCallSetQueue[mButtonCallSetQueueCount] = new ButtonCallSetData() { pd = pd, cb = null, add = false };
                mButtonCallSetQueueCount++;
            }
        }

        public void ClearButtonCall(string action) {
            int actionInd = GetActionIndex(action);
            if(actionInd != -1) ClearButtonCall(actionInd);
        }

        public void ClearAllButtonCalls() {
            foreach(BindData bd in mBinds) {
                if(bd != null && bd.players != null) {
                    foreach(PlayerData pd in bd.players) {
                        pd.callback = null;
                    }
                }
            }

            mButtonCallsCount = 0;

            mButtonCallSetQueueCount = 0;
        }

        //implements

        protected virtual float ProcessAxis(Key key, float deadZone, bool forceRaw) {
            if(key.input.Length > 0) {
                if(Time.timeScale == 0.0f || forceRaw) {
                    float val = Input.GetAxisRaw(key.input);
                    return Mathf.Abs(val) > deadZone ? val : 0.0f;
                }
                else {
                    return Input.GetAxis(key.input);
                }
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

        /// <summary>
        /// Used by IsPressed
        /// </summary>
        protected virtual bool ProcessButtonPressed(Key key) {
            return
                key.input.Length > 0 ? Input.GetButtonDown(key.input) :
            key.code != KeyCode.None ? Input.GetKeyDown(key.code) :
            false;
        }

        /// <summary>
        /// Used by IsReleased
        /// </summary>
        protected virtual bool ProcessButtonReleased(Key key) {
            return
                key.input.Length > 0 ? Input.GetButtonUp(key.input) :
            key.code != KeyCode.None ? Input.GetKeyUp(key.code) :
            false;
        }

        //internal

        protected override void OnDestroy() {
            ClearAllButtonCalls();

            base.OnDestroy();
        }

        protected virtual void Awake() {
            fastJSON.JSON.Parameters.UseExtensions = false;

            if(actionConfig != null) {
                List<string> actionLoad = fastJSON.JSON.ToObject<List<string>>(actionConfig.text);
                mActionNames = actionLoad != null ? actionLoad.ToArray() : new string[0];
            }
            else
                Debug.LogWarning("No action config loaded.  There will be no bindings!");

            if(config != null && mActionNames != null) {
                Dictionary<int, BindData> binds = new Dictionary<int, BindData>();

                List<Bind> bindCfg = fastJSON.JSON.ToObject<List<Bind>>(config.text);

                foreach(Bind key in bindCfg) {
                    if(key != null && key.keys != null)
                        binds.Add(key.action, new BindData(key));
                }

                //set bindings
                mBinds = new BindData[actionCount];
                foreach(KeyValuePair<int, BindData> pair in binds) {
                    mBinds[pair.Key] = pair.Value;
                }

                //register input actions to localizer
                for(int i = 0; i < actionCount; i++)
                    Localize.instance.RegisterParam("input_"+i, OnTextParam);
            }
            else {
                mBinds = new BindData[0];
            }
        }

        string OnTextParam(string key) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            int action = GetActionIndex(key);
            if(action != -1) {
                //NOTE: assumes player 0
                PlayerData pd = mBinds[action].players[0];

                for(int i = 0; i < pd.keys.Length; i++) {
                    string keyString = pd.keys[i].GetKeyString();
                    if(!string.IsNullOrEmpty(keyString)) {
                        sb.Append(keyString);

                        if(pd.keys.Length > 1 && i < pd.keys.Length - 1) {
                            sb.Append(", ");
                        }
                    }
                }
            }

            return sb.ToString();
        }

        protected virtual void Update() {
            for(int i = 0; i < mButtonCallsCount; i++) {
                PlayerData pd = mButtonCalls[i];

                pd.info.state = State.None;

                Key keyDown = null;

                for(int k = 0; k < pd.keys.Length; k++) {
                    Key key = pd.keys[k];
                    if(ProcessButtonDown(key)) {
                        keyDown = key;
                        break;
                    }
                }

                if(keyDown != null) {
                    if(!pd.down) {
                        pd.down = true;

                        pd.info.axis = keyDown.GetAxisValue();
                        pd.info.state = State.Pressed;
                        pd.info.index = keyDown.index;

                        pd.callback(pd.info);
                    }
                }
                else {
                    if(pd.down) {
                        pd.down = false;

                        pd.info.axis = 0.0f;
                        pd.info.state = State.Released;

                        pd.callback(pd.info);
                    }
                }
            }

            //add or remove button calls
            for(int i = 0; i < mButtonCallSetQueueCount; i++) {
                ButtonCallSetData dat = mButtonCallSetQueue[i];

                if(dat.add) {
                    if(dat.cb != null) {
                        if(dat.pd.callback != dat.cb) {
                            dat.pd.callback += dat.cb;
                        }

                        int ind = System.Array.IndexOf(mButtonCalls, dat.pd, 0, mButtonCallsCount);
                        if(ind == -1) {
                            mButtonCalls[mButtonCallsCount] = dat.pd;
                            mButtonCallsCount++;
                        }
                    }
                }
                else {
                    if(dat.cb != null)
                        dat.pd.callback -= dat.cb;
                    else
                        dat.pd.callback = null;

                    //no more callbacks, don't need to poll this anymore
                    if(dat.pd.callback == null) {
                        if(mButtonCallsCount > 1) {
                            int ind = System.Array.IndexOf(mButtonCalls, dat.pd, 0, mButtonCallsCount);
                            mButtonCalls[ind] = mButtonCalls[mButtonCallsCount - 1];

                            mButtonCallsCount--;
                        }
                        else
                            mButtonCallsCount = 0;
                    }
                }
            }

            mButtonCallSetQueueCount = 0;
        }
    }
}
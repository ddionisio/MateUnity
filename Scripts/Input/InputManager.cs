using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//generalized input handling, useful for porting to non-unity conventions (e.g. Ouya)
[AddComponentMenu("M8/Core/InputManager")]
public class InputManager : UserSetting {
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

    public TextAsset actionConfig;
    public TextAsset config;

    protected class PlayerData {
        public Info info;

        public bool down;

        public Key[] keys;

        public OnButton callback;

        public PlayerData(List<Key> aKeys) {
            down = false;
            ApplyKeyList(aKeys);
        }

        public void ApplyKeyList(List<Key> aKeys) {
            keys = aKeys.ToArray();
        }
    }

    protected class BindData {
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
                        players[i] = new PlayerData(playerKeys[i]);
                    else
                        players[i].ApplyKeyList(playerKeys[i]);
                }
            }
        }
    }
        
    protected BindData[] mBinds;

    private const int buttonCallMax = 32;
    protected PlayerData[] mButtonCalls = new PlayerData[buttonCallMax];
    protected int mButtonCallsCount = 0;

    private struct ButtonCallSetData {
        public PlayerData pd;
        public OnButton cb;
        public bool add;
    }

    private static InputManager mInstance;

    private string[] mActionNames;
    private ButtonCallSetData[] mButtonCallSetQueue = new ButtonCallSetData[buttonCallMax]; //prevent breaking enumeration during update when adding/removing
    private int mButtonCallSetQueueCount;

    //interfaces (available after awake)

    public static InputManager instance { get { return mInstance; } }

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

    /// <summary>
    /// Call this to reload binds from config and prefs.  This is to cancel editing key binds.
    /// If deletePrefs = true, then remove custom binds from prefs.
    /// </summary>
    public void RevertBinds(bool deletePrefs) {
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

        if(deletePrefs) {
            for(int act = 0; act < mBinds.Length; act++) {
                BindData bindDat = mBinds[act];
                if(bindDat != null) {
                    for(int player = 0; player < bindDat.players.Length; player++) {
                        PlayerData pd = bindDat.players[player];
                        if(pd != null) {
                            for(int index = 0; index < pd.keys.Length; index++) {
                                string usdKey = _BaseKey(act, player, index);
                                _DeletePlayerPrefs(usdKey);
                            }
                        }
                    }
                }
            }
        }
        else {
            LoadBinds();
        }
    }

    private void LoadBinds() {
        for(int act = 0; act < mBinds.Length; act++) {
            BindData bindDat = mBinds[act];
            if(bindDat != null) {
                for(int player = 0; player < bindDat.players.Length; player++) {
                    PlayerData pd = bindDat.players[player];
                    if(pd != null) {
                        for(int index = 0; index < pd.keys.Length; index++) {
                            string usdKey = _BaseKey(act, player, index);

                            if(userData.HasKey(usdKey + "_i")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new Key();

                                pd.keys[index].SetAsInput(userData.GetString(usdKey + "_i"));
                            }
                            else if(userData.HasKey(usdKey + "_k")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new Key();

                                pd.keys[index]._SetAsKey((uint)userData.GetInt(usdKey + "_k"));
                            }
                            else if(userData.HasKey(usdKey + "_m")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new Key();

                                pd.keys[index]._SetAsMap((uint)userData.GetInt(usdKey + "_m"));
                            }
                            else if(userData.HasKey(usdKey + "_d"))
                                pd.keys[index].ResetKeys();
                        }
                    }
                }
            }
        }
    }

    string _BaseKey(int action, int player, int key) {
        return string.Format("bind_{0}_{1}_{2}", action, player, key);
    }

    void _DeletePlayerPrefs(string baseKey) {
        userData.Delete(baseKey + "_i");
        userData.Delete(baseKey + "_k");
        userData.Delete(baseKey + "_m");
        userData.Delete(baseKey + "_d");
    }

    /// <summary>
    /// Call this once you are done modifying key binds
    /// </summary>
    public void SaveBinds() {
        for(int act = 0; act < mBinds.Length; act++) {
            BindData bindDat = mBinds[act];
            if(bindDat != null) {
                for(int player = 0; player < bindDat.players.Length; player++) {
                    PlayerData pd = bindDat.players[player];
                    if(pd != null) {
                        for(int index = 0; index < pd.keys.Length; index++) {
                            string usdKey = _BaseKey(act, player, index);

                            Key key = pd.keys[index];
                            if(key.isValid) {
                                if(key.IsDirty()) {
                                    //for previous bind if type is changed
                                    _DeletePlayerPrefs(usdKey);

                                    if(!string.IsNullOrEmpty(key.input)) {
                                        userData.SetString(usdKey + "_k", key.input);
                                    }
                                    else {
                                        //pack data
                                        ushort code = 0;
                                        string postfix;

                                        if(key.code != KeyCode.None) {
                                            code = (ushort)key.code;
                                            postfix = "_k";
                                        }
                                        else if(key.map != InputKeyMap.None) {
                                            code = (ushort)key.map;
                                            postfix = "_m";
                                        }
                                        else
                                            postfix = null;

                                        if(postfix != null) {
                                            int val = (int)M8.Util.MakeLong(M8.Util.MakeWord((byte)key.axis, (byte)key.index), code);

                                            userData.SetInt(usdKey + postfix, val);
                                        }
                                    }

                                    key.SetDirty(false);
                                }
                            }
                            else {
                                _DeletePlayerPrefs(usdKey);
                                userData.SetString(usdKey + "_d", "-");
                            }
                        }
                    }
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

    public State GetState(int player, int action) {
        return mBinds[action].players[player].info.state;
    }

    public State GetState(int player, string action) {
        int actionInd = GetActionIndex(action);
        if(actionInd != -1) return GetState(player, actionInd);
        return State.None;
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

    //internal

    protected virtual void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;

            ClearAllButtonCalls();
        }
    }

    protected override void Awake() {
        base.Awake();

        if(mInstance != null)
            return;
        else
            mInstance = this;

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
                GameLocalize.instance.RegisterParam("input_"+i, OnTextParam);
                        
            //load user config binds
            LoadBinds();
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

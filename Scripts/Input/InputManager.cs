using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//generalized input handling, useful for porting to non-unity conventions (e.g. Ouya)
[AddComponentMenu("M8/Core/InputManager")]
public class InputManager : MonoBehaviour {
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
        Minus
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

        public ButtonAxis axis = ButtonAxis.None; //for buttons as axis
        public int index = 0; //which index this key refers to

        private bool mNewBind = false;

        public void SetAsInput(string input) {
            ResetKeys();

            this.input = input;
        }

        void _ApplyInfo(uint dataPak) {
            ushort s1 = M8.Util.GetHiWord(dataPak);

            axis = (ButtonAxis)M8.Util.GetHiByte(s1);
            index = M8.Util.GetLoByte(s1);
        }

        public void SetAsKey(uint dataPak) {
            ResetKeys();

            _ApplyInfo(dataPak);

            code = (KeyCode)M8.Util.GetLoWord(dataPak);
        }

        public void SetAsMap(uint dataPak) {
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

        public void _NewBind() {
            mNewBind = true;
        }

        public bool _IsNewBind() {
            return mNewBind;
        }
    }

    public class Bind {
        public int action = 0;
        public Control control = InputManager.Control.Button;

        public float deadZone = 0.1f;

        public List<Key> keys = null;
    }

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

        public PlayerData[] players;

        public BindData(Bind bind) {
            control = bind.control;
            deadZone = bind.deadZone;

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

    protected HashSet<PlayerData> mButtonCalls;

    private struct ButtonCallSetData {
        public PlayerData pd;
        public OnButton cb;
        public bool add;
    }

    private Queue<ButtonCallSetData> mButtonCallSetQueue = new Queue<ButtonCallSetData>(64); //prevent breaking enumeration during update when adding/removing

    //interfaces (available after awake)

    /// <summary>
    /// Call this to reload binds from config and prefs.  This is to cancel editing key binds.
    /// If deletePrefs = true, then remove custom binds from prefs.
    /// </summary>
    public void RevertBinds(bool deletePrefs) {
        fastJSON.JSON.Instance.Parameters.UseExtensions = false;
        List<Bind> keys = fastJSON.JSON.Instance.ToObject<List<Bind>>(config.text);

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

                            if(PlayerPrefs.HasKey(usdKey + "_i")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new Key();

                                pd.keys[index].SetAsInput(PlayerPrefs.GetString(usdKey + "_i"));
                            }
                            else if(PlayerPrefs.HasKey(usdKey + "_k")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new Key();

                                pd.keys[index].SetAsKey((uint)PlayerPrefs.GetInt(usdKey + "_k"));
                            }
                            else if(PlayerPrefs.HasKey(usdKey + "_m")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new Key();

                                pd.keys[index].SetAsMap((uint)PlayerPrefs.GetInt(usdKey + "_m"));
                            }
                            else if(PlayerPrefs.HasKey(usdKey + "_d"))
                                pd.keys[index] = null; //destroy
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
        PlayerPrefs.DeleteKey(baseKey + "_i");
        PlayerPrefs.DeleteKey(baseKey + "_k");
        PlayerPrefs.DeleteKey(baseKey + "_m");
        PlayerPrefs.DeleteKey(baseKey + "_d");
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
                            if(key != null) {
                                if(key._IsNewBind()) {
                                    //for previous bind if type is changed
                                    _DeletePlayerPrefs(usdKey);

                                    if(!string.IsNullOrEmpty(key.input)) {
                                        PlayerPrefs.SetString(usdKey + "_k", key.input);
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

                                            PlayerPrefs.SetInt(usdKey + postfix, val);
                                        }
                                    }
                                }
                            }
                            else {
                                _DeletePlayerPrefs(usdKey);
                                PlayerPrefs.SetString(usdKey + "_d", "-");
                            }
                        }
                    }
                }
            }
        }
    }

    public void UnBindKey(int player, int action, int index) {
        mBinds[action].players[player].keys[index] = null;
    }

    public void BindKey(int action, int index, Key newKey) {
        newKey._NewBind();
        mBinds[action].players[newKey.player].keys[index] = newKey;
    }

    public bool CheckBindKey(int player, int action, int index) {
        return mBinds[action] != null && mBinds[action].players[player] != null && mBinds[action].players[player].keys[index] != null;
    }

    public Key GetBindKey(int player, int action, int index) {
        return mBinds[action].players[player].keys[index];
    }

    public bool CheckBind(int action) {
        return mBinds[action] != null;
    }

    public Control GetControlType(int action) {
        return mBinds[action].control;
    }

    public float GetAxis(int player, int action) {
        BindData bindData = mBinds[action];
        PlayerData pd = bindData.players[player];
        Key[] keys = pd.keys;

        pd.info.axis = 0.0f;

        foreach(Key key in keys) {
            if(key != null) {
                float axis = ProcessAxis(key, bindData.deadZone);
                if(axis != 0.0f) {
                    pd.info.axis = axis;
                    break;
                }
            }
        }

        return pd.info.axis;
    }

    public State GetState(int player, int action) {
        return mBinds[action].players[player].info.state;
    }

    public bool IsDown(int player, int action) {
        Key[] keys = mBinds[action].players[player].keys;

        foreach(Key key in keys) {
            if(key != null && ProcessButtonDown(key)) {
                return true;
            }
        }

        return false;
    }

    public int GetIndex(int player, int action) {
        return mBinds[action].players[player].info.index;
    }

    public void AddButtonCall(int player, int action, OnButton callback) {
        PlayerData pd = mBinds[action].players[player];

        mButtonCallSetQueue.Enqueue(new ButtonCallSetData() { pd = pd, cb = callback, add = true });
    }

    public void RemoveButtonCall(int player, int action, OnButton callback) {
        PlayerData pd = mBinds[action].players[player];

        mButtonCallSetQueue.Enqueue(new ButtonCallSetData() { pd = pd, cb = callback, add = false });
    }

    public void ClearButtonCall(int action) {
        foreach(PlayerData pd in mBinds[action].players) {
            pd.callback = null;

            mButtonCallSetQueue.Enqueue(new ButtonCallSetData() { pd = pd, cb = null, add = false });
        }
    }

    public void ClearAllButtonCalls() {
        foreach(BindData bd in mBinds) {
            if(bd != null && bd.players != null) {
                foreach(PlayerData pd in bd.players) {
                    pd.callback = null;
                }
            }
        }

        mButtonCalls.Clear();

        mButtonCallSetQueue.Clear();
    }

    //implements

    protected virtual float ProcessAxis(Key key, float deadZone) {
        if(key.input.Length > 0) {
            if(Time.timeScale == 0.0f) {
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
        ClearAllButtonCalls();
    }

    protected virtual void Awake() {
        if(config != null) {
            Dictionary<int, BindData> binds = new Dictionary<int, BindData>();

            fastJSON.JSON.Instance.Parameters.UseExtensions = false;
            List<Bind> keys = fastJSON.JSON.Instance.ToObject<List<Bind>>(config.text);

            int highestActionInd = 0;

            foreach(Bind key in keys) {
                if(key != null && key.keys != null) {
                    binds.Add(key.action, new BindData(key));

                    if(key.action > highestActionInd)
                        highestActionInd = key.action;
                }
            }

            mBinds = new BindData[highestActionInd + 1];
            foreach(KeyValuePair<int, BindData> pair in binds) {
                mBinds[pair.Key] = pair.Value;
            }

            mButtonCalls = new HashSet<PlayerData>();

            //load user config binds
            LoadBinds();
        }
        else {
            mBinds = new BindData[0];
        }
    }

    protected virtual void Update() {
        foreach(PlayerData pd in mButtonCalls) {
            pd.info.state = State.None;

            Key keyDown = null;

            foreach(Key key in pd.keys) {
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

        //add new button calls
        while(mButtonCallSetQueue.Count > 0) {
            ButtonCallSetData dat = mButtonCallSetQueue.Dequeue();
            if(dat.cb != null) {
                if(dat.add) {
                    if(!mButtonCalls.Contains(dat.pd)) {
                        dat.pd.callback += dat.cb;

                        mButtonCalls.Add(dat.pd);
                    }
                }
                else {
                    if(mButtonCalls.Contains(dat.pd)) {
                        dat.pd.callback -= dat.cb;

                        if(dat.pd.callback == null)
                            mButtonCalls.Remove(dat.pd);
                    }
                }
            }
        }
    }
}

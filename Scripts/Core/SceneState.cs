using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Put this in the core object, or on an object with DontDestroyOnLoad
[AddComponentMenu("M8/Core/SceneState")]
public class SceneState : MonoBehaviour {
    public const string GlobalDataFormat = "global_{0}";
    public const string DataFormat = "{0}_{1}";

    public delegate void StateCallback(bool isGlobal, string name, StateValue val);
        
    [System.Serializable]
    public class InitData {
        public string name = "";
        public int ival = 0;
        public float fval = 0.0f;
        public bool persistent = false;
    }

    [System.Serializable]
    public class InitSceneData {
        public string scene = "";
        public InitData[] data = null;

        public bool editFoldout = true; //just for editing
    }

    public struct StateValue {
        public int ival;
        public float fval;

        public StateValue(int aVal, float aFval) {
            ival = aVal;
            fval = aFval;
        }

        public void Apply(UserData ud, string key) {
            ival = ud.GetInt(key, ival);
            fval = ud.GetFloat(key, fval);
        }
    }

    public InitData[] globalStartData;

    public InitSceneData[] startData; //use for debug

    public event StateCallback onValueChange;
        
    private static SceneState mInstance = null;

    private Dictionary<string, InitData[]> mStartData;

    private string mScene = null; //scene associated with current flags
    private Dictionary<string, StateValue> mStates = null;

    private Dictionary<string, StateValue> mGlobalStates = new Dictionary<string, StateValue>();

    //only buffer one level previous for now
    private string mPrevScene = null;
    private Dictionary<string, StateValue> mPrevStates = null;

    public static SceneState instance { get { return mInstance; } }

    public static StateValue zeroValueData { get { return new StateValue(0, 0.0f); } }

    public Dictionary<string, StateValue> globalStates { get { return mGlobalStates; } }

    public Dictionary<string, StateValue> states { get { return mStates; } }

    public int GetValue(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        StateValue v = zeroValueData;
        //try local
        if(!mStates.TryGetValue(name, out v)) {
            //try user data
            v.Apply(UserData.instance, string.Format(DataFormat, mScene, name));
            mStates.Add(name, v);
        }

        return v.ival;
    }

    public void SetValue(string name, int val, bool persistent) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        bool isValueSet = false;
        StateValue curVal = zeroValueData;
        if(mStates.TryGetValue(name, out curVal)) {
            isValueSet = curVal.ival != val;

            if(isValueSet) {
                curVal.ival = val;
                mStates[name] = curVal;
            }
        }
        else {
            isValueSet = true;
            curVal.ival = val;
            mStates.Add(name, curVal);
        }

        if(isValueSet) {
            if(persistent) {
                string key = string.Format(DataFormat, mScene, name);
                UserData.instance.SetInt(key, val);
            }

            if(onValueChange != null) {
                onValueChange(false, name, curVal);
            }
        }
    }

    public bool CheckFlag(string name, int bit) {
        return CheckFlagMask(name, 1 << bit);
    }

    public bool CheckFlagMask(string name, int mask) {
        int flags = GetValue(name);

        return (flags & mask) != 0;
    }

    public void SetFlag(string name, int bit, bool state, bool persistent) {
        int flags = GetValue(name);

        if(state)
            flags |= 1 << bit;
        else
            flags &= ~(1 << bit);

        SetValue(name, flags, persistent);
    }

    public float GetValueFloat(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        StateValue v = zeroValueData;
        //try local
        if(!mStates.TryGetValue(name, out v)) {
            //try user data
            v.Apply(UserData.instance, string.Format(DataFormat, mScene, name));
            mStates.Add(name, v);
        }

        return v.fval;
    }

    public void SetValueFloat(string name, float val, bool persistent) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        bool isValueSet = false;
        StateValue curVal = zeroValueData;
        if(mStates.TryGetValue(name, out curVal)) {
            isValueSet = curVal.fval != val;

            if(isValueSet) {
                curVal.fval = val;
                mStates[name] = curVal;
            }
        }
        else {
            isValueSet = true;
            curVal.fval = val;
            mStates.Add(name, curVal);
        }

        if(isValueSet) {
            if(persistent) {
                string key = string.Format(DataFormat, mScene, name);
                UserData.instance.SetFloat(key, val);
            }

            if(onValueChange != null) {
                onValueChange(false, name, curVal);
            }
        }
    }

    public int GetGlobalValue(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        StateValue v = zeroValueData;
        //try local
        if(!mGlobalStates.TryGetValue(name, out v)) {
            //try user data
            v.Apply(UserData.instance, string.Format(GlobalDataFormat, name));
            mGlobalStates.Add(name, v);
        }

        return v.ival;
    }

    public void SetGlobalValue(string name, int val, bool persistent) {
        bool isValueSet = false;
        StateValue curVal = zeroValueData;
        if(mGlobalStates.TryGetValue(name, out curVal)) {
            isValueSet = curVal.ival != val;

            if(isValueSet) {
                curVal.ival = val;
                mGlobalStates[name] = curVal;
            }
        }
        else {
            isValueSet = true;
            curVal.ival = val;
            mGlobalStates.Add(name, curVal);
        }

        if(isValueSet) {
            if(persistent) {
                string key = string.Format(GlobalDataFormat, name);
                UserData.instance.SetInt(key, val);
            }

            if(onValueChange != null) {
                onValueChange(true, name, curVal);
            }
        }
    }

    public bool CheckGlobalFlag(string name, int bit) {
        return CheckGlobalFlagMask(name, 1 << bit);
    }

    public bool CheckGlobalFlagMask(string name, int mask) {
        int flags = GetGlobalValue(name);

        return (flags & mask) == mask;
    }

    public void SetGlobalFlag(string name, int bit, bool state, bool persistent) {
        int flags = GetGlobalValue(name);

        if(state)
            flags |= 1 << bit;
        else
            flags &= ~(1 << bit);

        SetGlobalValue(name, flags, persistent);
    }

    public float GetGlobalValueFloat(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        StateValue v = zeroValueData;
        //try local
        if(!mGlobalStates.TryGetValue(name, out v)) {
            //try user data
            v.Apply(UserData.instance, string.Format(GlobalDataFormat, name));
            mGlobalStates.Add(name, v);
        }

        return v.fval;
    }

    public void SetGlobalValueFloat(string name, float val, bool persistent) {
        bool isValueSet = false;
        StateValue curVal = zeroValueData;
        if(mGlobalStates.TryGetValue(name, out curVal)) {
            isValueSet = curVal.fval != val;

            if(isValueSet) {
                curVal.fval = val;
                mGlobalStates[name] = curVal;
            }
        }
        else {
            isValueSet = true;
            curVal.fval = val;
            mGlobalStates.Add(name, curVal);
        }

        if(isValueSet) {
            if(persistent) {
                string key = string.Format(GlobalDataFormat, name);
                UserData.instance.SetFloat(key, val);
            }

            if(onValueChange != null) {
                onValueChange(true, name, curVal);
            }
        }
    }

    public void ResetGlobalValues() {
        mGlobalStates.Clear();

        AppendGlobalInitData();
    }

    public void ResetValues() {
        mStates = null;
        mPrevStates = null;
        mPrevScene = null;

        AppendInitData();
    }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;
        }
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            mStartData = new Dictionary<string, InitData[]>(startData.Length);
            foreach(InitSceneData sdat in startData) {
                if(!string.IsNullOrEmpty(sdat.scene) && sdat.data != null)
                    mStartData.Add(sdat.scene, sdat.data);
            }
        }
    }

    void Start() {
        AppendGlobalInitData();

        if(mStates == null && mScene == null) {
            mScene = Application.loadedLevelName;
            mStates = new Dictionary<string, StateValue>();
            AppendInitData();
        }
    }

    void SceneChange(string toScene) {
        //new scene is being loaded, save current state to previous
        //revert flags if new scene is previous
        if(toScene != mScene) {
            string curScene = mScene;
            Dictionary<string, StateValue> curStates = mStates;

            mScene = toScene;

            if(toScene == mPrevScene) {
                mStates = mPrevStates;
            }
            else {
                mStates = new Dictionary<string, StateValue>();
                AppendInitData();
            }
                        
            mPrevScene = curScene;
            mPrevStates = curStates;
        }
    }

    void OnUserDataLoad(UserData ud) {
        //update global states
        if(mGlobalStates != null && mGlobalStates.Count > 0) {
            string[] keys = new string[mGlobalStates.Count];
            mGlobalStates.Keys.CopyTo(keys, 0);

            foreach(string key in keys) {
                StateValue val = mGlobalStates[key];
                val.Apply(ud, string.Format(GlobalDataFormat, key));
                mGlobalStates[key] = val;
            }
        }

        //update states
        if(mStates != null && mStates.Count > 0 && !string.IsNullOrEmpty(mScene)) {
            string[] keys = new string[mStates.Count];
            mStates.Keys.CopyTo(keys, 0);

            foreach(string key in keys) {
                StateValue val = mStates[key];
                val.Apply(ud, string.Format(DataFormat, mScene, key));
                mStates[key] = val;
            }
        }
    }

    void AppendGlobalInitData() {
        foreach(InitData dat in globalStartData) {
            if(!string.IsNullOrEmpty(dat.name)) {
                //check if data exists in user first
                string key = string.Format(GlobalDataFormat, dat.name);

                StateValue s = new StateValue(dat.ival, dat.fval);
                s.Apply(UserData.instance, key);

                if(mGlobalStates.ContainsKey(dat.name))
                    mGlobalStates[dat.name] = s;
                else
                    mGlobalStates.Add(dat.name, s);

                if(onValueChange != null) {
                    onValueChange(true, dat.name, s);
                }
            }
        }
    }

    void AppendInitData() {
        InitData[] dats;
        if(mStartData.TryGetValue(mScene, out dats)) {
            foreach(InitData dat in dats) {
                if(!string.IsNullOrEmpty(dat.name)) {
                    //check if data exists in user first
                    string key = string.Format(DataFormat, mScene, dat.name);

                    StateValue s = new StateValue(dat.ival, dat.fval);
                    s.Apply(UserData.instance, key);

                    if(mStates.ContainsKey(dat.name))
                        mStates[dat.name] = s;
                    else
                        mStates.Add(dat.name, s);

                    if(onValueChange != null) {
                        onValueChange(false, dat.name, s);
                    }
                }
            }
        }
    }
}

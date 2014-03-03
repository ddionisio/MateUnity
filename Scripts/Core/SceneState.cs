using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Put this in the core object, or on an object with DontDestroyOnLoad
[AddComponentMenu("M8/Core/SceneState")]
public class SceneState : MonoBehaviour {
    public const string GlobalDataPrefix = "g:";
    public const string GlobalDataFormat = "g:{0}";
    public const string DataFormat = "{0}:{1}";

    public delegate void StateCallback(bool isGlobal, string name, StateValue val);

    [System.Serializable]
    public class InitData {
        public string name = "";
        public int ival = 0;
        public float fval = 0.0f;
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

    public bool HasValue(string name) {
        if(!mStates.ContainsKey(name)) {
            //try user data
            return UserData.instance.HasKey(string.Format(DataFormat, mScene, name));
        }

        return false;
    }

    public void DeleteValuesByNameContain(string nameContains) {
        foreach(string key in new List<string>(mStates.Keys)) {
            if(key.Contains(nameContains)) {
                mStates.Remove(key);
            }
        }

        foreach(string key in new List<string>(mGlobalStates.Keys)) {
            if(key.Contains(nameContains)) {
                mGlobalStates.Remove(key);
            }
        }

        UserData.instance.DeleteAllByNameContain(nameContains);//string.Format(DataFormat, mScene, nameContains));
    }

    public void DeleteValue(string name, bool persistent) {
        mStates.Remove(name);

        if(persistent)
            UserData.instance.Delete(string.Format(DataFormat, mScene, name));
    }

    public int GetValue(string name, int defaultVal = 0) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        StateValue v;
        //try local
        if(!mStates.TryGetValue(name, out v)) {
            //try user data
            string key = string.Format(DataFormat, mScene, name);
            if(UserData.instance.HasKey(key)) {
                v.Apply(UserData.instance, key);
                mStates.Add(name, v);
                return v.ival;
            }
        }
        else
            return v.ival;

        return defaultVal;
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

    public float GetValueFloat(string name, float defaultVal = 0.0f) {
        if(mStates == null) {
            mStates = new Dictionary<string, StateValue>();
        }

        StateValue v;
        //try local
        if(!mStates.TryGetValue(name, out v)) {
            //try user data
            string key = string.Format(DataFormat, mScene, name);
            if(UserData.instance.HasKey(key)) {
                v.Apply(UserData.instance, key);
                mStates.Add(name, v);
                return v.fval;
            }
        }
        else
            return v.fval;

        return defaultVal;
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

    public string[] GetGlobalKeys(System.Predicate<KeyValuePair<string, StateValue>> predicate) {
        List<string> items = new List<string>(mGlobalStates.Count);
        foreach(KeyValuePair<string, StateValue> pair in mGlobalStates) {
            if(predicate(pair))
                items.Add(pair.Key);
        }

        return items.ToArray();
    }

    public bool HasGlobalValue(string name) {
        if(!mGlobalStates.ContainsKey(name)) {
            //try user data
            return UserData.instance.HasKey(string.Format(GlobalDataFormat, name));
        }

        return false;
    }

    public void DeleteGlobalValue(string name, bool persistent) {
        mGlobalStates.Remove(name);

        if(persistent)
            UserData.instance.Delete(string.Format(GlobalDataFormat, name));
    }

    public int GetGlobalValue(string name, int defaultVal = 0) {
        if(mGlobalStates == null) {
            mGlobalStates = new Dictionary<string, StateValue>();
        }

        StateValue v;
        //try local
        if(!mGlobalStates.TryGetValue(name, out v)) {
            //try user data
            string key = string.Format(GlobalDataFormat, name);
            if(UserData.instance.HasKey(key)) {
                v.Apply(UserData.instance, key);
                mGlobalStates.Add(name, v);
                return v.ival;
            }
        }
        else
            return v.ival;

        return defaultVal;
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

    public float GetGlobalValueFloat(string name, float defaultVal = 0.0f) {
        if(mGlobalStates == null) {
            mGlobalStates = new Dictionary<string, StateValue>();
        }

        StateValue v;
        //try local
        if(!mGlobalStates.TryGetValue(name, out v)) {
            //try user data
            string key = string.Format(GlobalDataFormat, name);
            if(UserData.instance.HasKey(key)) {
                v.Apply(UserData.instance, key);
                mGlobalStates.Add(name, v);
                return v.fval;
            }
        }
        else
            return v.fval;

        return defaultVal;
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

    public void ClearAllSavedData(bool resetValues = true) {
        if(mGlobalStates != null) {
            UserData.instance.DeleteAllByNameContain(GlobalDataPrefix);

            if(resetValues)
                ResetGlobalValues();
        }

        if(mStates != null && !string.IsNullOrEmpty(mScene)) {
            UserData.instance.DeleteAllByNameContain(mScene + ":");

            if(resetValues)
                ResetValues();
        }
    }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;
        }
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            UserData.instance.actCallback += OnUserDataAction;

            mStartData = new Dictionary<string, InitData[]>(startData.Length);
            foreach(InitSceneData sdat in startData) {
                if(!string.IsNullOrEmpty(sdat.scene) && sdat.data != null)
                    mStartData.Add(sdat.scene, sdat.data);
            }

            AppendGlobalInitData();

            if(mStates == null && mScene == null) {
                mScene = Application.loadedLevelName;
                mStates = new Dictionary<string, StateValue>();
                AppendInitData();
            }
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

    void OnUserDataAction(UserData ud, UserData.Action act) {
        switch(act) {
            case UserData.Action.Load:
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
                break;
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

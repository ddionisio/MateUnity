using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Put this in the core object, or on an object with DontDestroyOnLoad
[AddComponentMenu("M8/Core/SceneState")]
public class SceneState : MonoBehaviour {
    public const string GlobalDataFormat = "global_{0}";
    public const string DataFormat = "{0}_{1}";

    public delegate void StateCallback(bool isGlobal, string name, int val);
        
    [System.Serializable]
    public class InitData {
        public string name = "";
        public int val = 0;
        public bool persistent = false;
    }

    [System.Serializable]
    public class InitSceneData {
        public string scene = "";
        public InitData[] data = null;

        public bool editFoldout = true; //just for editing
    }

    public InitData[] globalStartData;

    public InitSceneData[] startData; //use for debug

    public event StateCallback onValueChange;

    private static SceneState mInstance = null;

    private Dictionary<string, InitData[]> mStartData;

    private string mScene = null; //scene associated with current flags
    private Dictionary<string, int> mStates = null;

    private Dictionary<string, int> mGlobalStates = new Dictionary<string,int>();

    //only buffer one level previous for now
    private string mPrevScene = null;
    private Dictionary<string, int> mPrevStates = null;

    public static SceneState instance { get { return mInstance; } }

    public Dictionary<string, int> globalStates { get { return mGlobalStates; } }

    public Dictionary<string, int> states { get { return mStates; } }

    public int GetValue(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, int>();
        }

        int v = 0;
        //try local
        if(!mStates.TryGetValue(name, out v)) {
            //try user data
            string key = string.Format(DataFormat, mScene, name);
            v = UserData.instance.GetInt(key, 0);
            mStates.Add(name, v);
        }

        return v;
    }

    public void SetValue(string name, int val, bool persistent) {
        if(mStates == null) {
            mStates = new Dictionary<string, int>();
        }

        bool isValueSet = false;
        int curVal = 0;
        if(mStates.TryGetValue(name, out curVal)) {
            isValueSet = curVal != val;

            if(isValueSet)
                mStates[name] = val;
        }
        else {
            isValueSet = true;

            mStates.Add(name, val);
        }

        if(isValueSet) {
            if(persistent) {
                string key = string.Format(DataFormat, mScene, name);
                UserData.instance.SetInt(key, val);
            }

            if(onValueChange != null) {
                onValueChange(false, name, val);
            }
        }
    }

    public bool CheckFlag(string name, int bit) {
        int flags = GetValue(name);

        return (flags & (1<<bit)) != 0;
    }

    public void SetFlag(string name, int bit, bool state, bool persistent) {
        int flags = GetValue(name);

        if(state)
            flags |= 1 << bit;
        else
            flags &= ~(1 << bit);

        SetValue(name, flags, persistent);
    }

    public int GetGlobalValue(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, int>();
        }

        int v = 0;
        //try local
        if(!mGlobalStates.TryGetValue(name, out v)) {
            //try user data
            string key = string.Format(GlobalDataFormat, name);
            v = UserData.instance.GetInt(key, 0);
            mGlobalStates.Add(name, v);
        }

        return v;
    }

    public void SetGlobalValue(string name, int val, bool persistent) {
        bool isValueSet = false;
        int curVal = 0;
        if(mGlobalStates.TryGetValue(name, out curVal)) {
            isValueSet = curVal != val;

            if(isValueSet)
                mGlobalStates[name] = val;
        }
        else {
            isValueSet = true;

            mGlobalStates.Add(name, val);
        }

        if(isValueSet) {
            if(persistent) {
                string key = string.Format(GlobalDataFormat, name);
                UserData.instance.SetInt(key, val);
            }

            if(onValueChange != null) {
                onValueChange(true, name, val);
            }
        }
    }

    public bool CheckGlobalFlag(string name, int bit) {
        int flags = GetGlobalValue(name);

        return (flags & (1 << bit)) != 0;
    }

    public void SetGlobalFlag(string name, int bit, bool state, bool persistent) {
        int flags = GetValue(name);

        if(state)
            flags |= 1 << bit;
        else
            flags &= ~(1 << bit);

        SetGlobalValue(name, flags, persistent);
    }

    public void ResetGlobalValues() {
    }

    public void ResetValues() {
        mStates = null;
        mPrevStates = null;
        mPrevScene = null;

        AppendInitData();
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;

        mStartData = new Dictionary<string, InitData[]>(startData.Length);
        foreach(InitSceneData sdat in startData) {
            if(!string.IsNullOrEmpty(sdat.scene) && sdat.data != null)
                mStartData.Add(sdat.scene, sdat.data);
        }
    }

    void Start() {
        AppendGlobalInitData();

        if(mStates == null && mScene == null) {
            mScene = Application.loadedLevelName;
            mStates = new Dictionary<string, int>();
            AppendInitData();
        }
    }

    void SceneChange(string toScene) {
        //new scene is being loaded, save current state to previous
        //revert flags if new scene is previous
        if(toScene != mScene) {
            string curScene = mScene;
            Dictionary<string, int> curStates = mStates;

            mScene = toScene;

            if(toScene == mPrevScene) {
                mStates = mPrevStates;
            }
            else {
                mStates = new Dictionary<string, int>();
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
                int val = mGlobalStates[key];
                string userKey = string.Format(GlobalDataFormat, key);
                mGlobalStates[key] = ud.GetInt(userKey, val);
            }
        }

        //update states
        if(mStates != null && mStates.Count > 0 && !string.IsNullOrEmpty(mScene)) {
            string[] keys = new string[mStates.Count];
            mStates.Keys.CopyTo(keys, 0);

            foreach(string key in keys) {
                int val = mStates[key];
                string userKey = string.Format(DataFormat, mScene, key);
                mStates[key] = ud.GetInt(userKey, val);
            }
        }
    }

    void AppendGlobalInitData() {
        foreach(InitData dat in globalStartData) {
            if(!string.IsNullOrEmpty(dat.name)) {
                //check if data exists in user first
                string key = string.Format(GlobalDataFormat, dat.name);

                int val;

                if(UserData.instance.HasKey(key)) {
                    val = UserData.instance.GetInt(key);
                }
                else {
                    val = dat.val;

                    if(dat.persistent)
                        UserData.instance.SetInt(key, val);
                }

                if(mGlobalStates.ContainsKey(dat.name))
                    mGlobalStates[dat.name] = val;
                else
                    mGlobalStates.Add(dat.name, val);

                if(onValueChange != null) {
                    onValueChange(true, dat.name, val);
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

                    int val;

                    if(UserData.instance.HasKey(key)) {
                        val = UserData.instance.GetInt(key);
                    }
                    else {
                        val = dat.val;

                        if(dat.persistent)
                            UserData.instance.SetInt(key, val);
                    }

                    if(mStates.ContainsKey(dat.name))
                        mStates[dat.name] = val;
                    else
                        mStates.Add(dat.name, val);

                    if(onValueChange != null) {
                        onValueChange(false, dat.name, val);
                    }
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/SceneState")]
    public class SceneState : SingletonBehaviour<SceneState> {
        public const string DataFormat = "{0}:{1}";

        public int localStateCache = 0;

        public delegate void StateCallback(string name, StateValue val);

        public enum Type {
            Invalid,
            Integer,
            Float,
            String
        }

        [System.Serializable]
        public class InitData {
            public string name = "";
            public Type type = Type.Integer;
            public int ival = 0;
            public float fval = 0.0f;
            public string sval = "";

            public StateValue stateValue {
                get {
                    switch(type) {
                        case Type.Integer:
                            return new StateValue(ival);
                        case Type.Float:
                            return new StateValue(fval);
                        case Type.String:
                            return new StateValue(sval);
                    }
                    return new StateValue() { type=Type.Invalid };
                }
            }
        }

        [System.Serializable]
        public class InitSceneData {
            public string scene = "";
            public InitData[] data = null;

            public bool editFoldout = true; //just for editing
        }

        public struct StateValue {
            public Type type;
            public int ival;
            public float fval;
            public string sval;

            public StateValue(int aVal) {
                type = Type.Integer;
                ival = aVal; fval = 0.0f; sval = "";
            }

            public StateValue(float aFval) {
                type = Type.Float;
                ival = 0; fval = aFval; sval = "";
            }

            public StateValue(string aSval) {
                type = Type.String;
                ival = 0; fval = 0.0f; sval = aSval;
            }

            public StateValue(UserData ud, string key) {
                System.Type t = ud.GetType(key);
                if(t == typeof(int)) { type = Type.Integer; ival = ud.GetInt(key, 0); fval = 0f; sval = ""; }
                else if(t == typeof(float)) { type = Type.Float; fval = ud.GetFloat(key, 0f); ival = 0; sval = ""; }
                else if(t == typeof(string)) { type = Type.String; sval = ud.GetString(key, ""); ival = 0; fval = 0f; }
                else { type = Type.Invalid; ival = 0; fval = 0f; sval = ""; }
            }
        }

        public class Table : IEnumerable<KeyValuePair<string, StateValue>> {
            public event StateCallback onValueChange;

            private Dictionary<string, StateValue> mStates;
            private string mPrefix;

            private Dictionary<string, StateValue> mStatesSnapshot;
            private InitData[] mStartData;

            //cache is used for when Init is called with a different prefix
            private struct CacheData {
                public string prefix;
                public Dictionary<string, StateValue> states;
            }

            private List<CacheData> mCache;

            public string prefix { get { return mPrefix; } }

            public Table(string prefix, InitData[] startData, int cacheCount = 0) {
                mStates = new Dictionary<string, StateValue>();

                if(cacheCount > 0)
                    mCache = new List<CacheData>(cacheCount);

                Init(prefix, startData);
            }

            public void Init(string prefix, InitData[] startData) {
                mStartData = startData;

                //store current prefix, restore data if new prefix exists
                if(mCache != null && mPrefix != prefix) {
                    int cacheInd = -1;
                    for(int i = 0; i < mCache.Count; i++) {
                        var cache = mCache[i];
                        if(cache.prefix == mPrefix) {
                            //store states
                            cache.states = new Dictionary<string, StateValue>(mStates);
                            mCache[i] = cache;
                            cacheInd = i;
                            break;
                        }
                    }

                    //store new cache
                    if(cacheInd == -1) {
                        //remove oldest cache
                        if(mCache.Count == mCache.Capacity)
                            mCache.RemoveAt(0);

                        mCache.Add(new CacheData() { prefix=mPrefix, states=new Dictionary<string, StateValue>(mStates) });
                    }

                    //if new prefix exists, use its values
                    for(int i = 0; i < mCache.Count; i++) {
                        var cache = mCache[i];
                        if(cache.prefix == prefix) {
                            mPrefix = prefix;
                            mStates = cache.states;
                            return;
                        }
                    }
                }

                mPrefix = prefix;
                                                                                
                Reset();
            }

            /// <summary>
            /// Reset all data to startData
            /// </summary>
            public void Reset() {
                Clear(false);

                //TODO: currently only grab from main userdata
                UserData ud = UserData.main;

                if(mStartData != null) {
                    for(int i = 0; i < mStartData.Length; i++) {
                        InitData dat = mStartData[i];
                        if(!string.IsNullOrEmpty(dat.name)) {
                            string key = string.Format(DataFormat, mPrefix, dat.name);

                            //check from userdata first, if invalid, then use init data
                            StateValue s = new StateValue(ud, key);
                            if(s.type == Type.Invalid)
                                s = dat.stateValue;

                            //dat.stateValue;
                            if(s.type != Type.Invalid) {
                                if(mStates.ContainsKey(dat.name))
                                    mStates[dat.name] = s;
                                else
                                    mStates.Add(dat.name, s);

                                if(onValueChange != null) {
                                    onValueChange(dat.name, s);
                                }
                            }
                        }
                    }
                }
            }

            public IEnumerator<KeyValuePair<string, StateValue>> GetEnumerator() {
                return mStates.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }

            public void RefreshFromUserData(UserData ud) {
                string[] keys = new string[mStates.Count];
                mStates.Keys.CopyTo(keys, 0);

                for(int i = 0; i < keys.Length; i++) {
                    string key = keys[i];

                    //set only if valid
                    StateValue val = new StateValue(ud, string.Format(DataFormat, mPrefix, key));
                    if(val.type != Type.Invalid)
                        mStates[key] = val;
                }
            }

            public bool Contains(string name) {
                if(!mStates.ContainsKey(name)) {
                    //try user data
                    return UserData.main.HasKey(string.Format(DataFormat, mPrefix, name));
                }

                return true;
            }

            public void DeleteValuesByNameContain(string nameContains, bool persistent) {
                foreach(string key in new List<string>(mStates.Keys)) {
                    if(key.Contains(nameContains))
                        DeleteValue(key, persistent);
                }
            }

            public void DeleteValue(string name, bool persistent) {
                mStates.Remove(name);

                if(persistent)
                    UserData.main.Delete(string.Format(DataFormat, mPrefix, name));
            }

            /// <summary>
            /// Clear out states, if persistent is true, calls ClearUserData beforehand
            /// </summary>
            /// <param name="persistent"></param>
            public void Clear(bool persistent) {
                if(persistent)
                    ClearUserData();

                mStates.Clear();
            }

            /// <summary>
            /// Clear up saved states from UserData.main, this does not clear actual states
            /// </summary>
            public void ClearUserData() {
                foreach(var pair in mStates)
                    UserData.main.Delete(string.Format(DataFormat, mPrefix, pair.Key));
            }

            public StateValue GetValueRaw(string name) {
                if(mStates != null) {
                    StateValue v;
                    if(!mStates.TryGetValue(name, out v)) {
                        //try user data; if valid, add to states
                        string key = string.Format(DataFormat, mPrefix, name);
                        v = new StateValue(UserData.main, key);
                        if(v.type != Type.Invalid) {
                            mStates.Add(name, v);
                            return v;
                        }
                    }

                    return v;
                }

                return new StateValue() { type=Type.Invalid };
            }

            public Type GetValueType(string name) {
                if(mStates != null) {
                    StateValue v;
                    if(!mStates.TryGetValue(name, out v)) {
                        System.Type t = UserData.main.GetType(string.Format(DataFormat, mPrefix, name));
                        if(t == typeof(int))
                            return Type.Integer;
                        else if(t == typeof(float))
                            return Type.Float;
                        else if(t == typeof(string))
                            return Type.String;
                    }
                    else
                        return v.type;
                }
                return Type.Invalid;
            }

            public int GetValue(string name, int defaultVal = 0) {
                StateValue v;
                //try local
                if(!mStates.TryGetValue(name, out v)) {
                    //try user data; if valid, add to states
                    string key = string.Format(DataFormat, mPrefix, name);
                    v = new StateValue(UserData.main, key);
                    if(v.type == Type.Integer) {
                        mStates.Add(name, v);
                        return v.ival;
                    }
                }
                else if(v.type == Type.Integer)
                    return v.ival;

                return defaultVal;
            }

            public void SetValue(string name, int val, bool persistent) {
                bool isValueSet = false;
                StateValue curVal;
                if(mStates.TryGetValue(name, out curVal)) {
                    if(curVal.type == Type.Integer) {
                        isValueSet = curVal.ival != val;

                        if(isValueSet) {
                            curVal.ival = val;
                            mStates[name] = curVal;
                        }
                    }
                    else {
                        isValueSet = true;
                        mStates[name] = curVal = new StateValue(val);
                    }
                }
                else {
                    isValueSet = true;
                    mStates.Add(name, curVal = new StateValue(val));
                }

                if(isValueSet) {
                    if(persistent) {
                        string key = string.Format(DataFormat, mPrefix, name);
                        UserData.main.SetInt(key, val);
                    }

                    if(onValueChange != null) {
                        onValueChange(name, curVal);
                    }
                }
            }

            public void SetPersist(string name, bool persist) {
                if(persist) {
                    StateValue curVal;
                    if(mStates.TryGetValue(name, out curVal)) {
                        string key = string.Format(DataFormat, mPrefix, name);
                        switch(curVal.type) {
                            case Type.Integer:
                                UserData.main.SetInt(key, curVal.ival);
                                break;
                            case Type.Float:
                                UserData.main.SetFloat(key, curVal.fval);
                                break;
                            case Type.String:
                                UserData.main.SetString(key, curVal.sval);
                                break;
                        }
                    }
                }
                else {
                    string key = string.Format(DataFormat, mPrefix, name);
                    UserData.main.Delete(key);
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
                StateValue v;
                //try local
                if(!mStates.TryGetValue(name, out v)) {
                    //try user data; if valid, add to states
                    string key = string.Format(DataFormat, mPrefix, name);
                    v = new StateValue(UserData.main, key);
                    if(v.type == Type.Float) {
                        mStates.Add(name, v);
                        return v.fval;
                    }
                }
                else if(v.type == Type.Float)
                    return v.fval;

                return defaultVal;
            }

            public void SetValueFloat(string name, float val, bool persistent) {
                bool isValueSet = false;
                StateValue curVal;
                if(mStates.TryGetValue(name, out curVal)) {
                    if(curVal.type == Type.Float) {
                        isValueSet = curVal.fval != val;

                        if(isValueSet) {
                            curVal.fval = val;
                            mStates[name] = curVal;
                        }
                    }
                    else {
                        isValueSet = true;
                        mStates[name] = curVal = new StateValue(val);
                    }
                }
                else {
                    isValueSet = true;
                    mStates.Add(name, curVal = new StateValue(val));
                }

                if(isValueSet) {
                    if(persistent) {
                        string key = string.Format(DataFormat, mPrefix, name);
                        UserData.main.SetFloat(key, val);
                    }

                    if(onValueChange != null) {
                        onValueChange(name, curVal);
                    }
                }
            }

            public string GetValueString(string name, string defaultVal = "") {
                StateValue v;
                //try local
                if(!mStates.TryGetValue(name, out v)) {
                    //try user data; if valid, add to states
                    string key = string.Format(DataFormat, mPrefix, name);
                    v = new StateValue(UserData.main, key);
                    if(v.type == Type.String) {
                        mStates.Add(name, v);
                        return v.sval;
                    }
                }
                else if(v.type == Type.String)
                    return v.sval;

                return defaultVal;
            }

            public void SetValueString(string name, string val, bool persistent) {
                bool isValueSet = false;
                StateValue curVal;
                if(mStates.TryGetValue(name, out curVal)) {
                    if(curVal.type == Type.String) {
                        isValueSet = curVal.sval != val;

                        if(isValueSet) {
                            curVal.sval = val;
                            mStates[name] = curVal;
                        }
                    }
                    else {
                        isValueSet = true;
                        mStates[name] = curVal = new StateValue(val);
                    }
                }
                else {
                    isValueSet = true;
                    mStates.Add(name, curVal = new StateValue(val));
                }

                if(isValueSet) {
                    if(persistent) {
                        string key = string.Format(DataFormat, mPrefix, name);
                        UserData.main.SetString(key, val);
                    }

                    if(onValueChange != null) {
                        onValueChange(name, curVal);
                    }
                }
            }

            public string[] GetKeys(System.Predicate<KeyValuePair<string, StateValue>> predicate) {
                List<string> items = new List<string>(mStates.Count);
                foreach(KeyValuePair<string, StateValue> pair in mStates) {
                    if(predicate(pair))
                        items.Add(pair.Key);
                }

                return items.ToArray();
            }

            public void SnapshotSave() {
                if(mStates != null)
                    mStatesSnapshot = new Dictionary<string, StateValue>(mStates);
                else
                    mStatesSnapshot = null;
            }

            public void SnapshotRestore() {
                if(mStatesSnapshot != null)
                    mStates = new Dictionary<string, StateValue>(mStatesSnapshot);
            }

            public void SnapshotDelete() {
                mStatesSnapshot = null;
            }
        }

        public InitData[] globalStartData;

        public InitSceneData[] startData; //use for debug

        private Dictionary<string, InitData[]> mStartData;

        private Table mLocal = null;
        private Table mGlobal = null;

        public Table global { get { return mGlobal; } }

        public Table local { get { return mLocal; } }

        protected override void OnInstanceInit() {
            UserData.main.actCallback += OnUserDataAction;

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

            mStartData = new Dictionary<string, InitData[]>(startData.Length);
            foreach(InitSceneData sdat in startData) {
                if(!string.IsNullOrEmpty(sdat.scene) && sdat.data != null)
                    mStartData.Add(sdat.scene, sdat.data);
            }

            InitLocalData(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            InitGlobalData();
        }

        protected override void OnInstanceDeinit() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
            if(mode == UnityEngine.SceneManagement.LoadSceneMode.Single)
                InitLocalData(scene);
        }

        void OnUserDataAction(UserData ud, UserData.Action act) {
            switch(act) {
                case UserData.Action.Load:
                    //update global states
                    mGlobal.RefreshFromUserData(ud);

                    //update local states
                    mLocal.RefreshFromUserData(ud);
                    break;
            }
        }

        void InitGlobalData() {
            if(mGlobal != null)
                mGlobal.Init("g", globalStartData);
            else
                mGlobal = new Table("g", globalStartData);
        }

        void InitLocalData(UnityEngine.SceneManagement.Scene scene) {
            string sceneName = scene.name;

            InitData[] dats = null;
            mStartData.TryGetValue(sceneName, out dats);

            if(mLocal != null) {
                mLocal.SnapshotDelete();

                if(localStateCache == 0 || mLocal.prefix != sceneName)
                    mLocal.Init(sceneName, dats);
            }
            else
                mLocal = new Table(sceneName, dats, localStateCache);
        }
    }
}
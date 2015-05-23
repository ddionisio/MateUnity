using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class ProjectConfig : ScriptableObject, ISerializationCallbackReceiver {
        [System.Serializable]
        private class Data {
            [SerializeField]
            string _key;

            public int iVal = 0;
            public float fVal = 0.0f;
            public string sVal = "";
            public Object oVal = null;
                        
            public string key { get { return _key; } }

            public Data(string aKey) {
                _key = aKey;
            }
        }

        private const string filePath = "Assets/m8config.asset";

        [SerializeField]
        private List<Data> _values;

        private Dictionary<string, Data> mData = new Dictionary<string,Data>();

        private static ProjectConfig mInstance;

        private static ProjectConfig instance {
            get {
                if(mInstance == null) {
                    //grab from asset
                    mInstance = AssetDatabase.LoadAssetAtPath(filePath, typeof(ProjectConfig)) as ProjectConfig;

                    //create a new one
                    if(mInstance == null) {
                        mInstance = new ProjectConfig();
                        AssetDatabase.CreateAsset(mInstance, filePath);
                        AssetDatabase.SaveAssets();
                    }
                }

                return mInstance;
            }
        }

        public static float GetFloat(string key, float defaultVal = 0.0f) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.fVal = defaultVal;
            }

            return dat.fVal;
        }

        public static void SetFloat(string key, float val) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.fVal = val;
            }
            else {
                dat.fVal = val;
                EditorUtility.SetDirty(inst);
            }
        }

        public static int GetInt(string key, int defaultVal = 0) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.iVal = defaultVal;
            }

            return dat.iVal;
        }

        public static void SetInt(string key, int val) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.iVal = val;
            }
            else {
                dat.iVal = val;
                EditorUtility.SetDirty(inst);
            }
        }

        public static string GetString(string key, string defaultVal = "") {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.sVal = defaultVal;
            }

            return dat.sVal;
        }

        public static void SetString(string key, string val) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.sVal = val;
            }
            else {
                dat.sVal = val;
                EditorUtility.SetDirty(inst);
            }
        }

        public static Object GetObject(string key) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
            }

            return dat.oVal;
        }

        public static T GetObject<T>(string key) where T : Object {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
            }

            return dat.oVal as T;
        }

        public static void SetObject(string key, Object val) {
            var inst = instance;

            Data dat;

            if(!inst.mData.TryGetValue(key, out dat)) {
                dat = inst.CreateItem(key);
                dat.oVal = val;
            }
            else {
                dat.oVal = val;
                EditorUtility.SetDirty(inst);
            }
        }

        private Data CreateItem(string key) {
            Data newDat = new Data(key);
            mData.Add(key, newDat);
            EditorUtility.SetDirty(this);
            return newDat;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            mData = new Dictionary<string, Data>();

            if(_values == null)
                return;

            int count = _values.Count;
            for(int i = 0; i < count; i++) {
                var dat = _values[i];
                if(!mData.ContainsKey(dat.key))
                    mData.Add(dat.key, dat);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            _values = new List<Data>();

            if(mData == null)
                return;

            foreach(var pair in mData)
                _values.Add(pair.Value);
        }
    }
}
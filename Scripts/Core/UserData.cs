using UnityEngine;
using System.Collections.Generic;

using System;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace M8 {
    public abstract class UserData : ScriptableObject, System.Collections.IEnumerable {
        
        [System.Serializable]
        public struct Data {
            public string name;
            public object obj;
        }

        public bool isLoaded { get { return mValues != null; } }
        public int valueCount { get { return mValues != null ? mValues.Count : 0; } }

        /// <summary>
        /// Called before values are saved
        /// </summary>
        public event Action saveCallback;

        /// <summary>
        /// Called after values are loaded
        /// </summary>
        public event Action loadedCallback;

        private Dictionary<string, object> mValues = null;

        private Dictionary<string, object> mValuesSnapshot = null;
                                
        public string[] GetKeys(System.Predicate<KeyValuePair<string, object>> predicate) {
            List<string> items = new List<string>(mValues.Count);
            foreach(KeyValuePair<string, object> pair in mValues) {
                if(predicate(pair))
                    items.Add(pair.Key);
            }

            return items.ToArray();
        }

        public System.Collections.IEnumerator GetEnumerator() {
            if(mValues == null) return null;
            return mValues.GetEnumerator();
        }

        public void SnapshotSave() {
            mValuesSnapshot = mValues != null ? new Dictionary<string, object>(mValues) : null;
        }

        public void SnapshotRestore() {
            if(mValuesSnapshot != null) {
                mValues = new Dictionary<string, object>(mValuesSnapshot);

                if(loadedCallback != null)
                    loadedCallback();
            }
        }

        public void SnapshotDelete() {
            mValuesSnapshot = null;
        }

        public void SnapshotPreserve(string key) {
            if(mValuesSnapshot != null) {
                object val;
                if(mValues.TryGetValue(key, out val)) {
                    if(mValuesSnapshot.ContainsKey(key))
                        mValuesSnapshot[key] = val;
                    else
                        mValuesSnapshot.Add(key, val);
                }
            }
        }

        public void Load() {
            Data[] dat;

            byte[] raw = LoadRawData();
            if(raw != null && raw.Length > 0) {
                dat = LoadData(raw);

                mValues = new Dictionary<string, object>();
                foreach(Data datum in dat) {
                    mValues.Add(datum.name, datum.obj);
                }

                if(loadedCallback != null)
                    loadedCallback();
            }
        }

        public virtual void Save() {
            if(mValues != null) {
                if(saveCallback != null)
                    saveCallback();

                List<Data> dat = new List<Data>(mValues.Count);
                foreach(KeyValuePair<string, object> pair in mValues)
                    dat.Add(new Data() { name = pair.Key, obj = pair.Value });
                SaveData(dat.ToArray());
            }
        }

        public void Unload() {
            mValues = null;
            DeleteRawData();
        }

        public bool HasKey(string name) {
            return mValues != null && mValues.ContainsKey(name);
        }

        public System.Type GetType(string name) {
            object ret;
            if(mValues != null && mValues.TryGetValue(name, out ret)) {
                if(ret != null) return ret.GetType();
            }
            return null;
        }

        public int GetInt(string name, int defaultValue = 0) {
            object ret;
            if(mValues != null && mValues.TryGetValue(name, out ret)) {
                if(ret is int)
                    return System.Convert.ToInt32(ret);
            }

            return defaultValue;
        }

        public void SetInt(string name, int value) {
            if(mValues == null) mValues = new Dictionary<string, object>();

            if(!mValues.ContainsKey(name))
                mValues.Add(name, value);
            else
                mValues[name] = value;
        }

        public float GetFloat(string name, float defaultValue = 0) {
            object ret;
            if(mValues != null && mValues.TryGetValue(name, out ret)) {
                if(ret is float)
                    return System.Convert.ToSingle(ret);
            }

            return defaultValue;
        }

        public void SetFloat(string name, float value) {
            if(mValues == null) mValues = new Dictionary<string, object>();

            if(!mValues.ContainsKey(name))
                mValues.Add(name, value);
            else
                mValues[name] = value;
        }

        public string GetString(string name, string defaultValue = "") {
            object ret;
            if(mValues != null && mValues.TryGetValue(name, out ret)) {
                if(ret is string)
                    return System.Convert.ToString(ret);
            }

            return defaultValue;
        }

        public void SetString(string name, string value) {
            if(mValues == null) mValues = new Dictionary<string, object>();

            if(!mValues.ContainsKey(name))
                mValues.Add(name, value);
            else
                mValues[name] = value;
        }

        public void DeleteAllByNameContain(string nameContains) {
            if(mValues == null)
                return;

            var deleteKeys = new List<string>();
            foreach(var pair in mValues) {
                if(pair.Key.Contains(nameContains))
                    deleteKeys.Add(pair.Key);
            }

            for(int i = 0; i < deleteKeys.Count; i++)
                mValues.Remove(deleteKeys[i]);
        }

        public void DeleteByNamePredicate(Predicate<string> predicate) {
            if(mValues == null)
                return;

            var deleteKeys = new List<string>();
            foreach(var pair in mValues) {
                if(predicate(pair.Key))
                    deleteKeys.Add(pair.Key);
            }

            for(int i = 0; i < deleteKeys.Count; i++)
                mValues.Remove(deleteKeys[i]);
        }

        public void Delete(string name) {
            if(mValues != null)
                mValues.Remove(name);
        }

        ////////////////////////////////////////////
        // Implements
        ////////////////////////////////////////////

        protected abstract byte[] LoadRawData();

        protected abstract void SaveRawData(byte[] dat);

        protected abstract void DeleteRawData();

        private Data[] LoadData(byte[] dat) {
            Data[] ret = null;

            BinaryFormatter bf = new BinaryFormatter();
            using(MemoryStream ms = new MemoryStream(dat)) {
                ret = (Data[])bf.Deserialize(ms);
            }

            return ret == null ? new Data[0] : ret;
        }

        private void SaveData(Data[] dat) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, dat);
            SaveRawData(ms.GetBuffer());
        }

        /*void OnApplicationQuit() {
            if(autoSave)
                Save();
        }

        void SceneChange(string toScene) {
            if(autoSave)
                Save();
        }*/

        /*void OnDestroy() {
            if(mMain == this) {
                mMain = null;
                mMainExists = false;
            }

            if(mInstances != null && !string.IsNullOrEmpty(id))
                mInstances.Remove(id);

            if(SceneManager.instance)
                SceneManager.instance.sceneChangeCallback -= SceneChange;
        }*/

        /*void Awake() {
            if(_main) {
                mMain = this;
                mMainExists = true;
            }

            if(!string.IsNullOrEmpty(id)) {
                if(mInstances.ContainsKey(id))
                    Debug.LogWarning("UserData "+id+" already exists.");
                else
                    mInstances.Add(id, this);
            }

            SceneManager.instance.sceneChangeCallback += SceneChange;

            if(loadOnStart)
                LoadOnStart();
        }*/
    }
}
using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[AddComponentMenu("")]
public abstract class UserData : MonoBehaviour, System.Collections.IEnumerable {
    public enum Action {
        Load,
        Save
    }

    [System.Serializable]
    public struct Data {
        public string name;
        public object obj;
    }

    public delegate void OnAction(UserData ud, Action act);

    public bool loadOnStart = true;
    public bool autoSave = true;

    public event OnAction actCallback;
        
    private Dictionary<string, object> mValues = null;

    private Dictionary<string, object> mValuesSnapshot = null;
        
    private static UserData mInstance = null;

    public static UserData instance { get { return mInstance; } }

    public bool started { get; private set; }

    public int valueCount { get { return mValues != null ? mValues.Count : 0; } }

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

            if(actCallback != null)
                actCallback(this, Action.Load);
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

            if(actCallback != null)
                actCallback(this, Action.Load);
        }
    }

    public void Save() {
        if(mValues != null) {
            if(actCallback != null)
                actCallback(this, Action.Save);

            List<Data> dat = new List<Data>(mValues.Count);
            foreach(KeyValuePair<string, object> pair in mValues)
                dat.Add(new Data() { name = pair.Key, obj = pair.Value });
            SaveData(dat.ToArray());
        }
    }

    public void Delete() {
        if(mValues != null)
            mValues.Clear();
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
        if(mValues != null) {
            //ew
            foreach(string key in new List<string>(mValues.Keys)) {
                if(key.Contains(nameContains))
                    mValues.Remove(key);
            }
        }
    }

    public void Delete(string name) {
        if(mValues != null)
            mValues.Remove(name);
    }

    ////////////////////////////////////////////
    // Implements
    ////////////////////////////////////////////

    protected virtual void LoadOnStart() {
        Load();
    }

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

    void OnApplicationQuit() {
        if(autoSave) {
            Save();
            PlayerPrefs.Save();
        }
    }

    void SceneChange(string toScene) {
        if(autoSave) {
            Save();
            PlayerPrefs.Save();
            //Debug.Log("save");
        }
    }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;

            if(SceneManager.instance)
                SceneManager.instance.sceneChangeCallback -= SceneChange;
        }
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            SceneManager.instance.sceneChangeCallback += SceneChange;

            if(loadOnStart)
                LoadOnStart();
        }
    }

    void Start() {
        started = true;
    }
}

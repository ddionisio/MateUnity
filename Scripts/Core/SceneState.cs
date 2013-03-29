using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Put this in the core object, or on an object with DontDestroyOnLoad
[AddComponentMenu("M8/Scene/SceneState")]
public class SceneState : MonoBehaviour {
    public const string DataFormat = "{0}_{1}";

    public delegate void StateCallback(string name, int val);

    public event StateCallback onValueChange;

    private static SceneState mInstance = null;

    private string mScene = null; //scene associated with current flags
    private Dictionary<string, int> mStates = null;

    //only buffer one level previous for now
    private string mPrevScene = null;
    private Dictionary<string, int> mPrevStates = null;

    public static SceneState instance { get { return mInstance; } }

    public int GetValue(string name) {
        if(mStates == null) {
            mStates = new Dictionary<string, int>();
        }

        int v = 0;
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

        int curVal = 0;
        mStates.TryGetValue(name, out curVal);

        if(curVal != val) {
            mStates.Add(name, val);

            if(persistent) {
                string key = string.Format(DataFormat, mScene, name);
                UserData.instance.SetInt(key, val);
            }

            if(onValueChange != null) {
                onValueChange(name, val);
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
            flags ^= 1 << bit;

        SetValue(name, flags, persistent);
    }

    public void Clear() {
        mStates = null;
        mPrevStates = null;
        mPrevScene = null;
    }

    void OnDestroy() {
        mInstance = null;
    }

    void Awake() {
        mInstance = this;
    }

    void SceneChange(string toScene) {
        //new scene is being loaded, save current state to previous
        //revert flags if new scene is previous
        if(toScene != mScene) {
            string curScene = mScene;
            Dictionary<string, int> curStates = mStates;

            if(toScene == mPrevScene) {
                mScene = toScene;
                mStates = mPrevStates;
            }

            mPrevScene = curScene;
            mPrevStates = curStates;
        }
    }

    void OnUserDataLoad(UserData ud) {
        //update states
        if(mStates != null && !string.IsNullOrEmpty(mScene)) {
            foreach(KeyValuePair<string, int> val in mStates) {
                string key = string.Format(DataFormat, mScene, val.Key);
                mStates[name] = ud.GetInt(key, val.Value);
            }
        }
    }
}

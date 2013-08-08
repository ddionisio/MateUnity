using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use this to serialize game object on scene with int/float values such that they persist when loading scene or saved in file
/// Also includes a persistent id that can be guaranteed to be the same when scene is loaded.
/// NOTE: This requires SceneState
/// </summary>
[AddComponentMenu("M8/Core/SceneSerializer")]
public class SceneSerializer : MonoBehaviour {
    public const int invalidID = 0;

    [SerializeField]
    [HideInInspector]
    int _id = invalidID;

    private static Dictionary<int, SceneSerializer> mRefs = null;

    /// <summary>
    /// Get the serialized id, for ungenerated ids, make sure you call this after Awake (usu. at Start)
    /// </summary>
    public int id {
        get {
#if UNITY_EDITOR
            if(!Application.isPlaying) {
                return _id;
            }
#endif
            if(_id == invalidID) {
                //find an id we can use
                int __id = 1;
                for(; mRefs.ContainsKey(__id); __id++) ;

                _id = __id;
                mRefs.Add(_id, this);
            }

            return _id;
        }
    }

    /// <summary>
    /// This is only used by the editor!
    /// </summary>
    /// <param name="nid"></param>
    public void __EditorSetID(int nid) {
        _id = nid;
    }

    public static SceneSerializer GetObject(int id) {
        SceneSerializer ret = null;

        if(mRefs != null) {
            mRefs.TryGetValue(id, out ret);
        }

        return ret;
    }

    string GetVarKey(string name) {
        return string.Format("obj{0}{1}", _id, name);
    }

    public bool HasValue(string name) {
        return SceneState.instance.HasValue(GetVarKey(name));
    }

    public void DeleteValue(string name, bool persistent) {
        SceneState.instance.DeleteValue(GetVarKey(name), persistent);
    }

    public int GetValue(string name, int defaultVal = 0) {
        return SceneState.instance.GetValue(GetVarKey(name), defaultVal);
    }

    public void SetValue(string name, int val, bool persistent) {
        SceneState.instance.SetValue(GetVarKey(name), val, persistent);
    }

    public bool CheckFlag(string name, int bit) {
        return SceneState.instance.CheckFlagMask(GetVarKey(name), 1 << bit);
    }

    public bool CheckFlagMask(string name, int mask) {
        return SceneState.instance.CheckFlagMask(GetVarKey(name), mask);
    }

    public void SetFlag(string name, int bit, bool state, bool persistent) {
        SceneState.instance.SetFlag(GetVarKey(name), bit, state, persistent);
    }

    public float GetValueFloat(string name, float defaultVal = 0.0f) {
        return SceneState.instance.GetValueFloat(GetVarKey(name), defaultVal);
    }

    public void SetValueFloat(string name, float val, bool persistent) {
        SceneState.instance.SetValueFloat(GetVarKey(name), val, persistent);
    }

    void OnDestroy() {
        if(mRefs != null) {
            if(_id != invalidID) {
                mRefs.Remove(_id);
            }

            if(mRefs.Count == 0) {
                mRefs = null;
            }
        }
    }

    void Awake() {
        if(mRefs == null)
            mRefs = new Dictionary<int, SceneSerializer>();

        if(_id != invalidID) {
            mRefs[_id] = this;
        }
    }
}

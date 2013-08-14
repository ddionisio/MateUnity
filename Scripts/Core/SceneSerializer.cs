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
    public const string removeKey = "_del";

    [SerializeField]
    [HideInInspector]
    int _id = invalidID;

    private static Dictionary<int, SceneSerializer> mRefs = null;
    private static int mGenIdStart = invalidID + 1;

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
                __GenNewID();
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

    /// <summary>
    /// Use with caution, most likely after spawning
    /// </summary>
    public void __GenNewID() {
        //find an id we can use
        int __id = mGenIdStart;
        for(; mRefs.ContainsKey(__id); __id++) ;

        _id = __id;
        mRefs.Add(_id, this);
    }

    public static SceneSerializer GetObject(int id) {
        SceneSerializer ret = null;

        if(mRefs != null) {
            mRefs.TryGetValue(id, out ret);
        }

        return ret;
    }

    string GetVarId() {
        return "obj" + id.ToString();
    }

    string GetVarKey(string name) {
        return GetVarId() + name;
    }

    public bool HasValue(string name) {
        return SceneState.instance.HasValue(GetVarKey(name));
    }

    public void DeleteValue(string name, bool persistent) {
        if(_id != invalidID)
            SceneState.instance.DeleteValue(GetVarKey(name), persistent);
    }

    public int GetValue(string name, int defaultVal = 0) {
        if(_id == invalidID)
            return defaultVal;

        return SceneState.instance.GetValue(GetVarKey(name), defaultVal);
    }

    public void SetValue(string name, int val, bool persistent) {
        if(_id != invalidID)
            SceneState.instance.SetValue(GetVarKey(name), val, persistent);
    }

    public bool CheckFlag(string name, int bit) {
        if(_id == invalidID)
            return false;

        return SceneState.instance.CheckFlagMask(GetVarKey(name), 1 << bit);
    }

    public bool CheckFlagMask(string name, int mask) {
        if(_id == invalidID)
            return false;

        return SceneState.instance.CheckFlagMask(GetVarKey(name), mask);
    }

    public void SetFlag(string name, int bit, bool state, bool persistent) {
        if(_id != invalidID)
            SceneState.instance.SetFlag(GetVarKey(name), bit, state, persistent);
    }

    public float GetValueFloat(string name, float defaultVal = 0.0f) {
        if(_id == invalidID)
            return defaultVal;

        return SceneState.instance.GetValueFloat(GetVarKey(name), defaultVal);
    }

    public void SetValueFloat(string name, float val, bool persistent) {
        if(_id != invalidID)
            SceneState.instance.SetValueFloat(GetVarKey(name), val, persistent);
    }

    /// <summary>
    /// Call this if you no longer want variables for this object to be kept in user data and scene data
    /// </summary>
    public void DeleteAllValues() {
        if(_id != invalidID) {
            SceneState.instance.DeleteValuesByNameContain(GetVarId());
        }
        else {
            Debug.LogWarning("Invalid id for "+name+", nothing to delete.");
        }
    }

    /// <summary>
    /// Call this to remove the object from scene when loaded.
    /// </summary>
    public void MarkRemove() {
        if(_id != invalidID) {
            DeleteAllValues();
            SetValue(removeKey, 1, true);
        }
    }

    void OnDestroy() {
        if(mRefs != null) {
            if(_id != invalidID) {
                mRefs.Remove(_id);
            }

            if(mRefs.Count == 0) {
                mRefs = null;
                mGenIdStart = invalidID + 1;
            }
        }
    }

    void Awake() {
        //check if this object has been marked as remove, then delete it right away
        if(GetValue(removeKey, 0) == 1) {
            DestroyImmediate(gameObject);
            return;
        }

        if(mRefs == null)
            mRefs = new Dictionary<int, SceneSerializer>();

        if(_id != invalidID) {
            mRefs[_id] = this;

            if(_id >= mGenIdStart)
                mGenIdStart = _id + 1;
        }
    }
}

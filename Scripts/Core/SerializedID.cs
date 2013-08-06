using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use this as a reference to an object on scene if you need to save certain data and load it later.
/// </summary>
[AddComponentMenu("M8/Core/SerializedID")]
public class SerializedID : MonoBehaviour {
    public const int invalidID = 0;

    [SerializeField]
    [HideInInspector]
    int _id = invalidID;

    private static Dictionary<int, SerializedID> mRefs = null;
    
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

    public static SerializedID GetObject(int id) {
        SerializedID ret = null;
        
        if(mRefs != null) {
            mRefs.TryGetValue(id, out ret);
        }

        return ret;
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
            mRefs = new Dictionary<int, SerializedID>();

        if(_id != invalidID) {
            mRefs[_id] = this;
        }
    }
}

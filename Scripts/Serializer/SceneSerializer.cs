using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Use this to serialize game object on scene with int/float values such that they persist when loading scene or saved in file
    /// Also includes a persistent id that can be guaranteed to be the same when scene is loaded.
    /// NOTE: This requires SceneState
    /// </summary>
    [AddComponentMenu("M8/Serializer/Object")]
    public class SceneSerializer : MonoBehaviour {
        public const int invalidID = 0;
        public const string removeKey = "_del";

        [SerializeField]
        [HideInInspector]
        private int _id = invalidID;

        private static Dictionary<int, SceneSerializer> mRefs = null;

        /// <summary>
        /// Get the serialized id
        /// </summary>
        public int id {
            get {
                return _id;
            }
        }

        /// <summary>
        /// This is only used by the editor!
        /// </summary>
        /// <param name="nid"></param>
        public void __SetID(int nid) {
            if(Application.isPlaying) {
                if(mRefs == null)
                    mRefs = new Dictionary<int, SceneSerializer>();

                if(_id != invalidID) {
                    SceneSerializer ss;
                    if(mRefs.TryGetValue(_id, out ss) && ss == this)
                        mRefs.Remove(_id);
                }

                _id = nid;

                mRefs.Add(_id, this);
            }
            else {
                _id = nid;
            }
        }

        public static SceneSerializer GetObject(int id) {
            SceneSerializer ret = null;

            if(mRefs != null) {
                mRefs.TryGetValue(id, out ret);
            }

            return ret;
        }

        public static void DeleteValues(int sid) {
            SceneState.instance.local.DeleteValuesByNameContain(GetVarKey(sid), true);
        }

        static string GetVarKey(int sid) {
            return "o" + sid.ToString();
        }

        string GetVarKey(string name) {
            return GetVarKey(id) + name;
        }

        public bool HasValue(string name) {
            return SceneState.instance.local.Contains(GetVarKey(name));
        }

        public void DeleteValue(string name, bool persistent) {
            if(_id != invalidID)
                SceneState.instance.local.DeleteValue(GetVarKey(name), persistent);
        }

        public SceneState.Type GetValueType(string name) {
            if(_id == invalidID)
                return SceneState.Type.Invalid;
            return SceneState.instance.local.GetValueType(GetVarKey(name));
        }

        public int GetValue(string name, int defaultVal = 0) {
            if(_id == invalidID)
                return defaultVal;

            return SceneState.instance.local.GetValue(GetVarKey(name), defaultVal);
        }

        public void SetValue(string name, int val, bool persistent) {
            if(_id != invalidID)
                SceneState.instance.local.SetValue(GetVarKey(name), val, persistent);
        }

        public bool CheckFlag(string name, int bit) {
            if(_id == invalidID)
                return false;

            return SceneState.instance.local.CheckFlagMask(GetVarKey(name), 1u << bit);
        }

        public bool CheckFlagMask(string name, uint mask) {
            if(_id == invalidID)
                return false;

            return SceneState.instance.local.CheckFlagMask(GetVarKey(name), mask);
        }

        public void SetFlag(string name, int bit, bool state, bool persistent) {
            if(_id != invalidID)
                SceneState.instance.local.SetFlag(GetVarKey(name), bit, state, persistent);
        }

        public float GetValueFloat(string name, float defaultVal = 0.0f) {
            if(_id == invalidID)
                return defaultVal;

            return SceneState.instance.local.GetValueFloat(GetVarKey(name), defaultVal);
        }

        public void SetValueFloat(string name, float val, bool persistent) {
            if(_id != invalidID)
                SceneState.instance.local.SetValueFloat(GetVarKey(name), val, persistent);
        }

        public string GetValueString(string name, string defaultVal = "") {
            if(_id == invalidID)
                return defaultVal;

            return SceneState.instance.local.GetValueString(GetVarKey(name), defaultVal);
        }

        public void SetValueString(string name, string val, bool persistent) {
            if(_id != invalidID)
                SceneState.instance.local.SetValueString(GetVarKey(name), val, persistent);
        }

        /// <summary>
        /// Call this if you no longer want variables for this object to be kept in user data and scene data
        /// </summary>
        public void DeleteAllValues() {
            if(_id != invalidID) {
                DeleteValues(_id);
            }
            else {
                Debug.LogWarning("Invalid id for "+name+", nothing to delete.");
            }
        }

        /// <summary>
        /// Call this to remove the object from scene when loaded.
        /// </summary>
        public virtual void MarkRemove() {
            if(_id != invalidID) {
                DeleteAllValues();
                SetValue(removeKey, 1, true);
            }
        }

        protected virtual void Init() {
            //check if this object has been marked as remove, then delete it right away
            if(GetValue(removeKey, 0) == 1) {
                DestroyImmediate(gameObject);
                return;
            }

            if(mRefs == null)
                mRefs = new Dictionary<int, SceneSerializer>();

            if(_id != invalidID) {
                mRefs[_id] = this;
                if(UserData.main)
                    UserData.main.actCallback += OnUserDataAction;
            }
        }

        protected virtual void Deinit() {
            if(_id != invalidID) {
                if(mRefs != null)
                    mRefs.Remove(_id);
                _id = invalidID;
                if(UserData.main)
                    UserData.main.actCallback -= OnUserDataAction;
            }
        }

        protected virtual void OnUserDataAction(UserData ud, UserData.Action act) {
            if(act == UserData.Action.Load) {
                //check to see if we are suppose to be removed
                if(GetValue(removeKey) == 1)
                    DestroyImmediate(gameObject);
            }
        }

        void OnDestroy() {
            Deinit();
        }

        void Awake() {
            Init();
        }
    }
}
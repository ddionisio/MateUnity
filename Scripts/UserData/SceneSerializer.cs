using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Use this to serialize game object on scene with int/float values such that they persist when loading scene or saved in file
    /// Also includes a persistent id that can be guaranteed to be the same when scene is loaded.
    /// </summary>
    [AddComponentMenu("M8/Serializer/Object")]
    public class SceneSerializer : MonoBehaviour {
        public const int invalidID = 0;
        private const string removeKey = "_del";

        /// <summary>
        /// Get the serialized id
        /// </summary>
        public int id {
            get {
                return _id;
            }
        }

        public bool isLoaded {
            get {
                return _userData && _userData.isLoaded;
            }
        }

        public event System.Action loadedCallback;
        public event System.Action saveCallback;

        [SerializeField]
        [HideInInspector]
        int _id = invalidID;

        [SerializeField]
        UserData _userData = null;

        private static Dictionary<int, SceneSerializer> mRefs = null;

        private string mKeyHeader;
        private Dictionary<string, string> mVarKeyCache = new Dictionary<string, string>();
                
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

        string GetHeaderKey() {
            //setup key header based on which scene this object belongs to
            if(string.IsNullOrEmpty(mKeyHeader)) {
                var scene = gameObject.scene;

                var sb = new System.Text.StringBuilder();
                sb.Append(scene.name).Append("o").Append(_id);

                mKeyHeader = sb.ToString();

                mVarKeyCache.Clear();
            }

            return mKeyHeader;
        }

        string GetVarKey(string name) {
            string varKey;

            if(!mVarKeyCache.TryGetValue(name, out varKey)) {
                varKey = GetHeaderKey() + name;
                mVarKeyCache.Add(name, varKey);
            }

            return varKey;
        }

        public bool Contains(string name) {
            return _userData.HasKey(GetVarKey(name));
        }

        public void RemoveValue(string name) {
            if(_id != invalidID)
                _userData.Remove(GetVarKey(name));
        }

        public int GetInt(string name, int defaultVal = 0) {
            if(_id == invalidID)
                return defaultVal;

            return _userData.GetInt(GetVarKey(name), defaultVal);
        }

        public void SetInt(string name, int val) {
            if(_id != invalidID)
                _userData.SetInt(GetVarKey(name), val);
        }

        public bool CheckFlagMask(string name, uint mask) {
            if(_id == invalidID)
                return false;

            uint flags = (uint)GetInt(name, 0);

            return (flags & mask) != 0;
        }

        public void SetFlagMask(string name, uint mask, bool state) {
            if(_id != invalidID) {
                uint flags = (uint)GetInt(name, 0);

                if(state)
                    flags |= mask;
                else
                    flags &= ~mask;

                SetInt(name, (int)flags);
            }
        }

        public float GetFloat(string name, float defaultVal = 0.0f) {
            if(_id == invalidID)
                return defaultVal;

            return _userData.GetFloat(GetVarKey(name), defaultVal);
        }

        public void SetFloat(string name, float val) {
            if(_id != invalidID)
                _userData.SetFloat(GetVarKey(name), val);
        }

        public string GetString(string name, string defaultVal = "") {
            if(_id == invalidID)
                return defaultVal;

            return _userData.GetString(GetVarKey(name), defaultVal);
        }

        public void SetString(string name, string val) {
            if(_id != invalidID)
                _userData.SetString(GetVarKey(name), val);
        }

        /// <summary>
        /// Call this if you no longer want variables for this object to be kept in user data and scene data
        /// </summary>
        public void RemoveAllValues() {
            if(_id != invalidID) {
                var headerKey = GetHeaderKey();
                _userData.RemoveByNamePredicate((key) => key.IndexOf(headerKey) != -1);
            }
            else {
                Debug.LogWarning("Invalid id for "+name+", nothing to delete.");
            }
        }
        
        public void Load() {
            _userData.Load();
        }

        public void Save() {
            _userData.Save();
        }

        /// <summary>
        /// Call this to remove the object from scene when loaded.
        /// </summary>
        public virtual void MarkRemove() {
            if(_id != invalidID) {
                RemoveAllValues();
                SetInt(removeKey, 1);
            }
        }

        protected virtual void Init() {
            //check if this object has been marked as remove, then delete it right away
            if(_userData.isLoaded && GetInt(removeKey, 0) == 1) {
                DestroyImmediate(gameObject);
                return;
            }

            if(mRefs == null)
                mRefs = new Dictionary<int, SceneSerializer>();

            if(_id != invalidID) {
                mRefs[_id] = this;

                _userData.loadedCallback += OnUserDataLoaded;
                _userData.saveCallback += OnUserDataSave;
            }
        }

        private void Deinit() {
            if(_id != invalidID) {
                if(mRefs != null)
                    mRefs.Remove(_id);
                _id = invalidID;

                _userData.loadedCallback -= OnUserDataLoaded;
                _userData.saveCallback -= OnUserDataSave;
            }
        }

        private void OnUserDataLoaded() {
            //check to see if we are suppose to be removed
            if(GetInt(removeKey) == 1) {
                DestroyImmediate(gameObject);
                return;
            }

            if(loadedCallback != null)
                loadedCallback();
        }

        private void OnUserDataSave() {
            if(saveCallback != null)
                saveCallback();
        }

        void OnDestroy() {
            Deinit();
        }

        void Awake() {
            Init();
        }
    }
}
 
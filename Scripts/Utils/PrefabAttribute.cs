using UnityEngine;
using System;

namespace M8 {
    /// <summary>
    /// Load script from Resources/'path'.prefab if not found on scene, with persistent (default: false)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PrefabFromResourceAttribute : Attribute {
        public readonly string path;
        public readonly bool persistent;

        public PrefabFromResourceAttribute(string path, bool persistent) {
            this.path = path;
            this.persistent = persistent;
        }

        public PrefabFromResourceAttribute(string path) {
            this.path = path;
            this.persistent = false;
        }

        public virtual T Instantiate<T>() where T : MonoBehaviour {
            T ret;

            //first check if type exists on scene
            var objects = UnityEngine.Object.FindObjectsOfType<T>();
            if(objects.Length > 0) {
                ret = objects[0];
                if(objects.Length > 1) {
                    Debug.LogWarning("There is more than one instance of Singleton of type \"" + typeof(T) + "\". Keeping the first. Destroying the others.");
                    for(var i = 1; i < objects.Length; i++) UnityEngine.Object.DestroyImmediate(objects[i].gameObject);
                }

                return ret;
            }

            if(String.IsNullOrEmpty(path)) {
                Debug.LogError("Prefab name is empty for Singleton of type \"" + typeof(T) + "\".");
                return null;
            }

            var resGO = Resources.Load<GameObject>(path);
            if(resGO == null) {
                Debug.LogError("Could not find Prefab \"" + path + "\" on Resources for Singleton of type \"" + typeof(T) + "\".");
                return null;
            }

            var gameObject = UnityEngine.Object.Instantiate(resGO) as GameObject;
            gameObject.name = path;

            ret = gameObject.GetComponentInChildren<T>();
            if(!ret) {
                Debug.LogWarning("There wasn't a component of type \"" + typeof(T) + "\" inside prefab \"" + path + "\". Creating one.");
                ret = gameObject.AddComponent<T>();
            }

            if(persistent)
                UnityEngine.Object.DontDestroyOnLoad(gameObject);

            return ret;
        }
    }

    /// <summary>
    /// Load from Resources/core.prefab if not found on scene, persistent=true
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class PrefabCoreAttribute : PrefabFromResourceAttribute {
        //private static GameObject mCoreGameObject = null;

        public PrefabCoreAttribute()
            : base("core", true) {

        }

        /*public override T Instantiate<T>() {
            T ret;

            if(mCoreGameObject) {
                ret = mCoreGameObject.GetComponentInChildren<T>();
                if(!ret)
                    ret = mCoreGameObject.AddComponent<T>();
            }
            else {
                InstantiateGameObject();
                if(mCoreGameObject) {
                    ret = mCoreGameObject.GetComponentInChildren<T>();
                    if(!ret)
                        ret = mCoreGameObject.AddComponent<T>();
                }
                else
                    ret = null;
            }

            return ret;
        }*/

        public void InstantiateGameObject() {
            //if(!mCoreGameObject) {
                //check if in scene
                GameObject mCoreGameObject = GameObject.Find(path);
                if(!mCoreGameObject) {
                    var resGO = Resources.Load<GameObject>(path);
                    if(resGO) {
                        mCoreGameObject = UnityEngine.Object.Instantiate(resGO) as GameObject;
                        mCoreGameObject.name = path;
                        
                    }
                    else
                        Debug.LogError("Could not find Prefab \"" + path + "\" on Resources.");
                }
            //}
        }
    }
}
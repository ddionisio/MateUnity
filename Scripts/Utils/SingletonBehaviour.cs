using UnityEngine;
using System;

namespace M8 {
    /// <summary>
    /// Based on Kleber Lopes da Silva's solution:
    /// http://kleber-swf.com/singleton-monobehaviour-unity-projects/
    /// </summary>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
        public enum InstanceState {
            Uninitialized,
            Initialized,
            Destroyed
        }

        private static T mInstance;
        private static InstanceState mInstanceState = InstanceState.Uninitialized;

        public static T instance {
            get {
                if(mInstanceState == InstanceState.Uninitialized)
                    Instantiate();

                return mInstance;
            }
        }

        /// <summary>
        /// In case you need this instance again after it is destroyed.
        /// </summary>
        public static void Reinstantiate() {
            if(mInstance)
                DestroyImmediate(mInstance.gameObject);
            mInstanceState = InstanceState.Uninitialized;

            Instantiate();
        }

        private static void Instantiate() {
            var type = typeof(T);
            var attribute = Attribute.GetCustomAttribute(type, typeof(PrefabFromResourceAttribute)) as PrefabFromResourceAttribute;
            if(attribute != null) {
                GameObject go = attribute.InstantiateGameObject();
                DontDestroyOnLoad(go);

                if(!mInstance) {
                    mInstance = go.GetComponentInChildren<T>();
                    if(!mInstance) { //expecting the component to be in the prefab
                        Debug.LogError(type.ToString() + " not found in " + attribute.path);
                        return;
                    }
                }
            }
            else {
                //manually grab
                var objects = FindObjectsOfType<T>();

                if(objects.Length > 0) {
                    mInstance = objects[0];
                    if(objects.Length > 1) {
                        Debug.LogWarning("There is more than one instance of Singleton of type \"" + type + "\". Keeping the first. Destroying the others.");
                        for(var i = 1; i < objects.Length; i++) DestroyImmediate(objects[i].gameObject);
                    }
                }
                else {
                    //just create a gameobject
                    GameObject go = new GameObject(type.ToString());
                    mInstance = go.AddComponent<T>();
                }
            }

            if(mInstanceState == InstanceState.Uninitialized) { //not instantiated via awake?
                mInstanceState = InstanceState.Initialized;
                (mInstance as SingletonBehaviour<T>).OnInstanceInit();
            }
        }

        protected virtual void OnInstanceInit() { }
        protected virtual void OnInstanceDeinit() { }

        void OnDestroy() {
            if(mInstance == this) {
                OnInstanceDeinit();

                mInstance = null;
                mInstanceState = InstanceState.Destroyed;
            }
        }

        void Awake() {
            if(mInstanceState != InstanceState.Initialized) {
                mInstanceState = InstanceState.Initialized;
                mInstance = this as T;
                OnInstanceInit();

                if(transform.parent == null)
                    DontDestroyOnLoad(gameObject);
            }
            else
                DestroyImmediate(gameObject);
        }
    }
}
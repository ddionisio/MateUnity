using System;
using UnityEngine;

namespace M8 {
    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject {
        private static T mInstance;

        public static bool isInstantiated { get; private set; }

        public static T instance {
            get {
                if(!isInstantiated)
                    Instantiate();

                return mInstance;
            }
        }

        public static void Unload() {
            if(isInstantiated) {
                Resources.UnloadAsset(mInstance);
                isInstantiated = false;
            }
        }

        private static void Instantiate() {
            var type = typeof(T);

            //first, see if we can get a path
            string path = null;

            var resPathAttr = Attribute.GetCustomAttribute(type, typeof(ResourcePathAttribute)) as ResourcePathAttribute;
            if(resPathAttr != null) {
                path = resPathAttr.path;
            }
            else {
                //try unity's
                //NOTE: this will assume it is in the Resources folder, not in any of its sub folders.
                var createAssetMenuAttr = Attribute.GetCustomAttribute(type, typeof(CreateAssetMenuAttribute)) as CreateAssetMenuAttribute;
                if(createAssetMenuAttr != null) {
                    path = createAssetMenuAttr.fileName;
                }
            }

            if(!string.IsNullOrEmpty(path)) {
                mInstance = Resources.Load<T>(path);

                if(!mInstance) {
                    Debug.LogError("Failed to load ScriptableObject: " + type + ". Path: " + path);
                }
            }
            else {
                //manually grab
                var objects = Resources.FindObjectsOfTypeAll<T>();
                if(objects.Length > 0) {
                    mInstance = objects[0];

                    if(objects.Length > 1) {
                        Debug.LogWarning("There is more than one of ScriptableObject: \"" + type + "\". Using first one: "+mInstance.name);
                    }
                }
                else {
                    //create from memory
                    mInstance = ScriptableObject.CreateInstance<T>();
                    mInstance.hideFlags = HideFlags.DontSave;
                }
            }

            isInstantiated = mInstance != null;
            if(isInstantiated)
                (mInstance as SingletonScriptableObject<T>).OnInstanceInit();
        }

        /// <summary>
        /// This is called on the first access to "instance" during runtime.
        /// </summary>
        protected virtual void OnInstanceInit() { }

        protected virtual void OnDestroy() {
            isInstantiated = false;
        }
    }
}
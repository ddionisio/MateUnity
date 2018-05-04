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

        private static void Instantiate() {
            var type = typeof(T);
            var attribute = Attribute.GetCustomAttribute(type, typeof(ResourcePathAttribute)) as ResourcePathAttribute;
            if(attribute != null) {
                var path = attribute.path;

                mInstance = Resources.Load<T>(path);

                if(!mInstance) {
                    Debug.LogError("Failed to load ScriptableObject: " + type + ". Path: " + path);
                }
            }
            else {
                //manually grab/generate
                var objects = Resources.FindObjectsOfTypeAll<T>();
                if(objects.Length > 0) {
                    mInstance = objects[0];

                    if(objects.Length > 1) {
                        Debug.LogWarning("There is more than one of ScriptableObject: \"" + type + "\". Using: "+mInstance.name);
                    }
                }
                else {
                    Debug.LogError("Unable to find any ScriptableObject of type: " + type);
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
    }
}
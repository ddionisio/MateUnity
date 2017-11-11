using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public struct LocalizeData {
        public delegate string ParameterCallback(string paramKey);

        public string text;
        public string[] param;

        public LocalizeData(string aText, string[] aParam) {
            text = aText;
            param = aParam;
        }

        public string ApplyParams(Dictionary<string, ParameterCallback> paramCallbacks) {
            if(param != null && param.Length > 0) {
                //convert parameters
                string[] textParams = new string[param.Length];
                for(int i = 0; i < param.Length; i++) {
                    ParameterCallback cb;
                    if(paramCallbacks.TryGetValue(param[i], out cb))
                        textParams[i] = cb(param[i]);
                }

                return string.Format(text, textParams);
            }

            return text;
        }
    }

    [PrefabCore]
    [AddComponentMenu("")]
    public abstract class Localize : SingletonBehaviour<Localize> {
        public delegate void LocalizeCallback();
                
        public event LocalizeCallback localizeCallback;

        private int mCurIndex;
        
        private Dictionary<string, LocalizeData.ParameterCallback> mParams;

        public string this[string index] {
            get {
                return GetText(index);
            }
        }

        /// <summary>
        /// Set this to null or empty to use default
        /// </summary>
        public string language {
            get { return GetLanguageName(mCurIndex); }
            set {
                if(language != value) {
                    if(!string.IsNullOrEmpty(value)) {
                        int ind = GetLanguageIndex(value);
                        if(ind != -1)
                            languageIndex = ind;
                    }
                    else
                        languageIndex = 0;
                }
            }
        }

        public int languageIndex {
            get { return mCurIndex; }
            set {
                if(mCurIndex != value) {
                    mCurIndex = value;

                    HandleLanguageChanged();

                    Refresh();
                }
            }
        }

        public abstract string[] languages { get; }

        public abstract int languageCount { get; }

        public static string Get(string id) {
            return instance.GetText(id);
        }
                
        /// <summary>
        /// Register during Awake such that GetText will be able to fill params correctly
        /// </summary>
        public void RegisterParam(string paramKey, LocalizeData.ParameterCallback cb) {
            if(mParams == null)
                mParams = new Dictionary<string, LocalizeData.ParameterCallback>();

            if(mParams.ContainsKey(paramKey))
                mParams[paramKey] = cb;
            else
                mParams.Add(paramKey, cb);
        }

        /// <summary>
        /// Only call this after Load.
        /// </summary>
        public string GetText(string key) {
            LocalizeData ret;

            if(!TryGetData(key, out ret)) {
                Debug.LogWarning("Key not found: " + key);
                return key;
            }
            
            return ret.ApplyParams(mParams);
        }
                
        public void Refresh() {
            if(localizeCallback != null)
                localizeCallback();
        }

        /// <summary>
        /// Return the corresponding index to given "lang", -1 if not found.
        /// </summary>
        public abstract int GetLanguageIndex(string lang);

        /// <summary>
        /// Return the language based on its corresponding index, null/empty if not found
        /// </summary>
        public abstract string GetLanguageName(int langInd);

        public abstract bool Exists(string key);

        public abstract string[] GetKeys();

        /// <summary>
        /// This is mostly used by editor, clear out data
        /// </summary>
        public abstract void Unload();

        /// <summary>
        /// Check if given file path is used by this localizer.  This is mostly used by editor.
        /// </summary>
        public abstract bool IsLanguageFile(string filepath);

        protected abstract void HandleLanguageChanged();

        protected abstract bool TryGetData(string key, out LocalizeData data);
    }
}
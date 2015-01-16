using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/Localize")]
    public class Localize : SingletonBehaviour<Localize> {

        public delegate string ParameterCallback(string paramKey);
        public delegate void LocalizeCallback();

        [System.Serializable]
        public class TableDataPlatform {
            public RuntimePlatform platform;
            public TextAsset file;
        }

        [System.Serializable]
        public class TableData {
            public Language language;
            public TextAsset file;
            public TableDataPlatform[] platforms; //these overwrite certain keys in the string table
        }

        public class Entry {
            public string key;
            public string text = "";
            public string[] param = null; //set this to null if you want to reference param from base
        }

        public TextAsset baseFile; //the default localization
        public TableDataPlatform[] basePlatforms;

        public TableData[] tables; //table info for each language

        public event LocalizeCallback localizeCallback;

        private struct Data {
            public string text;
            public string[] param;

            public Data(string aText, string[] aParam) {
                text = aText;
                param = aParam;
            }

            public void ApplyParams(Dictionary<string, ParameterCallback> paramCallbacks) {
                if(param != null && param.Length > 0) {
                    //convert parameters
                    string[] textParams = new string[param.Length];
                    for(int i = 0; i < param.Length; i++) {
                        ParameterCallback cb;
                        if(paramCallbacks.TryGetValue(param[i], out cb))
                            textParams[i] = cb(param[i]);
                    }

                    text = string.Format(text, textParams);
                }
            }
        }

        private Dictionary<string, Data> mTableBase;

        private Dictionary<string, Data> mTable;

        private Language mCurLanguage = Language.Default;

        private Dictionary<string, ParameterCallback> mParams = null;

        public Language language {
            get { return mCurLanguage; }
            set {
                if(mCurLanguage != value) {
                    mCurLanguage = value;
                    LoadCurrentLanguage();
                }
            }
        }

        /// <summary>
        /// Register during Awake such that GetText will be able to fill params correctly
        /// </summary>
        public void RegisterParam(string paramKey, ParameterCallback cb) {
            if(mParams == null)
                mParams = new Dictionary<string, ParameterCallback>();

            if(mParams.ContainsKey(paramKey))
                mParams[paramKey] = cb;
            else
                mParams.Add(paramKey, cb);
        }

        /// <summary>
        /// Only call this after Load.
        /// </summary>
        public string GetText(string key) {
            Data ret = new Data("", null);

            if(mTable == null || !mTable.TryGetValue(key, out ret)) {
                if(mTableBase == null || !mTableBase.TryGetValue(key, out ret))
                    Debug.LogWarning("Key not found: " + key);
            }

            ret.ApplyParams(mParams);

            return ret.text;
        }

        public void Refresh() {
            if(localizeCallback != null)
                localizeCallback();
        }

        void LoadTables(string textData, ref Dictionary<string, Data> table) {
            List<Entry> tableEntries = fastJSON.JSON.ToObject<List<Entry>>(textData);

            table = new Dictionary<string, Data>(tableEntries.Count);

            foreach(Entry entry in tableEntries) {
                string[] parms = null;

                if(entry.param == null && table != mTableBase) {
                    Data dat;
                    if(mTableBase.TryGetValue(entry.key, out dat))
                        parms = dat.param;
                }
                else
                    parms = entry.param;

                table.Add(entry.key, new Data(entry.text, parms));
            }
        }

        void LoadCurrentPlatform(TableDataPlatform[] platforms, Dictionary<string, Data> table) {
            if(platforms == null) return;

            //append platform specific entries
            TableDataPlatform platform = null;
            foreach(TableDataPlatform platformDat in platforms) {
                if(platformDat.platform == Application.platform) {
                    platform = platformDat;
                    break;
                }
            }

            //override entries based on platform
            if(platform != null) {
                List<Entry> platformEntries = fastJSON.JSON.ToObject<List<Entry>>(platform.file.text);

                foreach(Entry platformEntry in platformEntries) {
                    Data dat;
                    if(table.TryGetValue(platformEntry.key, out dat)) {
                        dat.text = platformEntry.text;
                        if(platformEntry.param != null)
                            dat.param = platformEntry.param;

                        table[platformEntry.key] = dat;
                    }
                    else
                        table.Add(platformEntry.key, new Data(platformEntry.text, platformEntry.param));
                }
            }
        }

        void LoadCurrentLanguage() {
            fastJSON.JSON.Parameters.UseExtensions = false;

            //load language
            int langInd = (int)mCurLanguage;

            TableData dat = langInd < tables.Length ? tables[langInd] : null;
            if(dat != null) {
                if(dat.file)
                    LoadTables(dat.file.text, ref mTable);
                else
                    mTable = new Dictionary<string, Data>();

                LoadCurrentPlatform(dat.platforms, mTable);
            }

            if(localizeCallback != null)
                localizeCallback();
        }

        protected override void OnInstanceInit() {
            //load base localize
            if(baseFile) {
                fastJSON.JSON.Parameters.UseExtensions = false;

                LoadTables(baseFile.text, ref mTableBase);

                LoadCurrentPlatform(basePlatforms, mTableBase);
            }
        }
    }
}
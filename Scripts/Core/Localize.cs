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
            public string text;
            public string[] param;
        }

        public TextAsset baseFile; //the default localization
        public TableDataPlatform[] basePlatforms;

        public TableData[] tables; //table info for each language

        public event LocalizeCallback localizeCallback;

        private Dictionary<string, string> mTableBase;
        private Dictionary<string, string[]> mTableBaseParams;

        private Dictionary<string, string> mTable;
        private Dictionary<string, string[]> mTableParams;

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
            string ret;

            if(mTable == null || !mTable.TryGetValue(key, out ret)) {
                if(mTableBase != null) { //try base
                    if(!mTableBase.TryGetValue(key, out ret)) {
                        Debug.LogWarning("String table key not found: " + key);
                        return "";
                    }
                }
                else {
                    Debug.LogWarning("Attempting to access string table when not initialized! Key: " + key);
                    return "";
                }
            }

            //see if there's params
            string[] keyParams = null;

            if(mTableParams == null || !mTableParams.TryGetValue(key, out keyParams)) {
                if(mTableBaseParams != null) //try base
                    mTableBaseParams.TryGetValue(key, out keyParams);
            }

            if(keyParams != null) {
                //convert parameters
                string[] textParams = new string[keyParams.Length];
                for(int i = 0; i < keyParams.Length; i++) {
                    ParameterCallback cb;
                    if(mParams.TryGetValue(keyParams[i], out cb)) {
                        textParams[i] = cb(keyParams[i]);
                    }
                }

                ret = string.Format(ret, textParams);
            }

            return ret;
        }

        public void Refresh() {
            if(localizeCallback != null)
                localizeCallback();
        }

        void LoadTables(string textData, ref Dictionary<string, string> table, ref Dictionary<string, string[]> tableParams) {
            List<Entry> tableEntries = fastJSON.JSON.ToObject<List<Entry>>(textData);

            table = new Dictionary<string, string>();
            tableParams = new Dictionary<string, string[]>();

            foreach(Entry entry in tableEntries) {
                if(table.ContainsKey(entry.key))
                    table[entry.key] = entry.text;
                else {
                    table.Add(entry.key, entry.text);

                    if(entry.param != null && entry.param.Length > 0)
                        tableParams.Add(entry.key, entry.param);
                }
            }
        }

        void LoadCurrentPlatform(TableDataPlatform[] platforms, Dictionary<string, string> table) {
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
                    if(table.ContainsKey(platformEntry.key)) {
                        table[platformEntry.key] = platformEntry.text;
                    }
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
                    LoadTables(dat.file.text, ref mTable, ref mTableParams);

                LoadCurrentPlatform(dat.platforms, mTable);
            }

            if(localizeCallback != null)
                localizeCallback();
        }

        protected override void OnInstanceInit() {
            //load base localize
            if(baseFile) {
                fastJSON.JSON.Parameters.UseExtensions = false;

                LoadTables(baseFile.text, ref mTableBase, ref mTableBaseParams);

                LoadCurrentPlatform(basePlatforms, mTableBase);
            }
        }
    }
}
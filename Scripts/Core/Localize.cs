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
        public class Entry {
            public string key;
            public string text = "";
            public string[] param = null; //set this to null if you want to reference param from base
        }

        [System.Serializable]
        public struct EntryList {
            [SerializeField]
            private List<Entry> items;

            public static List<Entry> FromJSON(string json) {
                return !string.IsNullOrEmpty(json) ? JsonUtility.FromJson<EntryList>(json).items : new List<Entry>();
            }

            public static string ToJSON(List<Entry> _items, bool prettyPrint) {
                return JsonUtility.ToJson(new EntryList() { items = _items }, prettyPrint);
            }
        }

        public struct Data {
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

        [System.Serializable]
        public class TableDataPlatform {
            public RuntimePlatform platform;
            public TextAsset file;
        }

        [System.Serializable]
        public class TableData {
            public string language;
            public TextAsset file;
            public TableDataPlatform[] platforms; //these overwrite certain keys in the string table

            private Dictionary<string, Data> mEntries;

            public Dictionary<string, Data> entries { get { return mEntries; } }
            
            public void Generate(Dictionary<string, Data> baseTable) {
                if(file)
                    Generate(file.text, baseTable);
            }

            public void Generate(string json, Dictionary<string, Data> baseTable) {
                if(mEntries != null) return;

                List<Entry> tableEntries = EntryList.FromJSON(json);

                mEntries = new Dictionary<string, Data>(tableEntries.Count);

                foreach(Entry entry in tableEntries) {
                    string[] parms = null;

                    //if no params, grab from base
                    if(entry.param == null && baseTable != null) {
                        Data dat;
                        if(baseTable.TryGetValue(entry.key, out dat))
                            parms = dat.param;
                    }
                    else
                        parms = entry.param;

                    mEntries.Add(entry.key, new Data(entry.text, parms));
                }

                //append platform specific entries
                if(platforms != null) {
                    TableDataPlatform platform = null;
                    foreach(TableDataPlatform platformDat in platforms) {
                        if(platformDat.platform == Application.platform) {
                            platform = platformDat;
                            break;
                        }
                    }

                    //override entries based on platform
                    if(platform != null) {
                        List<Entry> platformEntries = EntryList.FromJSON(platform.file.text);

                        foreach(Entry platformEntry in platformEntries) {
                            Data dat;
                            if(mEntries.TryGetValue(platformEntry.key, out dat)) {
                                dat.text = platformEntry.text;
                                if(platformEntry.param != null)
                                    dat.param = platformEntry.param;

                                mEntries[platformEntry.key] = dat;
                            }
                            else
                                mEntries.Add(platformEntry.key, new Data(platformEntry.text, platformEntry.param));
                        }
                    }
                }
            }
        }
        
        [SerializeField]
        TableData[] tables; //table info for each language, first element is the root

        public event LocalizeCallback localizeCallback;
                
        private int mCurIndex;

        private Dictionary<string, ParameterCallback> mParams;

        public string this[string index] {
            get {
                return GetText(index);
            }
        }
                
        /// <summary>
        /// Set this to null or empty to use default
        /// </summary>
        public string language {
            get { return tables[mCurIndex].language; }
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
                    tables[mCurIndex].Generate(mCurIndex > 0 ? tables[0].entries : null);

                    Refresh();
                }
            }
        }

        public string[] languages {
            get {
                string[] ret = new string[tables.Length];
                for(int i = 0; i < ret.Length; i++)
                    ret[i] = tables[i].language;
                return ret;
            }
        }

        public int languageCount {
            get { return tables.Length; }
        }

        public static string Get(string id) {
            return instance.GetText(id);
        }

        public bool LoadLanguage(string language, string json) {
            int languageInd = GetLanguageIndex(language);
            if(languageInd == -1) {
                TableData newTable = new TableData();
                newTable.language = language;
                newTable.Generate(tables != null && tables.Length > 0 ? tables[0].entries : null);

                if(tables == null)
                    tables = new TableData[1];
                else
                    System.Array.Resize(ref tables, tables.Length + 1);

                tables[tables.Length - 1] = newTable;
            }
            else {
                tables[languageInd].Generate(json, languageInd > 0 ? tables[0].entries : null);
            }

            return true;
        }

        public int GetLanguageIndex(string lang) {
            for(int i = 0; i < tables.Length; i++) {
                if(tables[i].language == lang)
                    return i;
            }

            return -1;
        }

        public string GetLanguageName(int langInd) {
            return tables[langInd].language;
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

            if(!tables[mCurIndex].entries.TryGetValue(key, out ret)) {
                if(mCurIndex == 0 || !tables[0].entries.TryGetValue(key, out ret))
                    Debug.LogWarning("Key not found: " + key);
            }

            ret.ApplyParams(mParams);

            return ret.text;
        }

        public bool Exists(string key) {
            return tables[mCurIndex].entries.ContainsKey(key);
        }

        public void Refresh() {
            if(localizeCallback != null)
                localizeCallback();
        }

        protected override void OnInstanceInit() {
            //load base localize
            if(tables != null && tables.Length > 0)
                tables[0].Generate(null);
        }
    }
}
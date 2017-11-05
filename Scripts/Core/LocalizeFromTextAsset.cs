using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/Localize (TextAsset)")]
    public class LocalizeFromTextAsset : Localize {        
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

            private string mJson;

            public void SetJSON(string json) {
                mJson = json;
            }

            public Dictionary<string, Data> Generate(Dictionary<string, Data> baseTable) {
                if(!string.IsNullOrEmpty(mJson))
                    return Generate(mJson, baseTable);
                else if(file)
                    return Generate(file.text, baseTable);

                return new Dictionary<string, Data>();
            }

            public Dictionary<string, Data> Generate(string json, Dictionary<string, Data> baseTable) {
                List<Entry> tableEntries = EntryList.FromJSON(json);

                var entries = new Dictionary<string, Data>(tableEntries.Count);

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

                    entries.Add(entry.key, new Data(entry.text, parms));
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
                            if(entries.TryGetValue(platformEntry.key, out dat)) {
                                dat.text = platformEntry.text;
                                if(platformEntry.param != null)
                                    dat.param = platformEntry.param;

                                entries[platformEntry.key] = dat;
                            }
                            else
                                entries.Add(platformEntry.key, new Data(platformEntry.text, platformEntry.param));
                        }
                    }
                }

                return entries;
            }
        }

        [SerializeField]
        TableData[] tables; //table info for each language, first element is the root
        
        private Dictionary<string, Data> mEntriesBase; //loaded from first table data
        private Dictionary<string, Data> mEntries;
        
        public override string[] languages {
            get {
                string[] ret = new string[languageCount];

                for(int i = 0; i < ret.Length; i++)
                    ret[i] = tables[i].language;

                return ret;
            }
        }

        public override int languageCount {
            get {
                return tables.Length;
            }
        }
        
        /// <summary>
        /// Manually apply data to given language
        /// </summary>
        public void SetLanguageData(string language, string json) {
            int languageInd = GetLanguageIndex(language);
            if(languageInd == -1) {
                TableData newTable = new TableData();
                newTable.language = language;
                newTable.SetJSON(json);

                if(tables == null)
                    tables = new TableData[1];
                else
                    System.Array.Resize(ref tables, tables.Length + 1);

                tables[tables.Length - 1] = newTable;
            }
            else {
                tables[languageInd].SetJSON(json);
            }
        }

        public override int GetLanguageIndex(string lang) {
            for(int i = 0; i < tables.Length; i++) {
                if(tables[i].language == lang)
                    return i;
            }

            return -1;
        }

        public override string GetLanguageName(int langInd) {
            return tables[langInd].language;
        }
        
        public override bool Exists(string key) {
            return mEntries.ContainsKey(key);
        }

        public override string[] GetKeys() {
            if(mEntriesBase == null)
                GenerateEntries(true);

            if(mEntriesBase != null) {
                var keys = mEntriesBase.Keys;
                string[] keyArray = new string[keys.Count];
                keys.CopyTo(keyArray, 0);
                return keyArray;
            }

            return new string[0];
        }

        protected override void OnInstanceInit() {
            GenerateEntries(true);
        }

        protected override bool TryGetData(string key, out Data data) {
            if(!mEntries.TryGetValue(key, out data)) {
                if(mEntries == mEntriesBase || mEntriesBase == null || !mEntriesBase.TryGetValue(key, out data)) {
                    return false;
                }
            }

            return true;
        }

        protected override void HandleLanguageChanged() {
            GenerateEntries(false);
        }

        void GenerateEntries(bool isBase) {
            if(isBase) {
                if(tables.Length > 0)
                    mEntriesBase = tables[0].Generate(null);
                else
                    mEntriesBase = new Dictionary<string, Data>();

                if(mEntries == null)
                    mEntries = mEntriesBase;
            }
            else {
                if(languageIndex >= 0 && languageIndex < tables.Length)
                    mEntries = tables[languageIndex].Generate(mEntriesBase);
            }
        }
    }
}
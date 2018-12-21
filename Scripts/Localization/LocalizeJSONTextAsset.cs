using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace M8 {
    /// <summary>
    /// Grab localize data via TextAsset from Unity in JSON format
    /// </summary>
    [CreateAssetMenu(fileName = "localize", menuName = "M8/Localize/JSON From TextAssets")]
    public class LocalizeJSONTextAsset : Localize, ISerializationCallbackReceiver {
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

            public Dictionary<string, LocalizeData> Generate() {
                if(!file)
                    return new Dictionary<string, LocalizeData>();

                List<LocalizeJSON.Entry> tableEntries = JSONList<LocalizeJSON.Entry>.FromJSON(file.text);

                var entries = new Dictionary<string, LocalizeData>(tableEntries.Count);

                foreach(var entry in tableEntries) {
                    entries.Add(entry.key, new LocalizeData(entry.text, entry.param));
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
                    if(platform != null && platform.file) {
                        var platformEntries = JSONList<LocalizeJSON.Entry>.FromJSON(platform.file.text);

                        foreach(var platformEntry in platformEntries) {
                            LocalizeData dat;
                            if(entries.TryGetValue(platformEntry.key, out dat)) {
                                dat.text = platformEntry.text;
                                if(platformEntry.param != null && platformEntry.param.Length > 0)
                                    dat.param = platformEntry.param;

                                entries[platformEntry.key] = dat;
                            }
                            else
                                entries.Add(platformEntry.key, new LocalizeData(platformEntry.text, platformEntry.param));
                        }
                    }
                }

                return entries;
            }
        }

        [SerializeField]
        TableData[] tables = new TableData[0]; //table info for each language

        [SerializeField]
        int baseTableIndex = 0; //default table
        
        private Dictionary<string, LocalizeData> mEntries;
        
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
            if(mEntries == null)
                GenerateEntries(languageIndex);

            return mEntries.ContainsKey(key);
        }
                
        public override bool IsLanguageFile(string filepath) {
            if(tables == null)
                return false;

            for(int i = 0; i < tables.Length; i++) {
                //check via file's name
                if(tables[i].file && filepath.Contains(tables[i].file.name)) {
                    return true;
                }

                if(tables[i].platforms != null) {
                    //check via file's name
                    for(int j = 0; j < tables[i].platforms.Length; j++) {
                        if(tables[i].platforms[j].file && filepath.Contains(tables[i].platforms[j].file.name)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
                
        protected override bool TryGetData(string key, out LocalizeData data) {
            if(mEntries == null)
                GenerateEntries(languageIndex);

            return mEntries.TryGetValue(key, out data);
        }

        protected override void HandleLanguageChanged() {
            GenerateEntries(languageIndex);
        }

        protected override string[] HandleGetKeys() {
            if(mEntries == null)
                GenerateEntries(languageIndex);

            var keys = mEntries.Keys;
            string[] keyArray = new string[keys.Count];
            keys.CopyTo(keyArray, 0);

            return keyArray;
        }

        protected override void HandleLoad() {
            GenerateEntries(languageIndex);
        }

        void GenerateEntries(int tableIndex) {
            mEntries = tables[tableIndex].Generate();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            //nothing to serialize
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            //initialize default states
            SetLanguageIndex(baseTableIndex);
        }
    }
}
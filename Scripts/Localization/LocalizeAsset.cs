using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Data directly from this asset. Useful for games that really just need one or two language, but still allow managing of texts.
    /// </summary>
    [CreateAssetMenu(fileName = "localize", menuName = "M8/Localize/Asset")]
    public class LocalizeAsset : Localize, ISerializationCallbackReceiver {
        public enum TableEntryType {
            Language,
            Platform
        }

        [System.Serializable]
        public struct Entry {
            public string key;
            public string text;
            public string[] param; //set this to null if you want to reference param from base
        }

        [System.Serializable]
        public class TableData {
            public string name;

            public TableEntryType entryType;

            //for platforms
            public string languageDependency; //which Table this is dependent on (ie. grab values from table[dependency] and overwrite/add values)
            public RuntimePlatform platformDependency;

            public Entry[] entries;
        }

        [SerializeField]
        TableData[] tables;

        [SerializeField]
        int baseTableIndex;

        //populated through deserialization
        private string[] mLanguageTableNames;
        private int[] mLanguageTableIndices;
        private Dictionary<RuntimePlatform, List<int>> mPlatformTableIndices;

        //populated through loading and changing language
        private Dictionary<string, LocalizeData> mEntries;

        public override string[] languages {
            get {
                return mLanguageTableNames;
            }
        }

        public override int languageCount {
            get {
                return mLanguageTableNames.Length;
            }
        }
        
        public override int GetLanguageIndex(string lang) {
            for(int i = 0; i < mLanguageTableIndices.Length; i++) {
                if(mLanguageTableNames[i] == lang)
                    return i;
            }

            return -1;
        }

        public override string GetLanguageName(int langInd) {
            return mLanguageTableNames[langInd];
        }

        public override bool Exists(string key) {
            if(mEntries == null)
                GenerateEntries(baseTableIndex);

            return mEntries.ContainsKey(key);
        }

        public override bool IsLanguageFile(string filepath) {            
            return false;
        }

        protected override bool TryGetData(string key, out LocalizeData data) {
            if(mEntries == null)
                GenerateEntries(baseTableIndex);

            return mEntries.TryGetValue(key, out data);
        }

        protected override void HandleLanguageChanged() {
            int tableIndex = mLanguageTableIndices[languageIndex];

            //check to see if there is a platform specific, check via platform's table dependency
            var curPlatform = Application.platform;
            List<int> platformTableIndices;
            if(mPlatformTableIndices.TryGetValue(curPlatform, out platformTableIndices)) {
                var curLanguage = language;

                for(int i = 0; i < platformTableIndices.Count; i++) {
                    var platformTableIndex = platformTableIndices[i];
                    if(tables[platformTableIndex].languageDependency == curLanguage) {
                        tableIndex = platformTableIndex;
                        break;
                    }
                }
            }

            GenerateEntries(tableIndex);
        }

        protected override string[] HandleGetKeys() {
            if(tables == null || tables.Length == 0)
                return new string[0];

            var table = tables[baseTableIndex];

            var keys = new string[table.entries.Length];
            for(int i = 0; i < keys.Length; i++)
                keys[i] = table.entries[i].key;

            return keys;
        }

        protected override void HandleLoad() {
            //load base entries
            GenerateEntries(baseTableIndex);
        }

        private Dictionary<string, LocalizeData> GenerateEntryDict(TableData table) {
            Dictionary<string, LocalizeData> ret;

            if(!string.IsNullOrEmpty(table.languageDependency)) {
                //create initial list of entries from dependent language, then overwrite any new changes
                int dependentLanguageIndex = GetLanguageIndex(table.languageDependency);
                int dependentTableIndex = mLanguageTableIndices[dependentLanguageIndex];

                ret = GenerateEntryDict(tables[dependentTableIndex]);

                for(int i = 0; i < table.entries.Length; i++) {
                    var tableEntry = table.entries[i];

                    LocalizeData entry;
                    if(ret.TryGetValue(tableEntry.key, out entry)) {
                        entry.text = tableEntry.text;

                        if(tableEntry.param != null && tableEntry.param.Length > 0)
                            entry.param = tableEntry.param;

                        ret[tableEntry.key] = entry;
                    }
                    else
                        ret.Add(tableEntry.key, new LocalizeData() { text = tableEntry.text, param = tableEntry.param });
                }
            }
            else {
                ret = new Dictionary<string, LocalizeData>();

                for(int i = 0; i < table.entries.Length; i++) {
                    var entry = table.entries[i];
                    ret.Add(entry.key, new LocalizeData() { text=entry.text, param=entry.param });
                }
            }

            return ret;
        }

        private void GenerateEntries(int tableIndex) {
            if(tables != null && tables.Length > 0)
                mEntries = GenerateEntryDict(tables[tableIndex]);
            else
                mEntries = new Dictionary<string, LocalizeData>();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            //no need to serialize anything
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            List<string> languageTableNameList = new List<string>();
            List<int> languageTableIndexList = new List<int>();

            mPlatformTableIndices = new Dictionary<RuntimePlatform, List<int>>();

            for(int i = 0; i < tables.Length; i++) {
                var table = tables[i];

                switch(table.entryType) {
                    case TableEntryType.Language:
                        languageTableNameList.Add(table.name);
                        languageTableIndexList.Add(i);
                        break;
                    case TableEntryType.Platform:
                        List<int> tableIndexList;
                        if(!mPlatformTableIndices.TryGetValue(table.platformDependency, out tableIndexList)) {
                            tableIndexList = new List<int>();
                            mPlatformTableIndices.Add(table.platformDependency, tableIndexList);
                        }

                        tableIndexList.Add(i);
                        break;
                }
            }

            mLanguageTableNames = languageTableNameList.ToArray();
            mLanguageTableIndices = languageTableIndexList.ToArray();
        }
    }
}
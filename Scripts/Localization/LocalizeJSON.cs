using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Generate entries via JSON format (Using JSONUtility), you'll need to call Load(string language, string json)
    /// Example JSON:
    /// { items:[ { key="key", value="value" }, ... ] }
    /// </summary>
    [CreateAssetMenu(fileName = "localize", menuName = "M8/Localize/JSON")]
    public class LocalizeJSON : Localize {
        [System.Serializable]
        public class Entry {
            public string key;
            public string text = "";
            public string[] param = null; //set this to null if you want to reference param from base
        }

        public override string[] languages { get { return new string[] { mLanguage }; } }

        public override int languageCount { get { return 1; } }
                
        private string mLanguage;
        private string mJSON;

        private Dictionary<string, LocalizeData> mEntries;

        public void Load(string language, string json) {
            mLanguage = language;
            mJSON = json;
            mEntries = GenerateEntries(json);
        }

        public override int GetLanguageIndex(string lang) {
            return lang == mLanguage ? 0 : -1;
        }

        public override string GetLanguageName(int langInd) {
            return langInd == 0 ? mLanguage : "";
        }

        public override bool Exists(string key) {
            if(mEntries == null) {
                if(!string.IsNullOrEmpty(mJSON)) //Load(language, json) needs to be called
                    mEntries = GenerateEntries(mJSON);
                else { Debug.LogWarning("No JSON specified. You need to call Load(language, json)."); return false; }                    
            }

            return mEntries.ContainsKey(key);
        }

        protected override void HandleLanguageChanged() { }

        protected override string[] HandleGetKeys() {
            if(mEntries == null) {
                if(!string.IsNullOrEmpty(mJSON)) //Load(language, json) needs to be called
                    mEntries = GenerateEntries(mJSON);
                else { Debug.LogWarning("No JSON specified. You need to call Load(language, json)."); return new string[0]; }
            }

            string[] keys = new string[mEntries.Count];
            mEntries.Keys.CopyTo(keys, 0);

            return keys;
        }

        protected override void HandleLoad() {
            if(!string.IsNullOrEmpty(mJSON))
                mEntries = GenerateEntries(mJSON);
        }

        protected override bool TryGetData(string key, out LocalizeData data) {
            if(mEntries == null && !string.IsNullOrEmpty(mJSON))
                mEntries = GenerateEntries(mJSON);

            return mEntries.TryGetValue(key, out data);
        }

        protected Dictionary<string, LocalizeData> GenerateEntries(string json) {
            var entryList = JSONList<Entry>.FromJSON(json);
                        
            var entries = new Dictionary<string, LocalizeData>(entryList.Count);

            for(int i = 0; i < entryList.Count; i++) {
                var entry = entryList[i];

                entries.Add(entry.key, new LocalizeData() { text = entry.text, param = entry.param });
            }

            return entries;
        }
    }
}
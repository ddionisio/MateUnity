using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/Localize (Loader)")]
    public class LocalizeFromLoader : Localize {        
        [SerializeField]
        LocalizeLoaderBase loader;
        
        private Dictionary<string, Data> mEntries;
        
        public override string[] languages {
            get {
                string[] ret = new string[languageCount];

                for(int i = 0; i < ret.Length; i++)
                    ret[i] = loader.GetLanguageString(i);
                
                return ret;
            }
        }

        public override int languageCount {
            get {
                return loader.GetLanguageCount();
            }
        }
        
        public override int GetLanguageIndex(string lang) {
            return loader.GetLanguageIndex(lang);
        }

        public override string GetLanguageName(int langInd) {
            return loader.GetLanguageString(langInd);
        }
        
        public override bool Exists(string key) {
            return mEntries.ContainsKey(key);
        }

        public override string[] GetKeys() {
            if(loader)
                return loader.GetKeys();

            return new string[0];
        }

        protected override void OnInstanceInit() {
            GenerateEntries(true);
        }
                
        protected override bool TryGetData(string key, out Data data) {
            return mEntries.TryGetValue(key, out data);
        }

        protected override void HandleLanguageChanged() {
            GenerateEntries(false);
        }

        void GenerateEntries(bool isBase) {
            mEntries = new Dictionary<string, Data>();

            if(isBase)
                loader.LoadBase(mEntries);
            else
                loader.Load(languageIndex, mEntries);
        }
    }
}
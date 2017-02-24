using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("")]
    public abstract class LocalizeLoaderBase : ScriptableObject {
        public abstract int GetLanguageIndex(string language);
        public abstract string GetLanguageString(int languageIndex);
        public abstract int GetLanguageCount();

        /// <summary>
        /// Populate output with the root language (default)
        /// </summary>
        public abstract void LoadBase(Dictionary<string, Localize.Data> output);

        /// <summary>
        /// Populate output based on given language index
        /// </summary>
        public abstract void Load(int languageIndex, Dictionary<string, Localize.Data> output);
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/UserSettingLanguage")]
    public class UserSettingLanguage : UserSetting<UserSettingLanguage> {
        public const string languageKey = "lang";

        public string language {
            get { return Localize.instance.language; }
            set {
                int ind = Localize.instance.GetLanguageIndex(value);
                if(ind != -1)
                    languageIndex = ind;
            }
        }

        public int languageIndex {
            get { return Localize.instance.languageIndex; }
            set {
                userData.SetInt(languageKey, value);

                Localize.instance.languageIndex = value;
            }
        }

        public override void Load() {
            int langInd = userData.GetInt(languageKey);
            Localize.instance.languageIndex = langInd;
        }
    }
}
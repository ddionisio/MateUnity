using UnityEngine;
using System.Collections;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/UserSettingLanguage")]
    public class UserSettingLanguage : UserSetting<UserSettingLanguage> {
        public const string languageKey = "lang";

        public Language language {
            get { return Localize.instance.language; }
            set {
                userData.SetInt(languageKey, (int)value);

                Localize.instance.language = value;
            }
        }

        protected override void OnInstanceInit() {
            base.OnInstanceInit();

            //load settings
            Language lang = (Language)userData.GetInt(languageKey, (int)Language.Default);
            if(lang != Language.Default)
                Localize.instance.language = lang;
        }
    }
}
using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Core/UserSettingLanguage")]
public class UserSettingLanguage : UserSetting {
    public const string languageKey = "lang";

    private static UserSettingLanguage mInstance;

    public static UserSettingLanguage instance { get { return mInstance; } }

    public GameLanguage language {
        get { return GameLocalize.instance.language; }
        set {
            userData.SetInt(languageKey, (int)value);

            GameLocalize.instance.language = value;
        }
    }

    void OnDestroy() {
        mInstance = null;
    }

    protected override void Awake() {
        mInstance = this;

        base.Awake();

        //load settings
        GameLocalize.instance.language = (GameLanguage)userData.GetInt(languageKey, (int)GameLanguage.English);
    }
}

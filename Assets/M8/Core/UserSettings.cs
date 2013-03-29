using UnityEngine;
using System.Collections;

public class UserSettings {
    public const string volumeKey = "v";
    public const string soundEnableKey = "snd";
    public const string musicEnableKey = "mus";
    public const string languageKey = "l";

    public bool isSoundEnable {
        get { return mSoundEnabled; }

        set {
            if(mSoundEnabled != value) {
                mSoundEnabled = value;
                PlayerPrefs.SetInt(soundEnableKey, mSoundEnabled ? 1 : 0);

                RelaySettingsChanged();
            }
        }
    }

    public bool isMusicEnable {
        get { return mMusicEnabled; }

        set {
            if(mMusicEnabled != value) {
                mMusicEnabled = value;
                PlayerPrefs.SetInt(musicEnableKey, mMusicEnabled ? 1 : 0);

                RelaySettingsChanged();
            }
        }
    }

    public float volume {
        get { return mVolume; }

        set {
            if(mVolume != value) {
                mVolume = value;
                PlayerPrefs.SetFloat(volumeKey, mVolume);

                AudioListener.volume = mVolume;
            }
        }
    }

    public GameLanguage language {
        get { return mLanguage; }
        set {
            if(mLanguage != value) {
                mLanguage = value;
                PlayerPrefs.SetInt(languageKey, (int)mLanguage);
            }
        }
    }

    //need to debug while listening to music
#if UNITY_EDITOR
    private const int enableDefault = 1;
#else
	private const int enableDefault = 1;
#endif

    private float mVolume;
    private bool mSoundEnabled;
    private bool mMusicEnabled;
    private GameLanguage mLanguage = GameLanguage.English;

    // Use this for initialization
    public UserSettings() {
        //load settings
        mVolume = PlayerPrefs.GetFloat(volumeKey, 1.0f);

        mSoundEnabled = PlayerPrefs.GetInt(soundEnableKey, enableDefault) > 0;

        mMusicEnabled = PlayerPrefs.GetInt(musicEnableKey, enableDefault) > 0;

        AudioListener.volume = mVolume;

        mLanguage = (GameLanguage)PlayerPrefs.GetInt(languageKey, (int)GameLanguage.English);
    }

    private void RelaySettingsChanged() {
        SceneManager.RootBroadcastMessage("UserSettingsChanged", this, SendMessageOptions.DontRequireReceiver);
    }
}

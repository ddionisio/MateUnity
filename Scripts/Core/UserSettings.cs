using UnityEngine;
using System.Collections;

public class UserSettings {
    public const string volumeKey = "v";
    public const string soundKey = "snd";
    public const string musicKey = "mus";
    public const string languageKey = "l";

    public float soundVolume {
        get { return mSoundVolume; }

        set {
            if(mSoundVolume != value) {
                mSoundVolume = value;
                PlayerPrefs.SetFloat(soundKey, mSoundVolume);

                RelaySettingsChanged();
            }
        }
    }

    public float musicVolume {
        get { return mMusicVolume; }

        set {
            if(mMusicVolume != value) {
                mMusicVolume = value;
                PlayerPrefs.SetFloat(musicKey, mMusicVolume);

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
    private const float volumeDefault = 1.0f;
#else
	private const float volumeDefault = 1.0f;
#endif

    private float mVolume;
    private float mSoundVolume;
    private float mMusicVolume;
    private GameLanguage mLanguage = GameLanguage.English;

    // Use this for initialization
    public UserSettings() {
        //load settings
        mVolume = PlayerPrefs.GetFloat(volumeKey, 1.0f);

        mSoundVolume = PlayerPrefs.GetFloat(soundKey, volumeDefault);

        mMusicVolume = PlayerPrefs.GetFloat(musicKey, volumeDefault);

        AudioListener.volume = mVolume;

        mLanguage = (GameLanguage)PlayerPrefs.GetInt(languageKey, (int)GameLanguage.English);
    }

    public void Save() {
        PlayerPrefs.SetFloat(volumeKey, mVolume);
        PlayerPrefs.SetFloat(soundKey, mSoundVolume);
        PlayerPrefs.SetFloat(musicKey, mMusicVolume);
        PlayerPrefs.SetInt(languageKey, (int)mLanguage);
    }

    private void RelaySettingsChanged() {
        SceneManager.RootBroadcastMessage("UserSettingsChanged", this, SendMessageOptions.DontRequireReceiver);
    }
}

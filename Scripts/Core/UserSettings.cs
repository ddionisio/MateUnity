using UnityEngine;
using System.Collections;

public class UserSettings {
    public const string volumeKey = "v";
    public const string soundKey = "snd";
    public const string musicKey = "mus";
    public const string languageKey = "l";
    public const string fullscreenKey = "f";
    public const string screenWidthKey = "sw";
    public const string screenHeightKey = "sh";
    public const string screenRefreshRateKey = "sr";

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

    public int screenWidth {
        get {
            return mScreenWidth;
        }
    }

    public int screenHeight {
        get {
            return mScreenHeight;
        }
    }

    public int screenRefreshRate {
        get {
            return mScreenRefreshRate;
        }
    }

    public bool fullscreen {
        get {
            return mFullscreen;
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
    private int mScreenWidth;
    private int mScreenHeight;
    private int mScreenRefreshRate;
    private bool mFullscreen;
    private GameLanguage mLanguage = GameLanguage.English;

    // Use this for initialization
    public UserSettings(int defaultScreenWidth, int defaultScreenHeight, int defaultRefreshRate, bool defaultFullscreen) {
        //load settings
        mVolume = PlayerPrefs.GetFloat(volumeKey, 1.0f);

        mSoundVolume = PlayerPrefs.GetFloat(soundKey, volumeDefault);

        mMusicVolume = PlayerPrefs.GetFloat(musicKey, volumeDefault);

        AudioListener.volume = mVolume;

        mScreenWidth = PlayerPrefs.GetInt(screenWidthKey, defaultScreenWidth);
        mScreenHeight = PlayerPrefs.GetInt(screenHeightKey, defaultScreenHeight);
        mScreenRefreshRate = PlayerPrefs.GetInt(screenRefreshRateKey, defaultRefreshRate);
        mFullscreen = PlayerPrefs.GetInt(fullscreenKey, defaultFullscreen ? 1 : 0) > 0;

        mLanguage = (GameLanguage)PlayerPrefs.GetInt(languageKey, (int)GameLanguage.English);
    }

    public void Save() {
        PlayerPrefs.SetFloat(volumeKey, mVolume);
        PlayerPrefs.SetFloat(soundKey, mSoundVolume);
        PlayerPrefs.SetFloat(musicKey, mMusicVolume);
        PlayerPrefs.SetInt(languageKey, (int)mLanguage);
        PlayerPrefs.SetInt(fullscreenKey, mFullscreen ? 1 : 0);
        PlayerPrefs.SetInt(screenWidthKey, mScreenWidth);
        PlayerPrefs.SetInt(screenHeightKey, mScreenHeight);
        PlayerPrefs.SetInt(screenRefreshRateKey, mScreenRefreshRate);
    }

    public void ApplyResolution(int width, int height, int refreshRate, bool fullscreen) {
        mScreenWidth = width;
        mScreenHeight = height;
        mScreenRefreshRate = refreshRate;
        mFullscreen = fullscreen;
        ApplyResolution();

        RelaySettingsChanged();
    }

    public void ApplyResolution() {
        Screen.SetResolution(mScreenWidth, mScreenHeight, mFullscreen, mScreenRefreshRate);
    }

    private void RelaySettingsChanged() {
        SceneManager.RootBroadcastMessage("UserSettingsChanged", this, SendMessageOptions.DontRequireReceiver);
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Core/UserSettingScreen")]
public class UserSettingScreen : UserSetting {
    public const string fullscreenKey = "fullscreen";
    public const string screenWidthKey = "screenW";
    public const string screenHeightKey = "screenH";
    public const string screenRefreshRateKey = "screenRefresh";

    public int defaultWidth;
    public int defaultHeight;
    public int defaultRefreshRate;
    public bool defaultFullscreen;

    private static UserSettingScreen mInstance;

    private int mScreenWidth;
    private int mScreenHeight;
    private int mScreenRefreshRate;
    private bool mFullscreen;

    public static UserSettingScreen instance { get { return mInstance; } }

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

    public void ApplyResolution(int width, int height, int refreshRate, bool fullscreen) {
        mScreenWidth = width;
        mScreenHeight = height;
        mScreenRefreshRate = refreshRate;
        mFullscreen = fullscreen;

        //save if desktop
#if UNITY_STANDALONE
        userData.SetInt(fullscreenKey, mFullscreen ? 1 : 0);
#endif

        userData.SetInt(screenWidthKey, mScreenWidth);
        userData.SetInt(screenHeightKey, mScreenHeight);
        userData.SetInt(screenRefreshRateKey, mScreenRefreshRate);

        ApplyResolution();

        RelaySettingsChanged();
    }

    public void ApplyResolution() {
        Screen.SetResolution(mScreenWidth, mScreenHeight, mFullscreen, mScreenRefreshRate);
    }

    void OnDestroy() {
        mInstance = null;
    }

    protected override void Awake() {
        mInstance = this;

        base.Awake();

        mScreenWidth = userData.GetInt(screenWidthKey, defaultWidth);
        mScreenHeight = userData.GetInt(screenHeightKey, defaultHeight);
        mScreenRefreshRate = userData.GetInt(screenRefreshRateKey, defaultRefreshRate);
        mFullscreen = userData.GetInt(fullscreenKey, defaultFullscreen ? 1 : 0) > 0;

        //only apply for standalone
#if UNITY_STANDALONE
        ApplyResolution();
#endif
    }
}

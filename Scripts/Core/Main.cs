using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/Main")]
public class Main : MonoBehaviour {
    [System.Serializable]
    public class Resolution {
        public int width;
        public int height;
        public int refreshRate;
        public bool fullscreen;
    }

    [SerializeField]
    string _startScene = "start"; //the scene to load to once initScene is finish
    [SerializeField]
    string _mainScene = "main"; //load start scene after initialization, assumes we are in scene index 0

    [SerializeField]
    bool _setResolution;
    [SerializeField]
    Resolution _resolution;
    [SerializeField]
    bool _fullScreenNoSave;
    [SerializeField]
    GamePlatform _platform = GamePlatform.Web;

    [SerializeField]
    UserSettings _userSettings;
            
    private static Main mInstance = null;

    public static GamePlatform platform { get { return mInstance ? mInstance._platform : GamePlatform.NumPlatforms; } }
    public static UserSettings userSettings { get { return mInstance ? mInstance._userSettings : null; } }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;

            _userSettings.DeInit();
        }
    }

    void OnDisable() {
        //Debug.Log("main disable");
        PlayerPrefs.Save();
    }

    void Awake() {
        //already one exists...
        if(mInstance != null) {
            DestroyImmediate(gameObject);
            return;
        }

        mInstance = this;

        DontDestroyOnLoad(gameObject);

        if(_setResolution) {
            _userSettings = new UserSettings(_resolution.width, _resolution.height, _resolution.refreshRate, _resolution.fullscreen);
        }
        else {
            UnityEngine.Resolution r = Screen.currentResolution;
            _userSettings = new UserSettings(r.width, r.height, r.refreshRate, Screen.fullScreen);
        }

        _userSettings.fullScreenNoSave = _fullScreenNoSave;
    }

    IEnumerator Start() {
        if(_setResolution) {
            userSettings.ApplyResolution();
        }
                
        //go to start if we are in main scene
        if(Application.loadedLevelName == _mainScene && !string.IsNullOrEmpty(_startScene)) {
            //wait for one frame to ensure all initialization has occured during main
            yield return new WaitForFixedUpdate();

            SceneManager.instance.LoadScene(_startScene);
        }
    }
}

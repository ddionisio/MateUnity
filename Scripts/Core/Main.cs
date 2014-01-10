using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/Main")]
public class Main : MonoBehaviour {
    public GamePlatform platform = GamePlatform.Default;

    public string initScene = "main"; //initial scene where the main initializes, goes to startScene afterwards
    public string startScene = "start"; //the scene to load to once initScene is finish

    [System.NonSerialized]
    public UserSettings userSettings;
    [System.NonSerialized]
    public SceneManager sceneManager;
    [System.NonSerialized]
    public InputManager input;
    [System.NonSerialized]
    public GameLocalize localizer;

    private static Main mInstance = null;

    public static Main instance {
        get {
            return mInstance;
        }
    }

    public SceneController sceneController {
        get {
            return sceneManager.sceneController;
        }
    }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;
    }

    void OnEnable() {
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

        userSettings = new UserSettings();

        sceneManager = GetComponentInChildren<SceneManager>();

        input = GetComponentInChildren<InputManager>();

        //load the string table
        localizer = GetComponentInChildren<GameLocalize>();
        if(localizer != null) {
            localizer.Load(userSettings.language, platform);
        }
    }

    void Start() {
        //TODO: maybe do other things before starting the game
        //go to start if we are in main scene
        if(Application.loadedLevelName == initScene) {
            sceneManager.LoadScene(startScene);
        }
        else {
            sceneManager.InitScene();
        }

    }
}

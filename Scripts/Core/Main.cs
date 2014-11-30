using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/Main")]
public class Main : MonoBehaviour {
    [SerializeField]
    string _startScene = "start"; //the scene to load to once initScene is finish
    [SerializeField]
    string _mainScene = "main"; //load start scene after initialization, assumes we are in scene index 0

    [SerializeField]
    GamePlatform _platform = GamePlatform.Web;

    private static Main mInstance = null;

    public static GamePlatform platform { get { return mInstance ? mInstance._platform : GamePlatform.NumPlatforms; } }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;
        }
    }

    void Awake() {
        //already one exists...
        if(mInstance != null) {
            DestroyImmediate(gameObject);
            return;
        }

        mInstance = this;

        DontDestroyOnLoad(gameObject);
    }

    IEnumerator Start() {
        //go to start if we are in main scene
        if(Application.loadedLevelName == _mainScene && !string.IsNullOrEmpty(_startScene)) {
            //wait for one frame to ensure all initialization has occured during main
            yield return null;

            SceneManager.instance.LoadSceneNoTransition(_startScene);
        }
    }
}

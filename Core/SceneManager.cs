using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/SceneManager")]
public class SceneManager : MonoBehaviour {

    public const string levelString = "level";

    public ScreenTransition screenTransition;

    private SceneController mSceneController;
    private string mCurSceneStr;
    private int mCurLevel;
    private float mPrevTimeScale;

    private SceneCheckpoint mCheckPoint = null;
    private string mCheckPointForScene = "";

    private bool mFirstTime = true;

    private string mSceneToLoad = null;

    private static List<Transform> mRoots = new List<Transform>();

    private bool mIsFullscreen = false;

    public int curLevel {
        get {
            return mCurLevel;
        }
    }

    public SceneController sceneController {
        get {
            return mSceneController;
        }
    }

    public static void RootBroadcastMessage(string methodName, object param, SendMessageOptions options) {
        if(mRoots.Count == 0) {
            //refresh roots
            Transform[] trans = (Transform[])FindObjectsOfType(typeof(Transform));
            foreach(Transform tran in trans) {
                if(tran.parent == null) {
                    mRoots.Add(tran);
                }
            }
        }

        foreach(Transform t in mRoots) {
            if(t != null)
                t.BroadcastMessage(methodName, param, options);    
        }
    }

    public void SetCheckPoint(SceneCheckpoint check) {
        mCheckPointForScene = mCurSceneStr;
        mCheckPoint = check;
    }

    public void LoadScene(string scene) {
        mSceneToLoad = scene;

        if(mFirstTime) {
            screenTransition.state = ScreenTransition.State.Done;
            DoLoad();
        }
        else {
            screenTransition.state = ScreenTransition.State.Out;
        }
    }

    public void LoadLevel(int level) {
        mCurLevel = level;
        LoadScene(levelString + level);
    }

    public void Reload() {
        if(!string.IsNullOrEmpty(mCurSceneStr)) {
            LoadScene(mCurSceneStr);
        }
    }

    public void Pause() {
        if(Time.timeScale != 0.0f) {
            mPrevTimeScale = Time.timeScale;
            Time.timeScale = 0.0f;
        }

        RootBroadcastMessage("OnScenePause", null, SendMessageOptions.DontRequireReceiver);
    }

    public void Resume() {
        Time.timeScale = mPrevTimeScale;

        RootBroadcastMessage("OnSceneResume", null, SendMessageOptions.DontRequireReceiver);
    }

    /// <summary>
    /// Internal use only. Called at OnLevelWasLoaded in SceneManager or in Main (for debug/dev)
    /// </summary>
    public void InitScene() {
        if(mSceneController == null) {
            mCurSceneStr = Application.loadedLevelName;
            mSceneController = (SceneController)Object.FindObjectOfType(typeof(SceneController));
        }
    }

    void OnLevelWasLoaded(int sceneInd) {
        mRoots.Clear();

        InitScene();

        if(mCheckPoint != null && mCurSceneStr == mCheckPointForScene) {
            mSceneController.OnCheckPoint(mCheckPoint);
        }

        mCheckPoint = null;
        mCheckPointForScene = "";

        if(screenTransition.state == ScreenTransition.State.Done) { //we were at out a while back
            mFirstTime = false;
            StartCoroutine(DoScreenTransitionIn());
        }
    }

    void OnDestroy() {
    }

    void Awake() {
        mIsFullscreen = Screen.fullScreen;

        mPrevTimeScale = Time.timeScale;

        screenTransition.finishCallback = OnScreenTransitionFinish;
    }

    void Update() {
        //lame
        //check resolution and fullscreen
        if(mIsFullscreen != Screen.fullScreen) {
            mIsFullscreen = Screen.fullScreen;
            RootBroadcastMessage("OnSceneScreenChanged", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnScreenTransitionFinish(ScreenTransition.State state) {
        switch(state) {
            case ScreenTransition.State.In:
                //notify?
                break;

            case ScreenTransition.State.Out:
                DoLoad();
                break;
        }
    }

    void DoLoad() {
        RootBroadcastMessage("SceneChange", mSceneToLoad, SendMessageOptions.DontRequireReceiver);

        mCurSceneStr = mSceneToLoad;

        Application.LoadLevel(mSceneToLoad);
    }

    IEnumerator DoScreenTransitionIn() {
        yield return new WaitForFixedUpdate();

        screenTransition.state = ScreenTransition.State.In;

        yield break;
    }
}

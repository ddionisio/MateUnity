using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/SceneManager")]
public class SceneManager : MonoBehaviour {

    public const string levelString = "level";
    public const int stackCapacity = 8;

    public ScreenTransition screenTransition;

    [SerializeField]
    bool stackEnable = false;

    private SceneController mSceneController;
    
    private string mCurSceneStr;

    private int mCurLevel;

    private Stack<string> mSceneStack;

    private float mPrevTimeScale;

    private SceneCheckpoint mCheckPoint = null;
    private string mCheckPointForScene = "";

    private bool mFirstTime = true;

    private string mSceneToLoad = null;

    private static List<Transform> mRoots = new List<Transform>();

    //private bool mIsFullscreen = false;
    private bool mPaused = false;

    public bool isPaused {
        get {
            return mPaused;
        }
    }

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

    void _LoadScene(string scene) {
        //make sure the scene is not paused
        Resume();

        mSceneToLoad = scene;

        if(mFirstTime) {
            screenTransition.state = ScreenTransition.State.Done;
            DoLoad();
        }
        else {
            screenTransition.state = ScreenTransition.State.Out;
        }
    }

    public void LoadScene(string scene) {
        SceneStackPush(mCurSceneStr, scene);

        _LoadScene(scene);
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

    /// <summary>
    /// Angry if stack is not enabled
    /// </summary>
    public void LoadLastSceneStack() {
        if(stackEnable) {
            if(mSceneStack.Count > 0) {
                _LoadScene(mSceneStack.Pop());
            }
            else {
                Debug.Log("No more scenes to pop!");
            }
        }
        else {
            Debug.LogError("Stack is not enabled!!!");
        }
    }

    public void ClearSceneStack() {
        if(stackEnable) {
            mSceneStack.Clear();
        }
        else {
            Debug.LogError("Stack is not enabled!!!");
        }
    }

    public void Pause() {
        if(!mPaused) {
            mPaused = true;

            if(Time.timeScale != 0.0f) {
                mPrevTimeScale = Time.timeScale;
                Time.timeScale = 0.0f;
            }

            RootBroadcastMessage("OnScenePause", null, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void Resume() {
        if(mPaused) {
            mPaused = false;

            Time.timeScale = mPrevTimeScale;

            RootBroadcastMessage("OnSceneResume", null, SendMessageOptions.DontRequireReceiver);
        }
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
        //mIsFullscreen = Screen.fullScreen;

        mPrevTimeScale = Time.timeScale;

        mPaused = false;

        if(stackEnable)
            mSceneStack = new Stack<string>(stackCapacity);

        screenTransition.finishCallback = OnScreenTransitionFinish;
    }

    /*void Update() {
        //lame
        //check resolution and fullscreen
        if(mIsFullscreen != Screen.fullScreen) {
            mIsFullscreen = Screen.fullScreen;
            RootBroadcastMessage("OnSceneScreenChanged", null, SendMessageOptions.DontRequireReceiver);
        }
    }*/

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

    private void SceneStackPush(string scene, string nextScene) {
        if(stackEnable && scene != nextScene) {
            if(mSceneStack.Count == stackCapacity) {
                //remove the oldest
                string[] stuff = mSceneStack.ToArray();
                mSceneStack.Clear();
                for(int i = stuff.Length - 2; i >= 0; i--) {
                    mSceneStack.Push(stuff[i]);
                }
            }

            //check if next scene is already in stack, then pop stuff and dont push
            int stackCount = 0;
            bool stackFound = false;
            foreach(string stackScene in mSceneStack) {
                stackCount++;

                if(stackScene == nextScene) {
                    stackFound = true;
                    break;
                }
            }

            if(stackFound) {
                for(int i = 0; i < stackCount; i++)
                    mSceneStack.Pop();
            }
            else
                mSceneStack.Push(scene);                
        }
    }
}

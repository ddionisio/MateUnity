using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/SceneManager")]
public class SceneManager : MonoBehaviour {

    public const string levelString = "level";
    public const int stackCapacity = 8;

    public delegate void OnSceneBoolCallback(bool b);
    public delegate void OnSceneStringCallback(string s);

    public event OnSceneBoolCallback pauseCallback;
    public event OnSceneStringCallback sceneChangeCallback;

    [SerializeField]
    ScreenTransition screenTransition;

    [SerializeField]
    bool stackEnable = false;

    private static SceneManager mInstance;

    private string mCurSceneStr;

    private int mCurLevel;

    private Stack<string> mSceneStack;

    private float mPrevTimeScale;

    private bool mFirstTime = true;

    private string mSceneToLoad = null;
    
    //private bool mIsFullscreen = false;
    private bool mPaused = false;

    public static SceneManager instance { get { return mInstance; } }

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

            if(pauseCallback != null) pauseCallback(mPaused);
        }
    }

    public void Resume() {
        if(mPaused) {
            mPaused = false;

            Time.timeScale = mPrevTimeScale;

            if(pauseCallback != null) pauseCallback(mPaused);
        }
    }

    void OnLevelWasLoaded(int sceneInd) {
        mCurSceneStr = Application.loadedLevelName;

        if(screenTransition.state == ScreenTransition.State.Done) { //we were at out a while back
            mFirstTime = false;
            StartCoroutine(DoScreenTransitionIn());
        }
    }

    void OnDestroy() {
        if(mInstance == this) {
            pauseCallback = null;
            sceneChangeCallback = null;
        }
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            mCurSceneStr = Application.loadedLevelName;
            //mIsFullscreen = Screen.fullScreen;

            mPrevTimeScale = Time.timeScale;

            mPaused = false;

            if(stackEnable)
                mSceneStack = new Stack<string>(stackCapacity);

            if(screenTransition)
                screenTransition.finishCallback = OnScreenTransitionFinish;
        }
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
        if(sceneChangeCallback != null) sceneChangeCallback(mSceneToLoad);

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

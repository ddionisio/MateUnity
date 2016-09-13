using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/SceneManager")]
    public class SceneManager : SingletonBehaviour<SceneManager> {
        public const string levelString = "level";
        public const int stackCapacity = 8;

        public delegate void OnSceneBoolCallback(bool b);
        public delegate void OnSceneStringCallback(string nextScene);

        public event OnSceneBoolCallback pauseCallback;
        public event OnSceneStringCallback sceneChangeCallback;

        //transitions, make sure ScreenTransitionManager is available

        [Tooltip("The Screen Transition to play when exiting the scene.  Make sure ScreenTransManager is available.")]
        [SerializeField]
        string sceneTransitionOut;
        [Tooltip("The Screen Transition to play when entering the new scene.  Make sure ScreenTransManager is available.")]
        [SerializeField]
        string sceneTransitionIn;

        [SerializeField]
        bool stackEnable = false;

        private string mCurSceneStr;

        private int mCurLevel;

        private Stack<string> mSceneStack;

        private float mPrevTimeScale;

        private bool mFirstTime = true;

        private string mSceneToLoad = null;

        private string mScreenTransOut, mScreenTransIn;

        //private bool mIsFullscreen = false;
        private bool mPaused = false;

        private Transform mHolder; //for use with storing game objects to be transferred to other scenes

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

        public string curLevelName {
            get {
                return mCurSceneStr;
            }
        }

        void _LoadScene(string scene, string transOut, string transIn) {
            //make sure the scene is not paused
            Resume();

            mSceneToLoad = scene;
            mFirstTime = false;
            mScreenTransOut = transOut;
            mScreenTransIn = transIn;

            if(mFirstTime)
                DoLoad();
            else {
                if(!string.IsNullOrEmpty(mScreenTransOut)) {
                    if(string.IsNullOrEmpty(mScreenTransIn))
                        ScreenTransManager.instance.Play(mScreenTransOut, OnScreenTransEndLoadScene);
                    else {
                        //allow transition to call prepare before loading next scene
                        ScreenTransManager.instance.Play(mScreenTransOut, null);
                        ScreenTransManager.instance.Play(mScreenTransIn, OnScreenTransBeginLoadScene);
                    }
                }
                else if(!string.IsNullOrEmpty(mScreenTransIn))
                    ScreenTransManager.instance.Play(mScreenTransIn, OnScreenTransBeginLoadScene);
                else
                    DoLoad();
            }
        }

        public void LoadSceneNoTransition(string scene) {
            SceneStackPush(mCurSceneStr, scene);
            _LoadScene(scene, null, null);
        }

        public void LoadScene(string scene) {
            SceneStackPush(mCurSceneStr, scene);
            _LoadScene(scene, sceneTransitionOut, sceneTransitionIn);
        }

        public void LoadScene(string scene, string transitionOut, string transitionIn) {
            SceneStackPush(mCurSceneStr, scene);
            _LoadScene(scene, transitionOut, transitionIn);
        }

        public void LoadLevel(int level) {
            mCurLevel = level;
            LoadScene(levelString + level);
        }

        public void LoadLevel(int level, string transitionOut, string transitionIn) {
            mCurLevel = level;
            LoadScene(levelString + level, transitionOut, transitionIn);
        }

        public void Reload() {
            if(!string.IsNullOrEmpty(mCurSceneStr)) {
                LoadScene(mCurSceneStr);
            }
        }

        public void Reload(string transitionOut, string transitionIn) {
            if(!string.IsNullOrEmpty(mCurSceneStr)) {
                LoadScene(mCurSceneStr, transitionOut, transitionIn);
            }
        }

        /// <summary>
        /// Angry if stack is not enabled
        /// </summary>
        public void LoadLastSceneStack() {
            if(stackEnable) {
                if(mSceneStack.Count > 0) {
                    _LoadScene(mSceneStack.Pop(), sceneTransitionOut, sceneTransitionIn);
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

        public void StoreAddObject(Transform t) {
            if(mHolder == null) {
                GameObject newGO = new GameObject("sceneHolder");
                mHolder = newGO.transform;
                mHolder.parent = transform;
            }

            t.parent = mHolder;
            t.gameObject.SetActive(false);
        }

        public Transform StoreGetObject(string name) {
            return mHolder ? mHolder.Find(name) : null;
        }

        public void StoreDestroyObject(string name) {
            Transform t = StoreGetObject(name);
            if(t)
                Destroy(t.gameObject);
        }
                
        protected override void OnInstanceInit() {
            mCurSceneStr = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            //mIsFullscreen = Screen.fullScreen;

            mPrevTimeScale = Time.timeScale;

            mPaused = false;

            if(stackEnable)
                mSceneStack = new Stack<string>(stackCapacity);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void OnInstanceDeinit() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void DoLoad() {
            if(sceneChangeCallback != null) sceneChangeCallback(mSceneToLoad);

            mCurSceneStr = mSceneToLoad;

            UnityEngine.SceneManagement.SceneManager.LoadScene(mSceneToLoad);
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
            if(mode == UnityEngine.SceneManagement.LoadSceneMode.Single) {
                mCurSceneStr = scene.name;
                mFirstTime = false;
            }
        }

        void OnScreenTransBeginLoadScene(ScreenTrans trans, ScreenTransManager.Action act) {
            if(act == ScreenTransManager.Action.Begin)
                DoLoad();
        }

        void OnScreenTransEndLoadScene(ScreenTrans trans, ScreenTransManager.Action act) {
            if(act == ScreenTransManager.Action.End)
                DoLoad();
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
}
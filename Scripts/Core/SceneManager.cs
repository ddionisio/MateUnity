using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/SceneManager")]
    public class SceneManager : SingletonBehaviour<SceneManager> {
        public enum Mode {
            Single,
            Additive
        }

        public interface ITransition {
            /// <summary>
            /// Higher priority executes first.
            /// </summary>
            int priority { get; }

            IEnumerator Out();
            IEnumerator In();            
        }

        public const int stackCapacity = 8;
        
        public delegate void OnSceneCallback();
        public delegate void OnSceneBoolCallback(bool b);
        public delegate void OnSceneStringCallback(string nextScene);
        public delegate void OnSceneDataCallback(Scene scene);

        /// <summary>
        /// Whenever Pause/Resume is called
        /// </summary>
        public event OnSceneBoolCallback pauseCallback;

        /// <summary>
        /// Called after transition (e.g. load screen, save states); and before unloading current scene, and then load new scene.
        /// </summary>
        public event OnSceneStringCallback sceneChangeCallback;

        /// <summary>
        /// Called after new scene is loaded, before transition in
        /// </summary>
        public event OnSceneCallback sceneChangePostCallback;

        /// <summary>
        /// Called after a new scene is added via AddScene
        /// </summary>
        public event OnSceneDataCallback sceneAddedCallback;

        [Tooltip("Use additive if you want to start the game with a root scene, and having one scene to append/replace as you load new scene.")]
        [SerializeField]
        Mode _mode = Mode.Additive;
        
        [Tooltip("Prefix for loading level by index.")]
        [SerializeField]
        string levelString = "level";
        
        [SerializeField]
        bool stackEnable = false; //TODO: refactor how stacking works
        
        private Scene mCurScene;
        private Scene mRootScene; //used for additive mode
        
        private Stack<string> mSceneStack;

        private List<ITransition> mTransitions;

        private List<Scene> mScenesAdded; //added scene to current one, filled by AddScene(), cleared out when a new scene is loaded via LoadScene
        private Queue<string> mScenesToAdd;

        private Coroutine mSceneAddRout;

        private float mPrevTimeScale;
        
        private Transform mHolder; //for use with storing game objects to be transferred to other scenes

        public Mode mode {
            get { return _mode; }
            set {
                if(isLoading) {
                    Debug.LogError("Current scene is still loading, can't set mode.");
                    return;
                }

                _mode = value;
            }
        }
        
        public bool isPaused { get; private set; }

        public int curLevel { get; private set; }

        public Scene curScene { get { return mCurScene; } }

        public bool isLoading { get; private set; }
                        
        public void LoadScene(string scene) {
            //can't allow this
            if(isLoading) {
                Debug.LogError("Current scene is still loading, can't load: "+scene);
                return;
            }

            SceneStackPush(mCurScene.name, scene);

            LoadSceneMode loadMode = LoadSceneMode.Single;
            bool unloadCurrent = false;

            //check if we are loading additive
            switch(mode) {
                case Mode.Additive:
                    loadMode = LoadSceneMode.Additive;
                    unloadCurrent = true;
                    break;
            }
            
            StartCoroutine(DoLoadScene(scene, loadMode, unloadCurrent));
        }
        
        public void LoadLevel(int level) {
            curLevel = level;
            LoadScene(levelString + level);
        }

        /// <summary>
        /// Load back root.  This can be used to reset the entire game.
        /// </summary>
        public void LoadRoot() {
            LoadScene(mRootScene.name);
        }
        
        public void Reload() {
            if(!string.IsNullOrEmpty(mCurScene.name)) {
                LoadScene(mCurScene.name);
            }
        }
        
        /// <summary>
        /// Angry if stack is not enabled
        /// </summary>
        public void LoadLastSceneStack() {
            if(stackEnable) {
                if(mSceneStack.Count > 0) {
                    LoadScene(mSceneStack.Pop());
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

        /// <summary>
        /// Add a scene to the current scene. Listen via sceneAddedCallback to know when it's added
        /// </summary>
        public void AddScene(string sceneName) {
            mScenesToAdd.Enqueue(sceneName);

            if(mSceneAddRout == null)
                mSceneAddRout = StartCoroutine(DoAddScene());
        }

        /// <summary>
        /// Unload a scene loaded via AddScene
        /// </summary>
        public void UnloadAddedScene(string sceneName) {
            for(int i = 0; i < mScenesAdded.Count; i++) {
                if(mScenesAdded[i].name == sceneName) {
                    mScenesAdded.RemoveAt(i);
                    UnitySceneManager.UnloadScene(sceneName);
                    return;
                }
            }

            //check the queue
            if(mScenesToAdd.Contains(sceneName)) {
                //reconstruct the queue excluding the sceneName
                var newSceneQueue = new Queue<string>();
                while(mScenesToAdd.Count > 0) {
                    var s = mScenesToAdd.Dequeue();
                    if(s != sceneName)
                        newSceneQueue.Enqueue(s);
                }
                mScenesToAdd = newSceneQueue;
            }
        }

        /// <summary>
        /// Unloads all added scenes loaded via AddScene
        /// </summary>
        public void UnloadAddedScenes() {
            for(int i = 0; i < mScenesAdded.Count; i++)
                UnitySceneManager.UnloadScene(mScenesAdded[i]);

            ClearAddSceneData();
        }

        private void ClearAddSceneData() {
            mScenesAdded.Clear();
            mScenesToAdd.Clear();

            if(mSceneAddRout != null) {
                StopCoroutine(mSceneAddRout);
                mSceneAddRout = null;
            }
        }

        public void Pause() {
            if(!isPaused) {
                isPaused = true;

                if(Time.timeScale != 0.0f) {
                    mPrevTimeScale = Time.timeScale;
                    Time.timeScale = 0.0f;
                }

                if(pauseCallback != null) pauseCallback(isPaused);
            }
        }

        public void Resume() {
            if(isPaused) {
                isPaused = false;

                Time.timeScale = mPrevTimeScale;

                if(pauseCallback != null) pauseCallback(isPaused);
            }
        }

        private void GenerateHolder() {
            if(mHolder == null) {
                const string holderName = "M8.SceneHolder";
                GameObject newGO = GameObject.Find(holderName);
                if(newGO == null)
                    newGO = new GameObject(holderName);
                DontDestroyOnLoad(newGO);
                mHolder = newGO.transform;
            }
        }

        public void StoreAddObject(Transform t) {
            GenerateHolder();

            t.parent = mHolder;
            t.gameObject.SetActive(false);
        }

        public Transform StoreGetObject(string name) {
            GenerateHolder();
            return mHolder.Find(name);
        }

        public void StoreDestroyObject(string name) {
            Transform t = StoreGetObject(name);
            if(t)
                Destroy(t.gameObject);
        }
        
        public void AddTransition(ITransition trans) {
            //only add if it doesn't exist
            if(!mTransitions.Contains(trans)) {
                if(mTransitions.Count > 0) {
                    for(int i = mTransitions.Count - 1; i >= 0; i--) {
                        if(mTransitions[i].priority >= trans.priority) {
                            mTransitions.Insert(i, trans);
                            break;
                        }
                    }
                }
                else
                    mTransitions.Add(trans);
            }
        }

        public void RemoveTransition(ITransition trans) {
            mTransitions.Remove(trans);
        }
        
        protected override void OnInstanceInit() {
            mRootScene = mCurScene = UnitySceneManager.GetActiveScene();
            //mIsFullscreen = Screen.fullScreen;

            mPrevTimeScale = Time.timeScale;

            isPaused = false;

            mTransitions = new List<ITransition>();
                        
            if(stackEnable)
                mSceneStack = new Stack<string>(stackCapacity);

            mScenesAdded = new List<Scene>();
            mScenesToAdd = new Queue<string>();
        }

        IEnumerator DoLoadScene(string toScene, LoadSceneMode mode, bool unloadCurrent) {
            isLoading = true;

            //make sure the scene is not paused
            Resume();

            //play out transitions
            for(int i = 0; i < mTransitions.Count; i++)
                yield return mTransitions[i].Out();
            
            //scene is about to change
            if(sceneChangeCallback != null)
                sceneChangeCallback(toScene);

            bool doLoad = true;

            if(mode == LoadSceneMode.Additive) {
                //Debug.Log("unload: "+mCurScene);
                if(unloadCurrent && mCurScene != mRootScene) {
                    UnitySceneManager.UnloadScene(mCurScene);

                    //unload added scenes
                    UnloadAddedScenes();
                }
                
                //load only if it doesn't exist
                doLoad = !UnitySceneManager.GetSceneByName(toScene).IsValid();
            }
            else {
                //single mode removes all other scenes
                ClearAddSceneData();
            }

            //load
            if(doLoad) {
                var sync = UnitySceneManager.LoadSceneAsync(toScene, mode);

                while(!sync.isDone)
                    yield return null;
            }
            else {
                yield return null;
            }

            mCurScene = UnitySceneManager.GetSceneByName(toScene);
            UnitySceneManager.SetActiveScene(mCurScene);

            if(sceneChangePostCallback != null)
                sceneChangePostCallback();

            //play in transitions
            for(int i = 0; i < mTransitions.Count; i++)
                yield return mTransitions[i].In();

            isLoading = false;
        }

        IEnumerator DoAddScene() {
            while(mScenesToAdd.Count > 0) {
                var sceneName = mScenesToAdd.Dequeue();

                var sync = UnitySceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                while(!sync.isDone)
                    yield return null;

                var sceneAdded = UnitySceneManager.GetSceneByName(sceneName);
                mScenesAdded.Add(sceneAdded);

                if(sceneAddedCallback != null)
                    sceneAddedCallback(sceneAdded);
            }

            mSceneAddRout = null;
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
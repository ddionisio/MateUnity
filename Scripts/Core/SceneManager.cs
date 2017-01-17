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
        /// Called just before the transition to change scene, useful for things like locking input, cancelling processes, etc.
        /// </summary>
        public event OnSceneCallback sceneChangeStartCallback;

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

        /// <summary>
        /// Called after a scene has unloaded via UnloadAddedScene
        /// </summary>
        public event OnSceneDataCallback sceneRemovedCallback;

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

        private Queue<Scene> mScenesToRemove; //scenes to remove via UnloadAddedScene
        private Coroutine mSceneRemoveRout;

        private float mPrevTimeScale;
        
        private Transform mHolder; //for use with storing game objects to be transferred to other scenes

        private int mPauseCounter;

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
        
        public bool isPaused { get { return mPauseCounter > 0; } }

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
                    //can't allow this
                    if(isLoading) {
                        Debug.LogError("Current scene is still loading, can't load: "+mSceneStack.Peek());
                        return;
                    }

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
            //can't allow this
            if(isLoading) {
                Debug.LogError("Current scene is still loading, can't add: "+sceneName);
                return;
            }

            mScenesToAdd.Enqueue(sceneName);

            if(mSceneAddRout == null)
                mSceneAddRout = StartCoroutine(DoAddScene());
        }

        /// <summary>
        /// Unload a scene loaded via AddScene
        /// </summary>
        public void UnloadAddedScene(string sceneName) {
            //can't allow this
            if(isLoading) {
                Debug.LogError("Current scene is still loading, can't remove: "+sceneName);
                return;
            }

            for(int i = 0; i < mScenesAdded.Count; i++) {
                if(mScenesAdded[i].name == sceneName) {
                    mScenesToRemove.Enqueue(mScenesAdded[i]);

                    mScenesAdded.RemoveAt(i);

                    if(mSceneRemoveRout == null)
                        mSceneRemoveRout = StartCoroutine(DoRemoveScene());

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
                mScenesToRemove.Enqueue(mScenesAdded[i]);

            ClearAddSceneData();

            if(mSceneRemoveRout == null)
                mSceneRemoveRout = StartCoroutine(DoRemoveScene());
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
            bool wasPaused = isPaused;

            mPauseCounter++;

            if(!wasPaused) {
                if(Time.timeScale != 0.0f) {
                    mPrevTimeScale = Time.timeScale;
                    Time.timeScale = 0.0f;
                }

                if(pauseCallback != null) pauseCallback(isPaused);
            }
        }

        public void Resume() {
            bool wasPaused = isPaused;

            mPauseCounter--;

            if(wasPaused) {
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
                bool isAdded = false;

                for(int i = 0; i < mTransitions.Count; i++) {
                    if(trans.priority > mTransitions[i].priority) {
                        mTransitions.Insert(i, trans);
                        isAdded = true;
                        break;
                    }
                }

                if(!isAdded)
                    mTransitions.Add(trans);
            }
        }

        public void RemoveTransition(ITransition trans) {
            mTransitions.Remove(trans);
        }

        protected override void OnInstanceDeinit() {
            //if we are destroying SceneManager for some reason
            if(isPaused)
                Time.timeScale = mPrevTimeScale;
        }

        protected override void OnInstanceInit() {
            mRootScene = mCurScene = UnitySceneManager.GetActiveScene();
            //mIsFullscreen = Screen.fullScreen;

            mPrevTimeScale = Time.timeScale;

            mPauseCounter = 0;

            mTransitions = new List<ITransition>();
                        
            if(stackEnable)
                mSceneStack = new Stack<string>(stackCapacity);

            mScenesAdded = new List<Scene>();
            mScenesToAdd = new Queue<string>();

            mScenesToRemove = new Queue<Scene>();
        }

        IEnumerator DoLoadScene(string toScene, LoadSceneMode mode, bool unloadCurrent) {
            isLoading = true;
            
            //about to change scene
            if(sceneChangeStartCallback != null)
                sceneChangeStartCallback();

            //play out transitions
            for(int i = 0; i < mTransitions.Count; i++)
                yield return mTransitions[i].Out();

            //wait for scene add to finish
            while(mSceneAddRout != null)
                yield return null;

            //unload added scenes
            UnloadAddedScenes();

            //wait for scene remove to finish
            while(mSceneRemoveRout != null)
                yield return null;

            //scene is about to change
            if(sceneChangeCallback != null)
                sceneChangeCallback(toScene);

            bool doLoad = true;

            if(mode == LoadSceneMode.Additive) {
                //Debug.Log("unload: "+mCurScene);
                if(unloadCurrent && mCurScene != mRootScene) {
                    var sync = UnitySceneManager.UnloadSceneAsync(mCurScene);

                    while(!sync.isDone)
                        yield return null;
                }
                
                //load only if it doesn't exist
                doLoad = !UnitySceneManager.GetSceneByName(toScene).IsValid();
            }

            //load
            if(doLoad) {
                var sync = UnitySceneManager.LoadSceneAsync(toScene, mode);

                //something went wrong
                if(sync == null) {
                    isLoading = false;
                    yield break;
                }

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
                //wait for scene removes to finish
                while(mSceneRemoveRout != null)
                    yield return null;

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

        IEnumerator DoRemoveScene() {
            while(mScenesToRemove.Count > 0) {
                var scene = mScenesToRemove.Dequeue();

                var sync = UnitySceneManager.UnloadSceneAsync(scene);

                while(!sync.isDone)
                    yield return null;

                if(sceneRemovedCallback != null)
                    sceneRemovedCallback(scene);
            }

            mSceneRemoveRout = null;
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
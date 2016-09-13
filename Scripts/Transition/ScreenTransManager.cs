using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Screen Transition/Manager")]
    public class ScreenTransManager : SingletonBehaviour<ScreenTransManager> {
        public enum Action {
            Begin, //transition is about to play, called after transition's Prepare
            End //transition has ended
        }

        public delegate void Callback(ScreenTrans trans, Action act);

        public struct ProgressData {
            public ScreenTrans transition;
            public Callback call;
        }

        public ScreenTrans[] library;
        public bool libraryFromChildren;

        private ScreenTrans mPrevTrans;

        private RenderTexture mRenderTexture;
        private Vector2 mRenderTextureScreenSize;
        private Queue<ProgressData> mProgress = new Queue<ProgressData>(8);
        private IEnumerator mProgressRoutine;

        private ScreenTransPlayer mCurPlayer;
        private ScreenTrans mCurTrans;

        public RenderTexture renderTexture {
            get {
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                if(mRenderTexture == null || mRenderTextureScreenSize != screenSize) {
                    if(mRenderTexture)
                        DestroyImmediate(mRenderTexture);

                    mRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);

                    mRenderTextureScreenSize = screenSize;
                }

                return mRenderTexture;
            }
        }

        public Texture CameraSnapshot(Camera cam) {
            if(mPrevTrans && mPrevTrans.target == ScreenTrans.ToType.Texture)
                return mPrevTrans.targetTexture;

            RenderTexture lastRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(false, true, Color.clear);
            if(!cam.targetTexture) {
                cam.targetTexture = renderTexture;
                cam.Render();
                cam.targetTexture = null;
            }
            RenderTexture.active = lastRT;

            return renderTexture;
        }

        public Texture CameraSnapshot(Camera[] cams) {
            if(mPrevTrans && mPrevTrans.target == ScreenTrans.ToType.Texture)
                return mPrevTrans.targetTexture;

            RenderTexture lastRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear(false, true, Color.clear);

            //NOTE: assumes cams are in the correct depth order
            for(int i = 0; i < cams.Length; i++) {
                if(!cams[i].targetTexture) {
                    cams[i].targetTexture = renderTexture;
                    cams[i].Render();
                    cams[i].targetTexture = null;
                }
            }

            RenderTexture.active = lastRT;

            return renderTexture;
        }

        public ScreenTrans GetTransition(string transName) {
            for(int i = 0; i < library.Length; i++) {
                if(library[i].name == transName)
                    return library[i];
            }
            Debug.LogWarning("Transition not found: " + transName);
            return null;
        }

        public void Play(ScreenTrans trans, Callback callback) {
            if(trans) {
                mProgress.Enqueue(new ProgressData() { transition=trans, call=callback });
                if(mProgressRoutine == null)
                    StartCoroutine(mProgressRoutine = DoProgress());
            }
        }

        public void Play(string transName, Callback callback) {
            Play(GetTransition(transName), callback);
        }
        
        protected override void OnInstanceInit() {
            if(libraryFromChildren)
                library = GetComponentsInChildren<ScreenTrans>();

            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected override void OnInstanceDeinit() {
            if(mRenderTexture)
                DestroyImmediate(mRenderTexture);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
            if(mode == UnityEngine.SceneManagement.LoadSceneMode.Single)
                RestorePlayer();
        }

        IEnumerator DoProgress() {
            WaitForEndOfFrame wait = new WaitForEndOfFrame();
            while(mProgress.Count > 0) {
                ProgressData curProg = mProgress.Dequeue();

                mCurTrans = curProg.transition;
                mCurTrans.Prepare();

                if(curProg.call != null)
                    curProg.call(mCurTrans, Action.Begin);

                while(!mCurTrans.isDone) {
                    //Debug.Log("playing: "+trans);
                    RestorePlayer();
                    yield return wait;
                }

                mPrevTrans = mCurTrans;

                if(curProg.call != null)
                    curProg.call(mCurTrans, Action.End);

                mCurTrans = null;
                mCurPlayer = null;
            }

            mProgressRoutine = null;
        }

        void RestorePlayer() {
            if(mCurTrans != null) {
                if(mCurPlayer == null) {
                    Camera cam = mCurTrans.GetCameraTarget();
                    if(cam) {
                        mCurPlayer = cam.GetComponent<ScreenTransPlayer>();
                        if(!mCurPlayer)
                            mCurPlayer = cam.gameObject.AddComponent<ScreenTransPlayer>();

                        mCurPlayer.Play(mCurTrans);
                    }
                }
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("")]
    public abstract class TransitionFX : MonoBehaviour {
        public enum SourceType {
            CameraSnapShot,
            Texture
        }

        public enum ToType {
            Camera,
            Texture
        }

        public enum Anchor {
            BottomLeft,
            Left,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            Center
        }

        public enum CameraType {
            Main, //use Camera.main
            All, //use the camera with the highest depth
            Target, //use cameraTarget
        }

        public Shader shader;

        public float delay = 1.0f;
        public AnimationCurve curve = new AnimationCurve();
        public bool curveNormalized; //if true, curve is based on 0-1 within delay

        public CameraType cameraType = CameraType.Main;
        public Camera cameraTarget;

        public ToType target = ToType.Camera;
        public Texture targetTexture; //for ToType.Texture
                
        private float mCurTime;
        private Material mMat;

        private RenderTexture mRenderTexture;
        private Vector2 mRenderTextureScreenSize;

        private Coroutine mPlayRout;

        private bool mIsRenderActive;
        private TransitionFXRender mTransRender;

        public float curTime { get { return mCurTime; } }
        public float curTimeNormalized { get { return mCurTime/delay; } }

        /// <summary>
        /// Returns current curve value based on current time
        /// </summary>
        public float curCurveValue { get { return curve.Evaluate(curveNormalized ? curTimeNormalized : curTime); } }

        public bool isPrepared { get; private set; }
        public bool isPlaying { get { return mPlayRout != null; } }

        public bool isRenderActive {
            get { return mIsRenderActive; }

            set {
                mIsRenderActive = value;
                if(mIsRenderActive) {
                    if(!mTransRender) {
                        mTransRender = GetPlayer();
                        if(mTransRender)
                            mTransRender.AddRender(this);
                    }
                }
                else if(mTransRender) {
                    mTransRender.RemoveRender(this);
                    mTransRender = null;
                }
            }
        }

        public Material material {
            get {
                if(mMat == null)
                    mMat = new Material(shader);
                return mMat;
            }
        }

        private RenderTexture renderTexture
        {
            get
            {
                Vector2 screenSize = new Vector2(Screen.width, Screen.height);
                if(mRenderTexture == null || mRenderTextureScreenSize != screenSize) {
                    if(mRenderTexture)
                        DestroyImmediate(mRenderTexture);

                    mRenderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);

                    mRenderTextureScreenSize = screenSize;
                }

                return mRenderTexture;
            }
        }

        /// <summary>
        /// Depending on cameraType, set cameraTarget to the appropriate reference.
        /// </summary>
        public Camera GetCameraTarget() {
            switch(cameraType) {
                case CameraType.Main:
                    return Camera.main;
                case CameraType.Target:
                    return cameraTarget ? cameraTarget : Camera.main;
                case CameraType.All:
                    Camera[] cams = Camera.allCameras;
                    float maxDepth = float.MinValue;
                    int maxIndex = -1;
                    for(int i = 0; i < cams.Length; i++) {
                        if(cams[i].depth > maxDepth) {
                            maxDepth = cams[i].depth;
                            maxIndex = i;
                        }
                    }

                    return maxIndex == -1 ? Camera.main : cams[maxIndex];
            }

            return null;
        }

        /// <summary>
        /// Certain transitions need to prepare before Play can be called (e.g. cross fade)
        /// </summary>
        public void Prepare() {
            if(!isPrepared) {
                mCurTime = 0f;

                OnPrepare();
                OnUpdate(); //do one update

                isPrepared = true;
            }
        }
        
        /// <summary>
        /// Call this to start rendering. Note: will continue to render after duration, you will need to call End to stop rendering.
        /// </summary>
        public void Play() {
            if(mPlayRout != null)
                StopCoroutine(mPlayRout);

            mPlayRout = StartCoroutine(DoPlay());
        }

        /// <summary>
        /// Call this to stop rendering.  Note: make sure to call this after Play
        /// </summary>
        public void End() {
            mCurTime = delay;
            isPrepared = false;
            isRenderActive = false;

            if(mPlayRout != null) {
                StopCoroutine(mPlayRout);
                mPlayRout = null;
            }

            OnFinish();

            if(mRenderTexture) {
                DestroyImmediate(mRenderTexture);
                mRenderTexture = null;
            }
        }
        
        protected void SetSourceTexture(SourceType source, Texture sourceTexture) {
            Debug.Log("set source texture");

            switch(source) {
                case SourceType.CameraSnapShot:
                    switch(cameraType) {
                        case CameraType.Main:
                            material.SetTexture("_SourceTex", CameraSnapshot(Camera.main));
                            break;
                        case CameraType.Target:
                            material.SetTexture("_SourceTex", CameraSnapshot(cameraTarget ? cameraTarget : Camera.main));
                            break;
                        case CameraType.All:
                            material.SetTexture("_SourceTex", CameraSnapshot(Util.GetAllCameraDepthSorted()));
                            break;
                    }
                    break;

                case SourceType.Texture:
                    material.SetTexture("_SourceTex", sourceTexture);
                    break;
            }
        }

        protected Vector2 GetUVScroll(Anchor anchor, float t) {
            Vector2 ret = Vector2.zero;
            switch(anchor) {
                case Anchor.TopLeft:
                    ret.x = -1f + t * 2f;
                    ret.y = -1f + (1f - t) * 2f;
                    break;
                case Anchor.Top:
                    ret.x = 0f;
                    ret.y = -1f + (1f - t) * 2f;
                    break;
                case Anchor.TopRight:
                    ret.x = -1f + (1f - t) * 2f;
                    ret.y = -1f + (1f - t) * 2f;
                    break;
                case Anchor.Right:
                    ret.x = -1f + (1f - t) * 2f;
                    ret.y = 0f;
                    break;
                case Anchor.BottomRight:
                    ret.x = -1f + (1f - t) * 2f;
                    ret.y = -1f + t * 2f;
                    break;
                case Anchor.Bottom:
                    ret.x = 0f;
                    ret.y = -1f + t * 2f;
                    break;
                case Anchor.BottomLeft:
                    ret.x = -1f + t * 2f;
                    ret.y = -1f + t * 2f;
                    break;
                case Anchor.Left:
                    ret.x = -1f + t * 2f;
                    ret.y = 0f;
                    break;
                default:
                    ret.x = 0f;
                    ret.y = 0f;
                    break;
            }

            return ret;
        }

        protected Vector2 GetAnchorPoint(Anchor anchor) {
            Vector2 ret = Vector2.zero;
            switch(anchor) {
                case Anchor.TopLeft:
                    ret.x = -1f;
                    ret.y = 1f;
                    break;
                case Anchor.Top:
                    ret.x = 0f;
                    ret.y = 1f;
                    break;
                case Anchor.TopRight:
                    ret.x = 1f;
                    ret.y = 1f;
                    break;
                case Anchor.Right:
                    ret.x = 1f;
                    ret.y = 0f;
                    break;
                case Anchor.BottomRight:
                    ret.x = 1f;
                    ret.y = -1f;
                    break;
                case Anchor.Bottom:
                    ret.x = 0f;
                    ret.y = -1f;
                    break;
                case Anchor.BottomLeft:
                    ret.x = -1f;
                    ret.y = -1f;
                    break;
                case Anchor.Left:
                    ret.x = -1f;
                    ret.y = 0f;
                    break;
                default:
                    ret.x = 0f;
                    ret.y = 0f;
                    break;
            }

            ret = ret / 2f + new Vector2(0.5f, 0.5f);

            return ret;
        }

        protected virtual void OnDisable() {
            End();
        }

        protected virtual void Awake() {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnDestroy() {
            if(mMat)
                DestroyImmediate(mMat);

            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #region internal

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode) {
            if(mode == UnityEngine.SceneManagement.LoadSceneMode.Single) {
                //restore render?
                if(mIsRenderActive) {
                    if(!mTransRender) {
                        mTransRender = GetPlayer();
                        if(mTransRender)
                            mTransRender.AddRender(this); //re-establish
                        else
                            End(); //abort
                    }
                }
            }
        }

        IEnumerator DoPlay() {
            Prepare();

            isRenderActive = true;

            if(!mTransRender) {
                End();
                yield break;
            }
            
            var wait = new WaitForEndOfFrame();

            while(mCurTime < delay) {
                yield return wait;

                mCurTime = Mathf.Min(mCurTime + Time.smoothDeltaTime, delay);

                OnUpdate();
            }

            mPlayRout = null;
        }

        TransitionFXRender GetPlayer() {
            Camera cam = GetCameraTarget();
            if(cam) {
                var player = cam.GetComponent<TransitionFXRender>();
                if(!player)
                    player = cam.gameObject.AddComponent<TransitionFXRender>();

                return player;
            }

            return null;
        }

        Texture CameraSnapshot(Camera cam) {
            //RenderTexture lastRT = RenderTexture.active;
            //RenderTexture.active = renderTexture;
            //GL.Clear(false, true, Color.clear);
            if(!cam.targetTexture) {
                cam.targetTexture = renderTexture;
                cam.Render();
                cam.targetTexture = null;
            }
            //RenderTexture.active = lastRT;

            return renderTexture;
        }

        Texture CameraSnapshot(Camera[] cams) {
            //RenderTexture lastRT = RenderTexture.active;
            //RenderTexture.active = renderTexture;
            //GL.Clear(false, true, Color.clear);

            //NOTE: assumes cams are in the correct depth order
            for(int i = 0; i < cams.Length; i++) {
                if(!cams[i].targetTexture) {
                    cams[i].targetTexture = renderTexture;
                    cams[i].Render();
                    cams[i].targetTexture = null;
                }
            }

            //RenderTexture.active = lastRT;

            return renderTexture;
        }

        #endregion

        #region implements

        protected virtual void OnPrepare() {
        }

        protected virtual void OnUpdate() {
        }

        protected virtual void OnFinish() {
        }

        public virtual void OnRenderImage(Texture source, RenderTexture destination) {
            if(mMat) //_MainTex = target
                Graphics.Blit(target == ToType.Camera ? source : targetTexture, destination, mMat);
            else
                Graphics.Blit(source, destination);
        }

        #endregion
    }
}
 
using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public abstract class ScreenTrans : MonoBehaviour {
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
    private bool mIsReady; //if true, this transition has been initialized and ready to play
    private Material mMat;

    public float curTime { get { return mCurTime; } }
    public float curTimeNormalized { get { return mCurTime/delay; } }

    /// <summary>
    /// Returns current curve value based on current time
    /// </summary>
    public float curCurveValue { get { return curve.Evaluate(curveNormalized ? curTimeNormalized : curTime); } }

    public bool isDone { get { return mCurTime >= delay; } }

    public Material material {
        get {
            if(mMat == null)
                mMat = new Material(shader);
            return mMat;
        }
    }

    /// <summary>
    /// Depending on cameraType, set cameraTarget to the appropriate reference.
    /// </summary>
    public Camera GetCameraTarget() {
        Camera cam = cameraTarget;

        switch(cameraType) {
            case CameraType.Main:
                cam = Camera.main;
                break;
            case CameraType.Target:
                if(!cam)
                    cam = Camera.main;
                break;
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

                cam = maxIndex == -1 ? Camera.main : cams[maxIndex];
                break;
        }

        return cam;
    }

    public void Prepare() {
        if(!mIsReady) {
            //set textures

            OnPrepare();

            mCurTime = 0.0f;
            mIsReady = true;
        }
    }

    public void End() {
        mCurTime = delay;
        mIsReady = false;
        OnFinish();
    }

    /// <summary>
    /// Called by ScreenTransPlayer
    /// </summary>
    public void Run(float dt) {
        if(mIsReady) {
            mCurTime += dt;
            if(mCurTime > delay)
                mCurTime = delay;

            OnUpdate();

            if(mCurTime == delay) {
                mIsReady = false;
                OnFinish();
            }
        }
    }

    protected void SetSourceTexture(SourceType source, Texture sourceTexture) {
        switch(source) {
            case SourceType.CameraSnapShot:
                switch(cameraType) {
                    case CameraType.Main:
                        material.SetTexture("_SourceTex", ScreenTransManager.instance.CameraSnapshot(Camera.main));
                        break;
                    case CameraType.Target:
                        material.SetTexture("_SourceTex", ScreenTransManager.instance.CameraSnapshot(cameraTarget ? cameraTarget : Camera.main));
                        break;
                    case CameraType.All:
                        material.SetTexture("_SourceTex", ScreenTransManager.instance.CameraSnapshot(M8.Util.GetAllCameraDepthSorted()));
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

    protected virtual void OnDestroy() {
        if(mMat)
            DestroyImmediate(mMat);
    }

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

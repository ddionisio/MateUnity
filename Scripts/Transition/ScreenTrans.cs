using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public class ScreenTrans : MonoBehaviour {
    
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
    public float curCurveValue { get { return curve.Evaluate(mCurTime); } }

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
                float minDepth = float.MinValue;
                int minIndex = -1;
                for(int i = 0; i < cams.Length; i++) {
                    if(cams[i].depth < minDepth) {
                        minDepth = cams[i].depth;
                        minIndex = i;
                    }
                }

                cam = minIndex == -1 ? Camera.main : cams[minIndex];
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

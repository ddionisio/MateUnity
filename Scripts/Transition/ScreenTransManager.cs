using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Screen Transition/Manager")]
public class ScreenTransManager : MonoBehaviour {
    public ScreenTrans[] library;

    private ScreenTrans mPrevTrans;
    private ScreenTrans mCurTrans;

    private RenderTexture mRenderTexture;
    private Vector2 mRenderTextureScreenSize;

    private static ScreenTransManager mInstance;

    public static ScreenTransManager instance { get { return mInstance; } }

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
        return null;
    }

    public void Play(ScreenTrans trans) {
        if(mCurTrans) {
            StopAllCoroutines();
            mCurTrans = null;
        }

        if(trans)
            StartCoroutine(DoPlay(trans));
    }

    public void Play(string transName) {
        Play(GetTransition(transName));
    }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;

            if(mRenderTexture)
                DestroyImmediate(mRenderTexture);
        }
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            //
        }
        else
            DestroyImmediate(gameObject);
    }

    IEnumerator DoPlay(ScreenTrans trans) {
        mCurTrans = trans;

        Camera cam = trans.GetCameraTarget();
        if(cam) {
            ScreenTransPlayer player = cam.GetComponent<ScreenTransPlayer>();
            if(!player)
                player = cam.gameObject.AddComponent<ScreenTransPlayer>();

            player.Play(trans);
        }

        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        while(mCurTrans && !mCurTrans.isDone)
            yield return wait;

        mPrevTrans = mCurTrans;
        mCurTrans = null;
    }
}

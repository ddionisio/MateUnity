using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Screen Transition/Manager")]
public class ScreenTransManager : MonoBehaviour {
    public enum Action {
        Begin, //transition is about to play, called after transition's Prepare
        End //transition has ended
    }

    public delegate void Callback(ScreenTrans trans, Action act);

    public ScreenTrans[] library;

    /// <summary>
    /// Get a call each time when transition begins and ends
    /// </summary>
    public event Callback transitionCallback;

    private ScreenTrans mPrevTrans;

    private RenderTexture mRenderTexture;
    private Vector2 mRenderTextureScreenSize;
    private Queue<ScreenTrans> mProgress = new Queue<ScreenTrans>(8);
    private IEnumerator mProgressRoutine;

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
        Debug.LogWarning("Transition not found: " + transName);
        return null;
    }

    public void Play(ScreenTrans trans) {
        if(trans) {
            mProgress.Enqueue(trans);
            if(mProgressRoutine == null)
                StartCoroutine(mProgressRoutine = DoProgress());
        }
    }

    public void Play(string transName) {
        Play(GetTransition(transName));
    }

    public void Prepare(ScreenTrans trans) {
        if(trans)
            trans.Prepare();
    }

    public void Prepare(string transName) {
        Play(GetTransition(transName));
    }

    void OnDestroy() {
        if(mInstance == this) {
            mInstance = null;

            if(mRenderTexture)
                DestroyImmediate(mRenderTexture);

            transitionCallback = null;
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

    IEnumerator DoProgress() {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        while(mProgress.Count > 0) {
            ScreenTrans trans = mProgress.Dequeue();

            trans.Prepare();

            if(transitionCallback != null)
                transitionCallback(trans, Action.Begin);

            Camera cam = trans.GetCameraTarget();
            if(cam) {
                ScreenTransPlayer player = cam.GetComponent<ScreenTransPlayer>();
                if(!player)
                    player = cam.gameObject.AddComponent<ScreenTransPlayer>();

                player.Play(trans);
            }

            while(!trans.isDone)
                yield return wait;

            mPrevTrans = trans;

            if(transitionCallback != null)
                transitionCallback(trans, Action.End);
        }

        mProgressRoutine = null;
    }
}

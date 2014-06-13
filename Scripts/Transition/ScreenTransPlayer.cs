using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Screen Transition/Player")]
[RequireComponent(typeof(Camera))]
public class ScreenTransPlayer : MonoBehaviour {
    private ScreenTrans mPrevTrans; //do a one frame render before going to next transition
    private ScreenTrans mCurrentTrans;

    public void Play(ScreenTrans trans) {
        mPrevTrans = mCurrentTrans;

        mCurrentTrans = trans;
        if(mCurrentTrans) {
            mCurrentTrans.Prepare();

            enabled = true;
        }
        else
            enabled = false;
    }

    void OnDestroy() {
        if(mCurrentTrans)
            mCurrentTrans.End();
    }

    void Update() {
        if(mCurrentTrans) {
            if(mCurrentTrans.isDone) {
                mCurrentTrans = null;
                enabled = false;
            }
            else
                mCurrentTrans.Run(Time.deltaTime);
        }
    }

    void Awake() {
        enabled = false;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if(mPrevTrans) {
            mPrevTrans.OnRenderImage(source, destination);
            mPrevTrans = null;
        }

        if(mCurrentTrans)
            mCurrentTrans.OnRenderImage(source, destination);
    }
}

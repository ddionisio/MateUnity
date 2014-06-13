using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Screen Transition/Player")]
[RequireComponent(typeof(Camera))]
public class ScreenTransPlayer : MonoBehaviour {
    private ScreenTrans mCurrentTrans;

    /*public static void Play(ScreenTrans trans) {
        //get camera
        Camera cam = trans.GetCameraTarget();
        if(cam) {
            ScreenTransPlayer player = cam.GetComponent<ScreenTransPlayer>();
            if(!player)
                player = cam.gameObject.AddComponent<ScreenTransPlayer>();

            player.DoPlay(trans);
        }
    }*/
    
    public void Play(ScreenTrans trans) {
        if(mCurrentTrans) mCurrentTrans.End();

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
        if(mCurrentTrans)
            mCurrentTrans.OnRenderImage(source, destination);
    }
}

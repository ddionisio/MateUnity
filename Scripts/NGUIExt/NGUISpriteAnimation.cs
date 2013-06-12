using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/SpriteAnimation")]
public class NGUISpriteAnimation : MonoBehaviour {
    public UISprite frameHolder;

    public string[] frames;
    public float framesPerSecond;

    public bool makePixelPerfect = false;

    private int mCurFrame;
    private float mFrameCounter;

    void Awake() {
        if(frameHolder == null)
            frameHolder = GetComponent<UISprite>();
    }

    // Use this for initialization
    void Start() {
        mFrameCounter = 0;
        mFrameCounter = 0;
        SetToCurrentFrame();
    }

    // Update is called once per frame
    void Update() {
        mFrameCounter += Time.deltaTime * framesPerSecond;
        int newFrame = Mathf.RoundToInt(mFrameCounter);
        if(mCurFrame != newFrame) {
            mCurFrame = newFrame;
            if(mCurFrame >= frames.Length) {
                mCurFrame = 0;
                mFrameCounter -= (float)frames.Length;
            }

            SetToCurrentFrame();
        }
    }

    void SetToCurrentFrame() {
        frameHolder.spriteName = frames[mCurFrame];
        if(makePixelPerfect)
            frameHolder.MakePixelPerfect();
    }
}

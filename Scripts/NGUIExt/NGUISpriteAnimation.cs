using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/SpriteAnimation")]
public class NGUISpriteAnimation : MonoBehaviour {
    public UISprite frameHolder;

    public string[] frames;
    public float framesPerSecond;

    public bool resetOnStop = false;
    public bool autoPlay = true;
    public bool makePixelPerfect = false;

    private int mCurFrame;
    private float mFrameCounter;
    private bool mStarted;
    private bool mPlaying;

    public void Play() {
        mPlaying = true;
    }

    public void Stop() {
        if(resetOnStop) {
            mCurFrame = 0;
            mFrameCounter = 0;
            SetToCurrentFrame();
        }

        mPlaying = false;
    }

    void Awake() {
        if(frameHolder == null)
            frameHolder = GetComponent<UISprite>();
    }

    void OnEnable() {
        if(mStarted && autoPlay)
            Play();
    }

    void OnDisable() {
        Stop();
    }

    // Use this for initialization
    void Start() {
        mCurFrame = 0;
        mFrameCounter = 0;
        SetToCurrentFrame();

        mStarted = true;

        if(autoPlay)
            Play();
    }

    // Update is called once per frame
    void Update() {
        if(mPlaying) {
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
    }

    void SetToCurrentFrame() {
        frameHolder.spriteName = frames[mCurFrame];
        if(makePixelPerfect)
            frameHolder.MakePixelPerfect();
    }
}

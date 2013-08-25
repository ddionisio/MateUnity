using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/NGUI/ColorLerp")]
public class NGUIColorLerp : MonoBehaviour {
    public enum Type {
        Once,
        Saw,
        SeeSaw,
        Repeat,

        NumType
    }

    public UIWidget target;

    public Type type;

    public float delay;

    public Color[] colors;

    private float mCurTime = 0;
    private bool mStarted = false;
    private bool mActive = false;
    private bool mReverse = false;

    public void SetCurTime(float time) {
        mCurTime = time;
    }

    void OnEnable() {
        if(mStarted) {
            mActive = true;
            mReverse = false;
            mCurTime = 0;
            target.color = colors[0];
        }
    }

    void Awake() {
        if(target == null)
            target = GetComponent<UIWidget>();
    }

    // Use this for initialization
    void Start() {
        mStarted = true;
        mActive = true;
        mReverse = false;
        target.color = colors[0];
    }

    // Update is called once per frame
    void Update() {
        if(mActive) {
            mCurTime = mCurTime + (mReverse ? -Time.deltaTime : Time.deltaTime);

            switch(type) {
                case Type.Once:
                    if(mCurTime >= delay) {
                        mActive = false;
                        target.color = colors[colors.Length - 1];
                    }
                    else {
                        target.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                    }
                    break;

                case Type.Repeat:
                    if(mCurTime > delay) {
                        mCurTime -= delay;
                    }

                    target.color = M8.ColorUtil.LerpRepeat(colors, mCurTime / delay);
                    break;

                case Type.Saw:
                    if(mCurTime > delay) {
                        mCurTime -= delay;
                    }

                    target.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                    break;

                case Type.SeeSaw:
                    if(mCurTime > delay) {
                        if(mReverse)
                            mCurTime -= delay;
                        else
                            mCurTime = delay - (mCurTime - delay);

                        mReverse = !mReverse;
                    }

                    target.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                    break;
            }
        }
    }
}

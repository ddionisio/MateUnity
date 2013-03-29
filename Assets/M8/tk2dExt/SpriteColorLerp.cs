using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ColorLerp")]
public class SpriteColorLerp : MonoBehaviour {
    public enum Type {
        Once,
        Saw,
        SeeSaw,
        Repeat,

        NumType
    }

    public Type type;

	public float delay;

    public Color[] colors;

	private tk2dBaseSprite mSprite;
	
	private float mCurTime = 0;
    private bool mStarted = false;
    private bool mActive = false;
    private bool mReverse = false;
        	
	void OnEnable() {
        if(mStarted) {
            mActive = true;
            mReverse = false;
            mCurTime = 0;
            mSprite.color = colors[0];
        }
	}
	
	void Awake() {
		mSprite = GetComponent<tk2dBaseSprite>();
	}
	
	// Use this for initialization
	void Start () {
        mStarted = true;
        mActive = true;
        mReverse = false;
        mSprite.color = colors[0];
	}
	
	// Update is called once per frame
	void Update () {
        if(mActive) {
            mCurTime = mCurTime + (mReverse ? -Time.deltaTime : Time.deltaTime);

            switch(type) {
                case Type.Once:
                    if(mCurTime >= delay) {
                        mActive = false;
                        mSprite.color = colors[colors.Length-1];
                    }
                    else {
                        mSprite.color = M8.ColorUtil.Lerp(colors, mCurTime/delay);
                    }
                    break;

                case Type.Repeat:
                    if(mCurTime > delay) {
                        mCurTime -= delay;
                    }

                    mSprite.color = M8.ColorUtil.LerpRepeat(colors, mCurTime/delay);
                    break;

                case Type.Saw:
                    if(mCurTime > delay) {
                        mCurTime -= delay;
                    }

                    mSprite.color = M8.ColorUtil.Lerp(colors, mCurTime/delay);
                    break;

                case Type.SeeSaw:
                    if(mCurTime > delay) {
                        if(mReverse)
                            mCurTime -= delay;
                        else
                            mCurTime = delay - (mCurTime-delay);

                        mReverse = !mReverse;
                    }

                    mSprite.color = M8.ColorUtil.Lerp(colors, mCurTime/delay);
                    break;
            }
        }
	}
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/TextColorLerp")]
public class TextColorLerp : MonoBehaviour {
    public enum Type {
        Once,
        Saw,
        SeeSaw,
        Repeat,

        NumType
    }

    public tk2dTextMesh sprite;

    public Type type;

	public float delay;

    public Color[] colors;

	private float mCurTime = 0;
    private bool mStarted = false;
    private bool mActive = false;
    private bool mReverse = false;
        	
	void OnEnable() {
        if(mStarted) {
            mActive = true;
            mReverse = false;
            mCurTime = 0;
            sprite.color = colors[0];
            sprite.Commit();
        }
	}
	
	void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dTextMesh>();
	}
	
	// Use this for initialization
	void Start () {
        mStarted = true;
        mActive = true;
        mReverse = false;
        sprite.color = colors[0];
        sprite.Commit();
	}
	
	// Update is called once per frame
	void Update () {
        if(mActive) {
            mCurTime = mCurTime + (mReverse ? -Time.deltaTime : Time.deltaTime);

            switch(type) {
                case Type.Once:
                    if(mCurTime >= delay) {
                        mActive = false;
                        sprite.color = colors[colors.Length - 1];
                    }
                    else {
                        sprite.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                    }
                    break;

                case Type.Repeat:
                    if(mCurTime > delay) {
                        mCurTime -= delay;
                    }

                    sprite.color = M8.ColorUtil.LerpRepeat(colors, mCurTime / delay);
                    break;

                case Type.Saw:
                    if(mCurTime > delay) {
                        mCurTime -= delay;
                    }

                    sprite.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                    break;

                case Type.SeeSaw:
                    if(mCurTime > delay) {
                        if(mReverse)
                            mCurTime -= delay;
                        else
                            mCurTime = delay - (mCurTime-delay);

                        mReverse = !mReverse;
                    }

                    sprite.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                    break;
            }

            sprite.Commit();
        }
	}
}

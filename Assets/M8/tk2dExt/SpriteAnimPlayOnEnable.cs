using UnityEngine;
using System.Collections;

[RequireComponent(typeof(tk2dAnimatedSprite))]
[AddComponentMenu("M8/tk2D/AnimPlayOnEnable")]
public class SpriteAnimPlayOnEnable : MonoBehaviour {
    private tk2dAnimatedSprite mSpr;
    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            mSpr.Play();
        }
    }

    void Awake() {
        mSpr = GetComponent<tk2dAnimatedSprite>();
    }

    // Use this for initialization
    void Start() {
        mStarted = true;
        if(!mSpr.playAutomatically)
            mSpr.Play();
    }
}

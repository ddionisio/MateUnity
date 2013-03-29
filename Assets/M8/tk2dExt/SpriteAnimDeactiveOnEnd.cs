using UnityEngine;

[RequireComponent(typeof(tk2dAnimatedSprite))]
[AddComponentMenu("M8/tk2D/AnimDeactiveOnEnd")]
public class SpriteAnimDeactiveOnEnd : MonoBehaviour {
    private tk2dAnimatedSprite mSpr;

    void OnDestroy() {
        if(mSpr != null)
            mSpr.animationCompleteDelegate -= OnAnimationComplete;
    }

    void Awake() {
        mSpr = GetComponent<tk2dAnimatedSprite>();
        mSpr.animationCompleteDelegate += OnAnimationComplete;
    }

    private void OnAnimationComplete(tk2dAnimatedSprite sprite, int clipId) {
        gameObject.SetActive(false);
    }
}

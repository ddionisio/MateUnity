using UnityEngine;

[AddComponentMenu("M8/tk2D/AnimDeactiveOnEnd")]
public class SpriteAnimDeactiveOnEnd : MonoBehaviour {
    public tk2dSpriteAnimator sprite;

    void OnDestroy() {
        if(sprite != null)
            sprite.AnimationCompleted -= OnAnimationComplete;
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dSpriteAnimator>();

        sprite.AnimationCompleted += OnAnimationComplete;
    }

    private void OnAnimationComplete(tk2dSpriteAnimator aSprite, tk2dSpriteAnimationClip clip) {
        gameObject.SetActive(false);
    }
}

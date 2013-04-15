using UnityEngine;

[RequireComponent(typeof(tk2dAnimatedSprite))]
[AddComponentMenu("M8/tk2D/AnimDeactiveOnEnd")]
public class SpriteAnimDeactiveOnEnd : MonoBehaviour {
    public tk2dAnimatedSprite sprite;

    void OnDestroy() {
        if(sprite != null)
            sprite.animationCompleteDelegate -= OnAnimationComplete;
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dAnimatedSprite>();

        sprite.animationCompleteDelegate += OnAnimationComplete;
    }

    private void OnAnimationComplete(tk2dAnimatedSprite aSprite, int clipId) {
        gameObject.SetActive(false);
    }
}

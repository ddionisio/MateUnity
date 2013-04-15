using UnityEngine;
using System.Collections;

[RequireComponent(typeof(tk2dAnimatedSprite))]
[AddComponentMenu("M8/tk2D/AnimPlayOnEnable")]
public class SpriteAnimPlayOnEnable : MonoBehaviour {
    public tk2dAnimatedSprite sprite;

    private bool mStarted = false;

    void OnEnable() {
        if(mStarted) {
            sprite.Play();
        }
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dAnimatedSprite>();
    }

    // Use this for initialization
    void Start() {
        mStarted = true;
        if(!sprite.playAutomatically)
            sprite.Play();
    }
}

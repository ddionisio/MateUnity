using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/PlayOnEnable")]
public class SpritePlayOnEnable : MonoBehaviour {
	
	public tk2dAnimatedSprite sprite;

    public float minDelay;
    public float maxDelay;

	void OnEnable() {
        if(minDelay > 0) {
            if(minDelay < maxDelay)
                Invoke("PlayDelayed", Random.Range(minDelay, maxDelay));
        }
        else {
            sprite.Play();
        }
	}
	
    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dAnimatedSprite>();

        sprite.playAutomatically = false;
    }
	
    void PlayDelayed() {
        sprite.Play();
    }
}

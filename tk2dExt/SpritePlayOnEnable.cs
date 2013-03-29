using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/PlayOnEnable")]
public class SpritePlayOnEnable : MonoBehaviour {
	
	private tk2dAnimatedSprite mAnimSprite;
	
	
	void OnEnable() {
		if(mAnimSprite != null) {
			mAnimSprite.PlayFromFrame(0);
		}
	}
	
	void OnDisable() {
	}
	
	void Start() {
		mAnimSprite = GetComponent<tk2dAnimatedSprite>();
		if(mAnimSprite != null && !mAnimSprite.playAutomatically) {
			mAnimSprite.Play();
		}
	}
}

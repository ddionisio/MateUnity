using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/FlipRepeat")]
public class SpriteFlipRepeat : MonoBehaviour {
	
	public bool hFlip;
	public bool vFlip;
	
	public float delay;
	
	private tk2dBaseSprite mSprite;
	
	void OnEnable() {
		InvokeRepeating("DoFlip", delay, delay);
	}
	
	void Awake() {
		mSprite = GetComponent<tk2dBaseSprite>();
	}
	
	void DoFlip() {
		Vector3 s = mSprite.scale;
		
		if(hFlip)
			s.x *= -1;
		
		if(vFlip)
			s.y *= -1;
		
		mSprite.scale = s;
	}
}

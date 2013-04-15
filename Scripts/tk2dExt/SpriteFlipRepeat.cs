using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/FlipRepeat")]
public class SpriteFlipRepeat : MonoBehaviour {
    public tk2dBaseSprite sprite;
	
	public bool hFlip;
	public bool vFlip;
	
	public float delay;
	
	void OnEnable() {
		InvokeRepeating("DoFlip", delay, delay);
	}
	
	void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dBaseSprite>();
	}
	
	void DoFlip() {
        Vector3 s = sprite.scale;
		
		if(hFlip)
			s.x *= -1;
		
		if(vFlip)
			s.y *= -1;

        sprite.scale = s;
	}
}

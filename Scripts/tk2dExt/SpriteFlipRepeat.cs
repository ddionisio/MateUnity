using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/FlipRepeat")]
public class SpriteFlipRepeat : MonoBehaviour {
    public tk2dBaseSprite sprite;
	
	public bool hFlip;
	public bool vFlip;
	
	public float delay;

    private bool mStarted = false;
	
	void OnEnable() {
        if(mStarted)
		    InvokeRepeating("DoFlip", delay, delay);
	}

    void OnDisable() {
        CancelInvoke("DoFlip");
    }

    void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dBaseSprite>();
    }
	
	void Start() {
        mStarted = true;
        InvokeRepeating("DoFlip", delay, delay);
	}
	
	void DoFlip() {
        if(hFlip)
            sprite.FlipX = !sprite.FlipX;
		
		if(vFlip)
            sprite.FlipY = !sprite.FlipY;
	}
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ColorPulse")]
public class SpriteColorPulse : MonoBehaviour {
    public tk2dBaseSprite sprite;

	public float pulsePerSecond;
	public Color startColor;
	public Color endColor = Color.white;
	
	private float mCurPulseTime = 0;
    private bool mStarted = false;
	
	void OnEnable() {
        if(mStarted) {
            mCurPulseTime = 0;
            sprite.color = startColor;
        }
	}
	
	void Awake() {
        if(sprite == null)
            sprite = GetComponent<tk2dBaseSprite>();
	}
	
	// Use this for initialization
	void Start () {
        mStarted = true;
        sprite.color = startColor;
	}
	
	// Update is called once per frame
	void Update () {
		mCurPulseTime += Time.deltaTime;
		
		float t = Mathf.Sin(Mathf.PI*mCurPulseTime*pulsePerSecond);
		t *= t;

        sprite.color = Color.Lerp(startColor, endColor, t);
	}
}

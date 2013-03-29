using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/ColorPulse")]
public class SpriteColorPulse : MonoBehaviour {
	public float pulsePerSecond;
	public Color startColor;
	public Color endColor = Color.white;
	
	private tk2dBaseSprite mSprite;
	
	private float mCurPulseTime = 0;
    private bool mStarted = false;
	
	void OnEnable() {
        if(mStarted) {
            mCurPulseTime = 0;
            mSprite.color = startColor;
        }
	}
	
	void Awake() {
		mSprite = GetComponent<tk2dBaseSprite>();
	}
	
	// Use this for initialization
	void Start () {
        mStarted = true;
        mSprite.color = startColor;
	}
	
	// Update is called once per frame
	void Update () {
		mCurPulseTime += Time.deltaTime;
		
		float t = Mathf.Sin(Mathf.PI*mCurPulseTime*pulsePerSecond);
		t *= t;
		
		mSprite.color = Color.Lerp(startColor, endColor, t);
	}
}

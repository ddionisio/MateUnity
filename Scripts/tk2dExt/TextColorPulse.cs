using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/tk2D/TextColorPulse")]
public class TextColorPulse : MonoBehaviour {
    public tk2dTextMesh text;

	public float pulsePerSecond;
	public Color startColor;
	public Color endColor = Color.white;
	
	private float mCurPulseTime = 0;
    private bool mStarted = false;
	
	void OnEnable() {
        if(mStarted) {
            mCurPulseTime = 0;
            text.color = startColor;
            text.Commit();
        }
	}

    void OnDisable() {
        if(mStarted && text != null) {
            text.color = startColor;
            text.Commit();
        }
    }
	
	void Awake() {
        if(text == null)
            text = GetComponent<tk2dTextMesh>();
	}
	
	// Use this for initialization
	void Start () {
        mStarted = true;
        text.color = startColor;
        text.Commit();
	}
	
	// Update is called once per frame
	void Update () {
		mCurPulseTime += Time.deltaTime;
		
		float t = Mathf.Sin(Mathf.PI*mCurPulseTime*pulsePerSecond);
		t *= t;

        text.color = Color.Lerp(startColor, endColor, t);
        text.Commit();
	}
}

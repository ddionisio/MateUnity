using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransAnimWave")]
public class TransAnimWave : MonoBehaviour {
	public float pulsePerSecond;
	
	public Vector2 start;
	public Vector2 end;
	
	private float mCurPulseTime = 0;
	
	void OnEnable() {
		mCurPulseTime = 0;
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		mCurPulseTime += Time.deltaTime;
		
		float t = Mathf.Sin(Mathf.PI*mCurPulseTime*pulsePerSecond);
		
		Vector2 newPos = Vector2.Lerp(start, end, t*t);
		
		transform.localPosition = new Vector3(newPos.x, newPos.y, transform.localPosition.z);
	}
}

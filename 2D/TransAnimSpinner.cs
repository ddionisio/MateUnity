using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransAnimSpinner")]
public class TransAnimSpinner : MonoBehaviour {
	
	public float rotatePerSecond;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 angles = transform.localEulerAngles; angles.z += rotatePerSecond*Time.deltaTime;
		transform.localEulerAngles = angles;
	}
}

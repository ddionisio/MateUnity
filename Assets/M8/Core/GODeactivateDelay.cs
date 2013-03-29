using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game/GODeactivateDelay")]
public class GODeactivateDelay : MonoBehaviour {
	public float delay;
	
	void OnEnable() {
		Invoke("OnDeactive", delay);
	}
	
	void OnDeactive() {
		gameObject.SetActive(false);
	}
}

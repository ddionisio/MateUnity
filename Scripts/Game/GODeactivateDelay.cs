using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game Object/Deactivate Delay")]
public class GODeactivateDelay : MonoBehaviour {
    public delegate void OnDeactivate();

	public float delay = 1.0f;

    public event OnDeactivate deactivateCallback;

    void OnDestroy() {
        deactivateCallback = null;
    }
	
	void OnEnable() {
		Invoke("OnDeactive", delay);
	}
	
	void OnDeactive() {
		gameObject.SetActive(false);

        if(deactivateCallback != null)
            deactivateCallback();
	}
}

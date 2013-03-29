using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/RootClamp")]
public class NGUIRootClamp : UIRoot {
	
	public int minWidthClamp = 960;
	
	//TODO: max height auto scale if resolution is too high?
	
	Transform mAutoTrans;
	
	public float GetClampScale() {
		if(Screen.width < minWidthClamp) {
			return ((float)Screen.width)/minWidthClamp;
		}
		
		return 1.0f;
	}
	
	void Start() {
		mAutoTrans = transform;

		UIOrthoCamera oc = GetComponentInChildren<UIOrthoCamera>();
		
		if (oc != null)
		{
			Debug.LogWarning("UIRoot should not be active at the same time as UIOrthoCamera. Disabling UIOrthoCamera.", oc);
			Camera cam = oc.gameObject.GetComponent<Camera>();
			oc.enabled = false;
			if (cam != null) cam.orthographicSize = 1f;
		}
	}
	
	void Update() {
		manualHeight = Mathf.Max(2, automatic ? Screen.height : manualHeight);
		
		//minWidthClamp
		float size = 2f / manualHeight;
		
		if(Screen.width < minWidthClamp) {
			size *= ((float)Screen.width)/minWidthClamp;
		}
		
		Vector3 ls = mAutoTrans.localScale;

		if (!(Mathf.Abs(ls.x - size) <= float.Epsilon) ||
			!(Mathf.Abs(ls.y - size) <= float.Epsilon) ||
			!(Mathf.Abs(ls.z - size) <= float.Epsilon))
		{
			mAutoTrans.localScale = new Vector3(size, size, size);
		}
	}
}

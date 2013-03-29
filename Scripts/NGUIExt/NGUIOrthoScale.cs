using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/OrthoScale")]
public class NGUIOrthoScale : MonoBehaviour {
	public float baseHeight = 720.0f;
	public float orthoSize = 12.0f;
	
	private Transform mTrans = null;

#if UNITY_EDITOR
    private int mHeight = 0;
#endif
	
	public void OnEnable() {
		if(mTrans == null) mTrans = transform;
		
		Refresh();
	}
	
	public void Refresh() {
		if(mTrans != null) {
			float s = orthoSize/(baseHeight*0.5f);
			mTrans.localScale = new Vector3(s, s, 1.0f);
		}
	}
	
	void OnSceneScreenChanged() {
		Refresh();
	}
	
#if UNITY_EDITOR
	void Update () {
        if(Screen.height != mHeight) {
            Refresh();
            mHeight = Screen.height;
        }
	}
#endif
	
}

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/LayoutAnchor")]
public class NGUILayoutAnchor : MonoBehaviour {
	
	public enum DirectionHorizontal {
		Left,
		Right,
		Center,
		Stretch,
		None
	}
	
	public enum DirectionVertical {
		Top,
		Bottom,
		Center,
		Stretch,
		None
	}
	
	public Transform target;
	
	public Vector2 padding;
	
	public DirectionHorizontal dirHorz;
	public DirectionVertical dirVert;
	
	public bool rounding = true;
	
	public bool alwaysUpdate = false;
	
	private Transform mTrans;
	
	public void Reposition() {
		if(target != null) {
			Vector3 pos = mTrans.localPosition;
			Vector3 s = mTrans.localScale;
			
			Bounds targetBound = NGUIMath.CalculateRelativeWidgetBounds(mTrans.parent, target);
			
			switch(dirHorz) {
			case DirectionHorizontal.Left:
				pos.x = targetBound.min.x + padding.x;
				break;
			case DirectionHorizontal.Center:
				pos.x = targetBound.center.x + padding.x;
				break;
			case DirectionHorizontal.Right:
				pos.x = targetBound.max.x + padding.x;
				break;
			case DirectionHorizontal.Stretch:
				pos.x = targetBound.min.x + padding.x;
				s.x = targetBound.size.x - padding.x*2.0f;
				break;
			}
			
			switch(dirVert) {
			case DirectionVertical.Top:
				pos.y = targetBound.max.y + padding.y;
				break;
			case DirectionVertical.Center:
				pos.y = targetBound.center.y + padding.y;
				break;
			case DirectionVertical.Bottom:
				pos.y = targetBound.min.y + padding.y;
				break;
			case DirectionVertical.Stretch:
				pos.y = targetBound.max.y - padding.y;
				s.y = targetBound.size.y - padding.y*2.0f;
				break;
			}
			
			if(rounding) {
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				
				s.x = Mathf.Round(s.x);
				s.y = Mathf.Round(s.y);
			}
			
			mTrans.localPosition = pos;
			mTrans.localScale = s;
		}
	}
	
	void Awake() {
		mTrans = transform;
	}
		
	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		Reposition();
#else
		if(alwaysUpdate) {
			Reposition();
		}
#endif
	}
}

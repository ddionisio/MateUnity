using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu("M8/NGUI/LayoutFlow")]
public class NGUILayoutFlow : MonoBehaviour {
	
	public enum Arrangement
	{
		Horizontal,
		Vertical,
	}
	
	public enum LineUp {
		None,
		Center,
		End
	}
	
	public Arrangement arrangement = Arrangement.Horizontal;
	public float padding;
	public bool rounding = true;
	public bool repositionNow = false;
	public bool relativeLineup = false;
	
	public LineUp lineup = LineUp.None;
	
	void Start () {
		Reposition();
	}
	
	void Update () {
		if (repositionNow)
			Reposition();
	}
	
	public void Reposition () {		
		int count = transform.childCount;
		
		Bounds b;
		Bounds[] bounds = new Bounds[count];
		
		float bMax = float.MinValue, bMin = float.MaxValue;
		
		for(int i = 0; i < count; ++i) {
			Transform t = transform.GetChild(i);
			
			b = NGUIMath.CalculateRelativeWidgetBounds(t);
			Vector3 scale = t.localScale;
			b.min = Vector3.Scale(b.min, scale);
			b.max = Vector3.Scale(b.max, scale);
			bounds[i] = b;
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				if(bMax < b.max.y)
					bMax = b.max.y;
				if(bMin > b.min.y)
					bMin = b.min.y;
				break;
				
			case Arrangement.Vertical:
				if(bMax < b.max.x)
					bMax = b.max.x;
				if(bMin > b.min.x)
					bMin = b.min.x;
				break;
			}
		}
		
		float offset = 0;
		
		for(int i = 0; i < count; ++i) {
			Transform t = transform.GetChild(i);
			
			if (!t.gameObject.activeSelf) continue;
			
			b = bounds[i];
									
			Vector3 pos = t.localPosition;
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				pos.x = offset + b.extents.x - b.center.x;
				pos.y = relativeLineup ? 0 : -(b.extents.y + b.center.y) + (b.max.y - b.min.y - bMax + bMin) * 0.5f;
				
				offset += b.max.x - b.min.x + padding;
				break;
				
			case Arrangement.Vertical:
				pos.x = relativeLineup ? 0 : (b.extents.x - b.center.x) + (b.min.x - bMin);
				pos.y = -(offset + b.extents.y + b.center.y);
				
				offset += b.size.y + padding;
				break;
			}
			
			if(rounding && lineup == LineUp.None) {
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
			}

			t.localPosition = pos;
		}
		
		switch(lineup) {
		case LineUp.None:
			break;
			
		case LineUp.Center:
			b = NGUIMath.CalculateRelativeWidgetBounds(transform);
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					if(rounding) {
						pos.x = Mathf.Round(pos.x - b.extents.x);
						pos.y = Mathf.Round(pos.y);
					}
					
					t.localPosition = pos;
				}
				break;
				
			case Arrangement.Vertical:
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					if(rounding) {
						pos.x = Mathf.Round(pos.x);
						pos.y = Mathf.Round(pos.y + b.extents.y);
					}
					
					t.localPosition = pos;
				}
				break;
			}
			break;
			
		case LineUp.End:
			b = NGUIMath.CalculateRelativeWidgetBounds(transform);
			
			switch(arrangement) {
			case Arrangement.Horizontal:
				//TODO
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					if(rounding) {
						pos.x = Mathf.Round(pos.x - b.size.x);
						pos.y = Mathf.Round(pos.y);
					}
					
					t.localPosition = pos;
				}
				break;
				
			case Arrangement.Vertical:
				foreach(Transform t in transform) {
					Vector3 pos = t.localPosition;
					
					if(rounding) {
						pos.x = Mathf.Round(pos.x);
						pos.y = Mathf.Round(pos.y + b.size.y);// .extents.y);
					}
					
					t.localPosition = pos;
				}
				break;
			}
			break;
		}
		
		repositionNow = false;
	}
}
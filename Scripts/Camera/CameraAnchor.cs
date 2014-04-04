using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Camera/Anchor")]
[ExecuteInEditMode]
public class CameraAnchor : MonoBehaviour {
	public enum Anchor {
		Left,
		Right,
		Top,
		Bottom,
		Center,
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight
	}

	public Anchor anchor = Anchor.Center;

	public Vector2 offset = new Vector2(0.0f, 0.0f);

	[SerializeField]
	Camera _cameraAttach;

	public bool useFixedBound; //bound using the fixed resolution in camera2D

	private Camera2D mCamera2D;

	public Camera cameraAttach {
		get { return _cameraAttach; }
		set {
			if(_cameraAttach != value) {
				_cameraAttach = value;
				if(_cameraAttach)
					mCamera2D = _cameraAttach.GetComponent<Camera2D>();
			}
		}
	}

	public void Refresh() {
		if(_cameraAttach == null) return;

#if UNITY_EDITOR
		mCamera2D = _cameraAttach.GetComponent<Camera2D>();
#endif

		Transform t = transform;

		Vector3 pos = t.position;

		float scale = 1.0f;

		Rect r;
		if(mCamera2D != null && _cameraAttach.orthographic) {
			r = useFixedBound ? mCamera2D.fixedScreenExtent : mCamera2D.screenExtent;
			scale = mCamera2D.getPixelSize(1.0f);
		}
		else {
			r = _cameraAttach.pixelRect;
		}

		Vector3 anchorPos = Vector3.zero;

		switch(anchor) {
		case Anchor.Left:
			anchorPos.x = r.xMin; anchorPos.y = r.center.y;
			break;
		case Anchor.Right:
			anchorPos.x = r.xMax; anchorPos.y = r.center.y;
			break;
		case Anchor.Top:
			anchorPos.x = r.center.x; anchorPos.y = r.yMax;
			break;
		case Anchor.Bottom:
			anchorPos.x = r.center.x; anchorPos.y = r.yMin;
			break;
		case Anchor.Center:
			Vector2 c = r.center;
			anchorPos.x = c.x; anchorPos.y = c.y;
			break;
		case Anchor.TopLeft:
			anchorPos.x = r.xMin; anchorPos.y = r.yMax;
			break;
		case Anchor.TopRight:
			anchorPos.x = r.xMax; anchorPos.y = r.yMax;
			break;
		case Anchor.BottomLeft:
			anchorPos.x = r.xMin; anchorPos.y = r.yMin;
			break;
		case Anchor.BottomRight:
			anchorPos.x = r.xMax; anchorPos.y = r.yMin;
			break;
		}

		anchorPos.x += offset.x*scale;
		anchorPos.y += offset.y*scale;

		Vector3 newPos = mCamera2D != null ? mCamera2D.transform.position + anchorPos :
			_cameraAttach.ScreenToWorldPoint(anchorPos); newPos.z = pos.z;

		newPos.z = pos.z;

		if(pos != newPos)
			t.position = newPos;
	}

	void Awake() {
		if(_cameraAttach != null)
			mCamera2D = _cameraAttach.GetComponent<Camera2D>();
	}

	void Update() {
		Refresh();
	}
}

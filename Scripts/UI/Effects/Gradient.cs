using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Effects {
	/// <summary>
	/// Gradient coloring
	/// </summary>
	[AddComponentMenu("M8/UI/Effects/Gradient")]
	public class Gradient : BaseMeshEffect {
		public enum Mode {
			Horizontal,
			Vertical,
		}
				
		[SerializeField]
		Mode _mode = Mode.Vertical;
		[SerializeField]
		[Range(-1f, 1f)]
		float _offset = 0f;
		[SerializeField]
		Color _colorStart = Color.white;
		[SerializeField]
		Color _colorEnd = Color.black;

		public Mode mode {
			get { return _mode; } 
			set {
				if(_mode != value) {
					_mode = value;
					graphic.SetVerticesDirty();
				}
			}
		}

		public Color colorStart { 
			get { return _colorStart; } 
			set {
				if(_colorStart != value) {
					_colorStart = value;
					graphic.SetAllDirty();
				}
			}
		}

		public Color colorEnd { 
			get { return _colorEnd; }
			set {
				if(_colorEnd != value) {
					_colorEnd = value;
					graphic.SetAllDirty();
				}
			}
		}

		private Graphic mGraphic;

		private List<UIVertex> mVertList;

		protected override void Awake() {
			mGraphic = GetComponent<Graphic>();
		}

		public override void ModifyMesh(VertexHelper vh) {
			int count = vh.currentVertCount;
			if(!IsActive() || count == 0) {
				return;
			}
			
			var vtx = new UIVertex();

			if(mVertList == null)
				mVertList = new List<UIVertex>();
			else
				mVertList.Clear();

			vh.GetUIVertexStream(mVertList);

			var start = _mode == Mode.Vertical ? mVertList[mVertList.Count - 1].position.y : mVertList[mVertList.Count - 1].position.x;
			var end = _mode == Mode.Vertical ? mVertList[0].position.y : mVertList[0].position.x;
						
			var len = end - start;

			for(int i = 0; i < count; i++) {
				vh.PopulateUIVertex(ref vtx, i);

				var t = ((_mode == Mode.Vertical ? vtx.position.y : vtx.position.x) - start) / len;

				t = Mathf.Clamp01(t + _offset);

				vtx.color *= Color.Lerp(_colorEnd, _colorStart, t);

				vh.SetUIVertex(vtx, i);
			}
		}
	}
}
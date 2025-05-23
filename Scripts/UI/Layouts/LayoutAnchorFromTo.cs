using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace M8.UI.Layouts {
	/// <summary>
	/// Attach to start's position, orient and stretch towards end's position
	/// </summary>
	[ExecuteInEditMode]
	[AddComponentMenu("M8/UI/Layouts/Anchor From To")]
	public class LayoutAnchorFromTo : MonoBehaviour {
		public enum Orientation {
			Up,
			Down,
			Left,
			Right
		}

		public RectTransform source;
		public Transform from;
		public Transform to;

		[Header("Config")]
		public Orientation orientation = Orientation.Up;
		public bool stretch = true;

		[Header("Adjustments")]
		public float paddingStart;
		public float paddingEnd;

		private Vector3 mFromLastPos;
		private Vector3 mToLastPos;

		public void Apply() {
			if(!source)
				source = transform as RectTransform;

			if(!from || !to)
				return;

			mFromLastPos = from.position;
			mToLastPos = to.position;

			var sPos = new Vector2(mFromLastPos.x, mFromLastPos.y);
			var ePos = new Vector2(mToLastPos.x, mToLastPos.y);

			var dpos = ePos - sPos;
			var len = dpos.magnitude;

			if(len <= 0f)
				return;

			var dir = dpos / len;

			var pos = mFromLastPos;
			var pivot = source.pivot;
			var sizeDelta = source.sizeDelta;
			
			switch(orientation) {
				case Orientation.Up:
					pivot.y = 0f;

					source.up = dir;

					if(stretch)
						sizeDelta.y = len - paddingEnd;
					break;
				case Orientation.Down:
					pivot.y = 1f;

					source.up = -dir;

					if(stretch)
						sizeDelta.y = len - paddingEnd;
					break;
				case Orientation.Left:
					pivot.x = 1f;

					source.right = -dir;

					if(stretch)
						sizeDelta.x = len - paddingEnd;
					break;
				case Orientation.Right:
					pivot.x = 0f;

					source.right = dir;

					if(stretch)
						sizeDelta.x = len - paddingEnd;
					break;
			}

			if(paddingStart != 0f) {
				var ofs = dir * paddingStart;
				pos.x += ofs.x;
				pos.y += ofs.y;
			}

			source.position = pos;
			source.pivot = pivot;
			source.sizeDelta = sizeDelta;
		}

		void OnEnable() {
			Apply();
		}

		void Update() {
			if(!from || !to)
				return;

			if(mFromLastPos != from.position || mToLastPos != to.position)
				Apply();
		}
	}
}
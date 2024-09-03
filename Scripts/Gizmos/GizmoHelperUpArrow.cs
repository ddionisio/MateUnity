using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    //just to help visual circular area in the scene
    [AddComponentMenu("M8/Gizmo Helpers/Up Arrow")]
    public class GizmoHelperUpArrow : MonoBehaviour {
		public Color color = Color.white;
		public float length = 1f;

		private void OnDrawGizmos() {
			Gizmos.color = color;

			Gizmo.ArrowFourLine(transform.position, transform.up, length);
		}
	}
}
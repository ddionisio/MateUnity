using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CustomEditor(typeof(SpriteColorGradientGroup))]
	public class SpriteColorGradientGroupInspector : Editor {
		public override void OnInspectorGUI() {
			if(DrawDefaultInspector()) {
				var dat = target as SpriteColorGradientGroup;
				dat.Refresh();
			}
		}
	}
}
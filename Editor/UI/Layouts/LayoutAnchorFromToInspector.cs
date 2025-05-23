using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8.UI.Layouts {
	[CustomEditor(typeof(LayoutAnchorFromTo))]
	public class LayoutAnchorFromToInspector : Editor {

		void OnEnable() {
			((LayoutAnchorFromTo)target).Apply();
		}

		public override void OnInspectorGUI() {
			base.OnInspectorGUI();

			if(GUI.changed)
				((LayoutAnchorFromTo)target).Apply();
		}
	}
}
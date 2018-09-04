using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8.UI.Layouts {
    [CustomEditor(typeof(LayoutAnchorStretch))]
    public class LayoutAnchorStretchInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(GUI.changed)
                ((LayoutAnchorStretch)target).Apply();
        }
    }
}
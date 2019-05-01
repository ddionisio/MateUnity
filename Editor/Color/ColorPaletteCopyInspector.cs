using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(ColorPaletteCopy))]
    public class ColorPaletteCopyInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorExt.Utility.DrawSeparator();

            var dat = target as ColorPaletteCopy;

            if(GUILayout.Button("Apply"))
                dat.Apply();

            if(GUILayout.Button("Revert"))
                dat.Revert();
        }
    }
}
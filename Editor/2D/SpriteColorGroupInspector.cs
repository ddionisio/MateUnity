using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(SpriteColorGroup))]
    public class SpriteColorGroupInspector : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorExt.Utility.DrawSeparator();

            var dat = target as SpriteColorGroup;

            if(GUILayout.Button("Revert")) {
                dat.Revert();
            }
        }
    }
}
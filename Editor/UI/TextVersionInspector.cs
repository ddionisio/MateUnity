using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

namespace M8.UI {
    [CustomEditor(typeof(Texts.TextVersion), true)]
    public class TextVersionInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorExt.Utility.DrawSeparator();
            
            if(GUILayout.Button("Preview")) {
                var dat = target as Texts.TextVersion;
                dat.Apply();
            }
        }
    }
}
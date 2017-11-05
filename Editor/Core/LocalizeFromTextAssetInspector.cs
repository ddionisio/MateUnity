using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(LocalizeFromTextAsset))]
    public class LocalizeFromTextAssetInspector : Editor {
        public override void OnInspectorGUI() {
            if(GUILayout.Button("Edit")) {
                LocalizeFromTextAssetConfig.Open(target as LocalizeFromTextAsset);
            }

            M8.EditorExt.Utility.DrawSeparator();

            //
            var propTables = serializedObject.FindProperty("tables");

            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            GUI.enabled = propTables.arraySize > 0;
            if(GUILayout.Button("Clear Table")) {
                propTables.arraySize = 0;
            }

            GUI.backgroundColor = prevColor;
            //
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
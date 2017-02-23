using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(Localize))]
    public class LocalizeInspector : Editor {
        public override void OnInspectorGUI() {
            var propMode = serializedObject.FindProperty("mode");

            EditorGUILayout.PropertyField(propMode);

            var curMode = (Localize.Mode)propMode.enumValueIndex;

            switch(curMode) {
                case Localize.Mode.CustomLoader:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("loader"));
                    break;
                case Localize.Mode.TextAsset:
                    if(GUILayout.Button("Edit")) {
                        LocalizeConfig.Open(target as Localize);
                    }
                    break;

                default:
                    break;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(ColorPalette))]
    public class ColorPaletteInspector : Editor {
        public override void OnInspectorGUI() {
            var dat = target as ColorPalette;
            var palettesVar = serializedObject.FindProperty("_colors");

            int insertInd = -1;
            int deleteInd = -1;

            for(int i = 0; i < palettesVar.arraySize; i++) {
                var colorVar = palettesVar.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginHorizontal();

                var prevColorVal = colorVar.colorValue;

                EditorGUILayout.PropertyField(colorVar, new GUIContent(string.Format("{0:D2}.", i)));

                if(colorVar.colorValue != prevColorVal)
                    dat.Invoke(i);

                if(EditorExt.Utility.DrawAddButton())
                    insertInd = i;
                if(EditorExt.Utility.DrawRemoveButton())
                    deleteInd = i;

                EditorGUILayout.EndHorizontal();
            }

            if(insertInd != -1) {
                palettesVar.InsertArrayElementAtIndex(insertInd);
            }
            else if(deleteInd != -1) {
                palettesVar.DeleteArrayElementAtIndex(deleteInd);
            }

            if(GUILayout.Button("Add New Color"))
                palettesVar.arraySize = palettesVar.arraySize + 1;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
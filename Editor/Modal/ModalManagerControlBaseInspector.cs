using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(ModalManagerControlBase), true)]
    public class ModalManagerControlBaseInspector : Editor {
        public override void OnInspectorGUI() {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            var useMainVar = serializedObject.FindProperty("_modalMgrUseMain");

            EditorGUILayout.PropertyField(useMainVar, new GUIContent("Use Main"));

            if(!useMainVar.boolValue) {
                var mgrVar = serializedObject.FindProperty("_modalMgr");

                EditorGUILayout.PropertyField(mgrVar, new GUIContent("Modal Manager"));
            }

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            base.OnInspectorGUI();
        }
    }
}
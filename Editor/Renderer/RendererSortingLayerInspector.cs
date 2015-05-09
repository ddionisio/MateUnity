using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;

namespace M8 {
    [CustomEditor(typeof(RendererSortingLayer))]
    public class RendererSortingLayerInspector : Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty sortingLayerName = serializedObject.FindProperty("_sortingLayerName");
            SerializedProperty sortingOrder = serializedObject.FindProperty("_sortingOrder");

            Rect firstHoriz = EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();

            EditorGUI.BeginProperty(firstHoriz, GUIContent.none, sortingLayerName);

            string[] layerNames = GetSortingLayerNames();

            int selected = -1;
            //What is selected?
            string sName = sortingLayerName.stringValue;
            for(int i = 0; i < layerNames.Length; i++) {
                //Debug.Log(sID + " " + layerID[i]);
                if(sName == layerNames[i]) {
                    selected = i;
                }
            }

            if(selected == -1) {
                //Select Default.
                for(int i = 0; i < layerNames.Length; i++) {
                    if(layerNames[i] == "Default") {
                        selected = i;
                    }
                }
            }

            selected = EditorGUILayout.Popup("Sorting Layer", selected, layerNames);

            //Translate to ID
            sortingLayerName.stringValue = layerNames[selected];

            EditorGUI.EndProperty();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(sortingOrder, new GUIContent("Order in Layer"));


            EditorGUILayout.EndHorizontal();

            if(serializedObject.ApplyModifiedProperties()) {
                Renderer r = (target as MonoBehaviour).GetComponent<Renderer>();
                if(r) {
                    r.sortingLayerName = sortingLayerName.stringValue;
                    r.sortingOrder = sortingOrder.intValue;
                }
            }
        }

        public string[] GetSortingLayerNames() {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }

        public int[] GetSortingLayerUniqueIDs() {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayerUniqueIDsProperty = internalEditorUtilityType.GetProperty("sortingLayerUniqueIDs", BindingFlags.Static | BindingFlags.NonPublic);
            return (int[])sortingLayerUniqueIDsProperty.GetValue(null, new object[0]);
        }
    }
}
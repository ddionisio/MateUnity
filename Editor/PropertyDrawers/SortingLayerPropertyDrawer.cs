using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;
using System;

namespace M8 {
    [CustomPropertyDrawer(typeof(SortingLayerAttribute))]
    public class SortingLayerPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            string[] layerNames = GetSortingLayerNames();

            int selected = -1;
            //What is selected?
            string sName = property.stringValue;
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

            selected = EditorGUI.Popup(position, selected, layerNames);

            //Translate to ID
            property.stringValue = layerNames[selected];

            EditorGUI.EndProperty();
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
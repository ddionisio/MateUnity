using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(LocalizeAttribute))]
    public class LocalizePropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            property.stringValue = LocalizeConfig.DrawSelector(position, property.stringValue);

            EditorGUI.EndProperty();
        }
    }
}
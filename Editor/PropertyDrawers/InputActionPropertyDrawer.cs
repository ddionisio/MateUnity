using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(InputActionAttribute))]
    public class InputActionPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            property.intValue = EditorExt.InputBinder.GUISelectInputActionPopup(position, property.intValue);

            EditorGUI.EndProperty();
        }
    }
}
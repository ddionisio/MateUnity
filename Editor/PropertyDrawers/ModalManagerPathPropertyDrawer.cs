using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(ModalManagerPath))]
    public class ModalManagerPathPropertyDrawer : PropertyDrawer {

        SerializedProperty GetModeProperty(SerializedProperty property) {
            return property.FindPropertyRelative("mode");
        }

        SerializedProperty GetRefProperty(SerializedProperty property) {
            return property.FindPropertyRelative("managerRef");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var propMode = GetModeProperty(property);
            var propRef = GetRefProperty(property);

            var mode = (ModalManagerPath.Mode)propMode.enumValueIndex;

            switch(mode) {
                case ModalManagerPath.Mode.Main:
                    EditorGUI.PropertyField(position, propMode, new GUIContent(""));
                    break;

                case ModalManagerPath.Mode.Reference:
                    var halfHeight = position.height * 0.5f;

                    var modePos = new Rect(position.position, new Vector2(position.width, halfHeight));
                    var refPos = new Rect(new Vector2(position.position.x, position.position.y + halfHeight), new Vector2(position.width, halfHeight));

                    EditorGUI.PropertyField(modePos, propMode, new GUIContent(""));
                    EditorGUI.PropertyField(refPos, propRef, new GUIContent(""));
                    break;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = base.GetPropertyHeight(property, label);

            var modeProp = GetModeProperty(property);
            var mode = (ModalManagerPath.Mode)modeProp.enumValueIndex;

            switch(mode) {
                case ModalManagerPath.Mode.Reference:
                    return height * 2.0f;
                default:
                    return height;
            }
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(RangeInt))]
    public class RangeIntPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var propMin = property.FindPropertyRelative("min");
            var propMax = property.FindPropertyRelative("max");

            var minPos = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            var maxPos = new Rect(minPos.xMax + 4f, position.y, position.width * 0.5f - 4f, position.height);

            EditorGUIUtility.labelWidth = 45f;

            var minVal = EditorGUI.IntField(minPos, "min", propMin.intValue);
            var maxVal = EditorGUI.IntField(maxPos, "max", propMax.intValue);

            propMin.intValue = minVal;
            propMax.intValue = maxVal;

            EditorGUI.EndProperty();
        }
    }
}
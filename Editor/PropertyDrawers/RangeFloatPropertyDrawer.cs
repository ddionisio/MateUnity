using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(RangeFloat))]
    public class RangeFloatPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var propMin = property.FindPropertyRelative("min");
            var propMax = property.FindPropertyRelative("max");

            var minPos = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            var maxPos = new Rect(minPos.xMax, position.y, position.width * 0.5f, position.height);

            EditorGUIUtility.labelWidth = 42f;

            var minVal = propMin.floatValue;
            var maxVal = propMax.floatValue;

            minVal = EditorGUI.FloatField(minPos, "min", minVal);
            if(minVal > maxVal)
                maxVal = minVal;

            maxVal = EditorGUI.FloatField(maxPos, "max", maxVal);
            if(maxVal < minVal)
                minVal = maxVal;

            propMin.floatValue = minVal;
            propMax.floatValue = maxVal;

            EditorGUI.EndProperty();
        }
    }
}
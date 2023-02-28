using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(GenericParamSerialized))]
    public class GenericParamFieldInspector : PropertyDrawer {
        private string[] boolDisplays = new string[] { "False", "True" };

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var propKey = property.FindPropertyRelative("key");
            var propType = property.FindPropertyRelative("type");

            var keyPos = position; keyPos.width *= 0.3f;

            propKey.stringValue = EditorGUI.TextField(keyPos, propKey.stringValue);

            var typePos = position; typePos.x = keyPos.xMax + 8f; typePos.width *= 0.22f;

            propType.enumValueIndex = EditorGUI.Popup(typePos, propType.enumValueIndex, propType.enumDisplayNames);

            var valPos = position; valPos.x = typePos.xMax + 4f; valPos.width = position.xMax - valPos.x;

            var propValInt = property.FindPropertyRelative("iVal");
            var propValVec = property.FindPropertyRelative("vectorVal");
            var propValStr = property.FindPropertyRelative("sVal");
            var propValObj = property.FindPropertyRelative("oVal");

            var vVal = propValVec.vector4Value;

            switch((GenericParamSerialized.ValueType)propType.enumValueIndex) {
                case GenericParamSerialized.ValueType.Boolean:
                    propValInt.intValue = EditorGUI.Popup(valPos, propValInt.intValue, boolDisplays);
                    break;
                case GenericParamSerialized.ValueType.Int:
                    propValInt.intValue = EditorGUI.IntField(valPos, propValInt.intValue);
                    break;
                case GenericParamSerialized.ValueType.Float:
                    vVal.x = EditorGUI.FloatField(valPos, vVal.x);
                    propValVec.vector4Value = vVal;
                    break;
                case GenericParamSerialized.ValueType.String:
                    propValStr.stringValue = EditorGUI.TextField(valPos, propValStr.stringValue);
                    break;
                case GenericParamSerialized.ValueType.Vector2:
                    propValVec.vector4Value = EditorGUI.Vector2Field(valPos, "", new Vector2 { x=vVal.x, y=vVal.y });
                    break;
                case GenericParamSerialized.ValueType.Vector3:
                    propValVec.vector4Value = EditorGUI.Vector3Field(valPos, "", new Vector3 { x = vVal.x, y = vVal.y, z = vVal.z });
                    break;
                case GenericParamSerialized.ValueType.Vector4:
                    propValVec.vector4Value = EditorGUI.Vector4Field(valPos, "", vVal);
                    break;
                case GenericParamSerialized.ValueType.Object:
                    EditorGUI.ObjectField(valPos, propValObj, new GUIContent());
                    break;
            }

            EditorGUI.EndProperty();
        }
    }
}
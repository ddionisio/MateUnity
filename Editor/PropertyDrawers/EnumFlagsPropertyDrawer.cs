using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomPropertyDrawer(typeof(EnumMaskAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer {
        private string[] mEnumNames;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if(mEnumNames == null) {
                //find the field and extract Type from there
                var selectObjType = property.serializedObject.targetObject.GetType();
                var fieldInfo = selectObjType.GetField(property.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                var enumType = fieldInfo.FieldType;
                                
                var enumValues = Enum.GetValues(enumType);

                //ensure it is ordered
                Array.Sort(enumValues, new EnumValueComparer());

                //var enumNames = Enum.GetNames(enumType);

                List<string> enumNameList = new List<string>();
                for(int i = 0; i < enumValues.Length; i++) {
                    var enumItem = enumValues.GetValue(i);
                    int val = Convert.ToInt32(enumItem);
                    if(val != 0)
                        enumNameList.Add(enumItem.ToString());
                }

                mEnumNames = enumNameList.ToArray();
            }

            int curFlags = property.intValue;

            int newFlags = EditorGUI.MaskField(position, curFlags, mEnumNames);

            property.intValue = newFlags;

            EditorGUI.EndProperty();
        }

        private class EnumValueComparer : IComparer {
            public int Compare(object x, object y) {
                int xVal = Convert.ToInt32(x);
                int yVal = Convert.ToInt32(y);

                return xVal - yVal;
            }
        }
    }
}
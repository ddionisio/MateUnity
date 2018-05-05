using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomPropertyDrawer(typeof(RangeMinMaxAttribute))]
    public class RangeLimitAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //grab limits
            var rangeMinMaxAttr = this.attribute as RangeMinMaxAttribute;

            //grab range values
            string minFieldName = "min";
            if(!string.IsNullOrEmpty(rangeMinMaxAttr.minFieldName))
                minFieldName = rangeMinMaxAttr.minFieldName;

            string maxFieldName = "max";
            if(!string.IsNullOrEmpty(rangeMinMaxAttr.maxFieldName))
                maxFieldName = rangeMinMaxAttr.maxFieldName;

            var propMin = property.FindPropertyRelative(minFieldName);
            var propMax = property.FindPropertyRelative(maxFieldName);

            //TODO: better to check actual type than the type name itself
            bool isInt = propMin.type == "int";
            bool isFloat = propMin.type == "float";

            if(propMin == null || propMax == null) {
                GUI.Label(position, string.Format("Unable to find one of the fields: {0}, {1}", minFieldName, maxFieldName));
            }
            if(propMin.type != propMax.type) {
                GUI.Label(position, string.Format("{0} and {1} need to be of the same type.", minFieldName, maxFieldName));
            }
            else if(!isInt && !isFloat) {
                GUI.Label(position, string.Format("{0} and {1} need to be either float or int.", minFieldName, maxFieldName));
            }
            else {
                float min, max;

                //cast to float if it's int
                if(isInt) {
                    min = propMin.intValue;
                    max = propMax.intValue;
                }
                else {
                    min = propMin.floatValue;
                    max = propMax.floatValue;
                }

                //edit
                float baseHeight = base.GetPropertyHeight(property, label);

                var sliderPos = position; sliderPos.height = baseHeight;

                EditorGUI.MinMaxSlider(sliderPos, ref min, ref max, rangeMinMaxAttr.minLimit, rangeMinMaxAttr.maxLimit);

                //min val
                var valMinPos = position; valMinPos.y += baseHeight; valMinPos.width = position.width * 0.5f; valMinPos.height = baseHeight;

                var minLabelStyle = new GUIStyle(GUI.skin.label);
                minLabelStyle.alignment = TextAnchor.MiddleLeft;

                GUI.Label(valMinPos, min.ToString(), minLabelStyle);
                //

                //max val
                var valMaxPos = position; valMaxPos.x += position.width * 0.5f; valMaxPos.y += baseHeight; valMaxPos.width = position.width * 0.5f; valMaxPos.height = baseHeight;

                var maxLabelStyle = new GUIStyle(GUI.skin.label);
                maxLabelStyle.alignment = TextAnchor.MiddleRight;

                GUI.Label(valMaxPos, max.ToString(), maxLabelStyle);
                //

                if(isInt) {
                    propMin.intValue = Mathf.RoundToInt(min);
                    propMax.intValue = Mathf.RoundToInt(max);
                }
                else {
                    propMin.floatValue = min;
                    propMax.floatValue = max;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            //double the height to allow min and max value
            float baseHeight = base.GetPropertyHeight(property, label);
            return baseHeight * 2.0f;
        }
    }
}
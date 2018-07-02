using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8.EditorExt {
    public class UnityInputManagerAxesDisplay {
        public string[] axisNames { get { return mAxisNames; } }

        private string[] mAxisNames;
        private int[] mAxisIndices;

        public void Init() {
            var inputMgr = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset");

            var serializedObj = new SerializedObject(inputMgr);

            var axisArray = serializedObj.FindProperty("m_Axes");

            mAxisNames = new string[axisArray.arraySize];
            mAxisIndices = new int[axisArray.arraySize];

            for(int i = 0; i < axisArray.arraySize; i++) {
                var axis = axisArray.GetArrayElementAtIndex(i);

                mAxisNames[i] = axis.FindPropertyRelative("m_Name").stringValue;
                mAxisIndices[i] = i;
            }
        }

        public string FirstAxisName() {
            return mAxisNames.Length > 0 ? mAxisNames[0] : "";
        }

        public int GetAxisNameIndex(string axisName) {
            if(!string.IsNullOrEmpty(axisName)) {
                for(int i = 0; i < mAxisNames.Length; i++) {
                    if(mAxisNames[i] == axisName)
                        return i;
                }
            }

            return -1;
        }

        public string AxisNamePopupLayout(string currentAxisName) {
            int curInd = GetAxisNameIndex(currentAxisName);
            if(curInd == -1)
                curInd = 0;

            int newInd = AxisNamePopupLayout(curInd);

            return mAxisNames[newInd];
        }

        public string AxisNamePopupLayout(string label, string currentAxisName) {
            int curInd = GetAxisNameIndex(currentAxisName);
            if(curInd == -1)
                curInd = 0;

            int newInd = AxisNamePopupLayout(label, curInd);

            return mAxisNames[newInd];
        }

        public int AxisNamePopupLayout(int currentAxisNameIndex) {
            return EditorGUILayout.IntPopup(currentAxisNameIndex, mAxisNames, mAxisIndices);
        }

        public int AxisNamePopupLayout(string label, int currentAxisNameIndex) {
            return EditorGUILayout.IntPopup(label, currentAxisNameIndex, mAxisNames, mAxisIndices);
        }
    }
}
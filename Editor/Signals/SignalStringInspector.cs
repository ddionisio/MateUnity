using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(SignalString))]
    public class SignalStringInspector : Editor {
        private string mVal;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //Test options during Play mode
            if(Application.isPlaying) {
                var signal = target as SignalString;

                EditorExt.Utility.DrawSeparator();

                mVal = EditorGUILayout.TextField("Value", mVal);

                if(GUILayout.Button("Invoke"))
                    signal.Invoke(mVal);
            }
        }
    }
}
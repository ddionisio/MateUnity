using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(SignalInteger))]
    public class SignalIntegerInspector : Editor {
        private int mVal;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //Test options during Play mode
            if(Application.isPlaying) {
                var signal = target as SignalInteger;

                EditorExt.Utility.DrawSeparator();

                mVal = EditorGUILayout.IntField("Value", mVal);

                if(GUILayout.Button("Invoke"))
                    signal.Invoke(mVal);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(SignalFloat))]
    public class SignalFloatInspector : Editor {
        private float mVal;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //Test options during Play mode
            if(Application.isPlaying) {
                var signal = target as SignalFloat;

                EditorExt.Utility.DrawSeparator();

                mVal = EditorGUILayout.FloatField("Value", mVal);

                if(GUILayout.Button("Invoke"))
                    signal.Invoke(mVal);
            }
        }
    }
}
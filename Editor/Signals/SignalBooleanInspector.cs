using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(SignalBoolean))]
    public class SignalBooleanInspector : Editor {
        private bool mVal;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //Test options during Play mode
            if(Application.isPlaying) {
                var signal = target as SignalBoolean;

                EditorExt.Utility.DrawSeparator();

                mVal = EditorGUILayout.Toggle("Value", mVal);

                if(GUILayout.Button("Invoke"))
                    signal.Invoke(mVal);
            }
        }
    }
}
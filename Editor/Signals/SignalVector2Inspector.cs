using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(SignalVector2))]
    public class SignalVector2Inspector : Editor {
        private Vector2 mVal;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //Test options during Play mode
            if(Application.isPlaying) {
                var signal = target as SignalVector2;

                EditorExt.Utility.DrawSeparator();

                mVal = EditorGUILayout.Vector2Field("Value", mVal);

                if(GUILayout.Button("Invoke"))
                    signal.Invoke(mVal);
            }
        }
    }
}
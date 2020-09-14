using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(Signal))]
    public class SignalInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //Test options during Play mode
            if(Application.isPlaying) {
                var signal = target as Signal;

                EditorExt.Utility.DrawSeparator();

                if(GUILayout.Button("Invoke"))
                    signal.Invoke();
            }
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(Localize))]
    public class LocalizeInspector : Editor {
        public override void OnInspectorGUI() {
            if(GUILayout.Button("Edit")) {
                LocalizeConfig.Open(target as Localize);
            }
        }
    }
}
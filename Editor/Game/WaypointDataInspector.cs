using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(WaypointData))]
    public class WaypointDataInspector : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(Application.isPlaying) {
                M8.EditorExt.Utility.DrawSeparator();

                WaypointData input = target as WaypointData;

                EditorGUILayout.LabelField("Current Index", input.currentIndex.ToString());
            }
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(SpriteClipAngle))]
    public class SpriteClipAngleInspector : Editor {

        public override void OnInspectorGUI() {
            var angleField = serializedObject.FindProperty("_angle");
            var angleMinField = serializedObject.FindProperty("_angleMin");
            var angleMaxField = serializedObject.FindProperty("_angleMax");

            EditorGUILayout.Slider(angleField, 0f, 360f, "Offset");
            EditorGUILayout.Slider(angleMinField, 0f, 360f, "Start");
            EditorGUILayout.Slider(angleMaxField, 0f, 360f, "End");

            if(serializedObject.ApplyModifiedProperties()) {
                var tgt = target as SpriteClipAngle;
                tgt.Refresh();
            }
        }
    }
}
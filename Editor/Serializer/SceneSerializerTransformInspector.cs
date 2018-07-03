using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(SceneSerializerTransform))]
    public class SceneSerializerTransformInspector : Editor {
        public override void OnInspectorGUI() {
            SceneSerializerTransform data = target as SceneSerializerTransform;

            EditorGUI.BeginChangeCheck();

            var isLocal = EditorGUILayout.Toggle("local", data.isLocal);

            var positionFlags = (SceneSerializerTransform.Axis)EditorGUILayout.EnumFlagsField("position", data.positionFlags);
            var rotationFlags = (SceneSerializerTransform.Axis)EditorGUILayout.EnumFlagsField("rotation", data.rotationFlags);

            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Change Scene Serializer");

                data.isLocal = isLocal;

                data.positionFlags = positionFlags;
                data.rotationFlags = rotationFlags;
            }
        }
    }
}
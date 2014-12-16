using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(SceneSerializerTransform))]
    public class SceneSerializerTransformInspector : Editor {
        public override void OnInspectorGUI() {
            SceneSerializerTransform data = target as SceneSerializerTransform;

            data.isLocal = EditorGUILayout.Toggle("local", data.isLocal);
            data.persistent = EditorGUILayout.Toggle("persistent", data.persistent);

            data.positionFlags = (SceneSerializerTransform.Axis)EditorGUILayout.EnumMaskField("position", data.positionFlags);
            data.rotationFlags = (SceneSerializerTransform.Axis)EditorGUILayout.EnumMaskField("rotation", data.rotationFlags);

            if(GUI.changed)
                EditorUtility.SetDirty(data);
        }
    }
}
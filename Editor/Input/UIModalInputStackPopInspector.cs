using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIModalInputStackPop))]
public class UIModalInputStackPopInspector : Editor {
    public override void OnInspectorGUI() {
        UIModalInputStackPop obj = target as UIModalInputStackPop;

        obj.player = EditorGUILayout.IntField("Player", obj.player);

        obj.escape = M8.Editor.InputMapper.GUISelectInputAction("Escape", obj.escape);
    }
}

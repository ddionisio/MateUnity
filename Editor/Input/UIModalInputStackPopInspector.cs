using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIModalInputStackPop))]
public class UIModalInputStackPopInspector : Editor {
    public override void OnInspectorGUI() {
        GUI.changed = false;

        UIModalInputStackPop obj = target as UIModalInputStackPop;

        obj.player = EditorGUILayout.IntField("Player", obj.player);

        obj.escape = M8.Editor.InputBinder.GUISelectInputAction("Escape", obj.escape);

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

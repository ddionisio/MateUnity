using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using UIModalInputStackPop = M8.UIModal.Input.CloseTop;

[CustomEditor(typeof(UIModalInputStackPop))]
public class UIModalInputStackPopInspector : Editor {
    public override void OnInspectorGUI() {
        UIModalInputStackPop obj = target as UIModalInputStackPop;

        EditorGUI.BeginChangeCheck();

        var player = EditorGUILayout.IntField("Player", obj.player);

        var escape = M8.EditorExt.InputBinder.GUISelectInputAction("Escape", obj.escape);

        if(EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Change Input Stack Pop Key");

            obj.player = player;
            obj.escape = escape;
        }
    }
}

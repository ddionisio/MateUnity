using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using UIModalInputStackPop = M8.UIModal.Input.CloseTop;

[CustomEditor(typeof(UIModalInputStackPop))]
public class UIModalInputStackPopInspector : Editor {
    public override void OnInspectorGUI() {
        GUI.changed = false;

        UIModalInputStackPop obj = target as UIModalInputStackPop;

        obj.player = EditorGUILayout.IntField("Player", obj.player);

        obj.escape = M8.EditorExt.InputBinder.GUISelectInputAction("Escape", obj.escape);

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

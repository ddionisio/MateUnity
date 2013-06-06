using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NGUIInputClick))]
public class NGUIInputClickInspector : Editor {

    public override void OnInspectorGUI() {
        GUI.changed = false;

        NGUIInputClick input = target as NGUIInputClick;

        input.player = EditorGUILayout.IntField("Player", input.player);

        input.action = M8.Editor.InputMapper.GUISelectInputAction("Action", input.action);
        input.alternate = M8.Editor.InputMapper.GUISelectInputAction("Alt. Action", input.alternate);

        input.axisCheck = EditorGUILayout.FloatField("Axis Check", input.axisCheck);
        input.axisDelay = EditorGUILayout.FloatField("Axis Delay", input.axisDelay);

        input.checkSelected = EditorGUILayout.Toggle("Check Selected", input.checkSelected);

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

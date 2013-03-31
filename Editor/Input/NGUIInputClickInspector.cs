using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(NGUIInputClick))]
public class NGUIInputClickInspector : Editor {

    public override void OnInspectorGUI() {
        NGUIInputClick input = target as NGUIInputClick;

        input.action = M8.Editor.InputMapper.GUISelectInputAction("Action", input.action);
        input.alternate = M8.Editor.InputMapper.GUISelectInputAction("Alt. Action", input.alternate);

        input.checkSelected = EditorGUILayout.Toggle("Check Selected", input.checkSelected);
    }
}

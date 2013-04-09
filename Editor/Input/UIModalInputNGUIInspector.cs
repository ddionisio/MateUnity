using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(UIModalInputNGUI))]
public class UIModalInputNGUIInspector : Editor {
    public override void OnInspectorGUI() {
        UIModalInputNGUI obj = target as UIModalInputNGUI;

        obj.player = EditorGUILayout.IntField("Player", obj.player);

        obj.axisX = M8.Editor.InputMapper.GUISelectInputAction("Axis X", obj.axisX);
        obj.axisY = M8.Editor.InputMapper.GUISelectInputAction("Axis Y", obj.axisY);

        obj.axisDelay = EditorGUILayout.FloatField("Axis Delay", obj.axisDelay);
        obj.axisThreshold = EditorGUILayout.FloatField("Axis Threshold", obj.axisThreshold);

        obj.enter = M8.Editor.InputMapper.GUISelectInputAction("Enter", obj.enter);
        obj.cancel = M8.Editor.InputMapper.GUISelectInputAction("Cancel", obj.cancel);
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(WaypointData))]
public class WaypointDataInspector : Editor {

    public override void OnInspectorGUI() {
        GUI.changed = false;

        base.OnInspectorGUI();

        if(Application.isPlaying) {
            M8.Editor.Utility.DrawSeparator();

            WaypointData input = target as WaypointData;

            EditorGUILayout.LabelField("Current Index", input.currentIndex.ToString());
        }

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

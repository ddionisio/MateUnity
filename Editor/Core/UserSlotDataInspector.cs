using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(UserSlotData))]
public class UserSlotDataInspector : UserDataInspector {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(Application.isPlaying) {
            M8.Editor.Utility.DrawSeparator();

            EditorGUILayout.LabelField("Current Slot", ((UserSlotData)target).slot.ToString());
        }
    }
}

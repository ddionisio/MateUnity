using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(UserData))]
public class UserDataInspector : Editor {
    private bool mDetailFold;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(Application.isPlaying) {
            M8.Editor.Utility.DrawSeparator();

            if(mDetailFold = EditorGUILayout.Foldout(mDetailFold, "Items")) {
                UserData dat = target as UserData;

                GUILayout.BeginVertical();

                foreach(KeyValuePair<string, object> itm in dat) {
                    EditorGUILayout.LabelField(itm.Key, itm.Value.ToString());
                }

                GUILayout.EndVertical();
            }
        }
    }
}

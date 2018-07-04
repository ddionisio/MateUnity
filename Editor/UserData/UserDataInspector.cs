using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomEditor(typeof(UserData), true)]
    public class UserDataInspector : Editor {
        private bool mDetailFold;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(Application.isPlaying) {
                M8.EditorExt.Utility.DrawSeparator();

                if(mDetailFold = EditorGUILayout.Foldout(mDetailFold, "Items")) {
                    UserData dat = target as UserData;
                    if(dat.valueCount > 0) {
                        GUILayout.BeginVertical();

                        foreach(KeyValuePair<string, object> itm in dat) {
                            EditorGUILayout.LabelField(itm.Key, itm.Value.ToString());
                        }

                        GUILayout.EndVertical();
                    }
                }
            }
        }
    }
}
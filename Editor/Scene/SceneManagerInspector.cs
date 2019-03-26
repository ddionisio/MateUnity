using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomEditor(typeof(SceneManager))]
    public class SceneManagerInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(Application.isPlaying) {
                EditorExt.Utility.DrawSeparator();

                var sceneMgr = target as SceneManager;

                if(GUILayout.Button("Scene Reload")) {
                    sceneMgr.Reload();
                }
            }
        }
    }
}
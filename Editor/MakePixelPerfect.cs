using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace M8.Editor {
    public class MakePixelPerfect : EditorWindow {

        [MenuItem("M8/2D/Make Pixel-Perfect")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(MakePixelPerfect));
        }

        private bool mRecursive = true;
        
        private bool mXNudge = false;
        private bool mYNudge = false;

        private Vector2 mScrollPos = Vector2.zero;

        void OnGUI() {
            EditorGUIUtility.LookLikeControls(80.0f);

            GUI.skin.label.wordWrap = true;

            Object[] objs = Selection.objects;

            NGUIEditorTools.DrawSeparator();

            mRecursive = GUILayout.Toggle(mRecursive, "Recursive");
            mXNudge = GUILayout.Toggle(mXNudge, "Nudge X");
            mYNudge = GUILayout.Toggle(mYNudge, "Nudge Y");

            if(GUILayout.Button("Apply")) {
                foreach(Object obj in objs) {
                    GameObject go = obj as GameObject;
                    if(go != null) {
                        Apply(go);

                        Vector3 pos = go.transform.localPosition;
                        if(mXNudge) pos.x += 0.5f;
                        if(mYNudge) pos.y += 0.5f;
                        go.transform.localPosition = pos;
                    }
                }

                SceneView.RepaintAll();
                
            }

            NGUIEditorTools.DrawSeparator();

            GUILayout.Label("Selected Objects:");

            mScrollPos = GUILayout.BeginScrollView(mScrollPos);

            foreach(Object obj in objs) {
                GameObject go = obj as GameObject;
                if(go != null)
                    GUILayout.Label(go.name);
            }

            GUILayout.EndScrollView();
        }

        void Apply(GameObject go) {
            //check for ngui
            UIWidget widget = go.GetComponent<UIWidget>();
            if(widget != null)
                widget.MakePixelPerfect();
            else {
                Vector3 scale = go.transform.localScale;
                int width = Mathf.RoundToInt(scale.x);
                int height = Mathf.RoundToInt(scale.y);

                scale.x = width;
                scale.y = height;
                scale.z = 1f;
                go.transform.localScale = scale;

                Vector3 pos = go.transform.localPosition;
                pos.x = Mathf.Round(pos.x);
                pos.y = Mathf.Round(pos.y);
                go.transform.localPosition = pos;
            }

            if(mRecursive) {
                foreach(Transform t in go.transform)
                    Apply(t.gameObject);
            }
        }
    }
}

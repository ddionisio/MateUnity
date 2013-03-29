using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace M8.Editor {
    public class tk2dSetColorHierarchy : EditorWindow {

        [MenuItem("M8/tk2d/Set Color Hierarchy")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(tk2dSetColorHierarchy));
        }

        private Color mColor = Color.white;
        private bool mIncludeInactive = true;
        private Vector2 mScrollPos = Vector2.zero;

        void OnGUI() {
            EditorGUIUtility.LookLikeControls(80.0f);

            GUI.skin.label.wordWrap = true;

            Object[] objs = Selection.objects;

            mColor = EditorGUILayout.ColorField(mColor);

            NGUIEditorTools.DrawSeparator();

            mIncludeInactive = GUILayout.Toggle(mIncludeInactive, "Include Inactive");

            if(GUILayout.Button("Apply")) {
                foreach(Object obj in objs) {
                    GameObject go = obj as GameObject;
                    if(go != null)
                        ApplyColorHierarchy(go);
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

        void ApplyColorHierarchy(GameObject go) {
            tk2dBaseSprite[] sprites = go.GetComponentsInChildren<tk2dBaseSprite>(mIncludeInactive);
            foreach(tk2dBaseSprite spr in sprites) {
                spr.color = mColor;
            }
        }
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace M8.Editor {
    public struct Utility {
        public static Texture2D blankTexture {
            get {
                return EditorGUIUtility.whiteTexture;
            }
        }

        public static void DrawSeparator() {
            GUILayout.Space(12f);

            if(Event.current.type == EventType.Repaint) {
                Texture2D tex = blankTexture;
                Rect rect = GUILayoutUtility.GetLastRect();
                GUI.color = new Color(0f, 0f, 0f, 0.25f);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 4f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 6f, Screen.width, 1f), tex);
                GUI.DrawTexture(new Rect(0f, rect.yMin + 9f, Screen.width, 1f), tex);
                GUI.color = Color.white;
            }
        }

        public static bool IsDirectory(Object obj) {
            string path = AssetDatabase.GetAssetPath(obj);
            FileAttributes attr = File.GetAttributes(path);
            return (attr & FileAttributes.Directory) != 0;
        }

        public static string FindPath(string filename, string dir, bool recursive) {
            string[] paths = Directory.GetFiles(dir, filename, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            return paths.Length > 0 ? paths[0].Replace('\\', '/') : null;
        }

        public static string[] GetPaths(Object objDir, string pattern, bool recursive) {
            string path = AssetDatabase.GetAssetPath(objDir);
            FileAttributes attr = File.GetAttributes(path);

            if((attr & FileAttributes.Directory) != 0) {
                string[] paths = Directory.GetFiles(path, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                for(int i = 0; i < paths.Length; i++) {
                    paths[i] = paths[i].Replace('\\', '/');
                }

                return paths;
            }
            else {
                return new string[0];
            }
        }

        public static string GetSelectionFolder() {
            if(Selection.activeObject != null) {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());

                if(!string.IsNullOrEmpty(path)) {
                    int dot = path.LastIndexOf('.');
                    int slash = Mathf.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));
                    if(slash > 0) return (dot > slash) ? path.Substring(0, slash + 1) : path + "/";
                }
            }
            return "Assets/";
        }

        public static string GetTextContent(string filepath) {
            string ret = null;

            try {
                ret = File.ReadAllText(filepath);
            }
            catch(System.Exception e) {
                Debug.LogWarning(e);
            }

            return ret;
        }
    }
}

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace M8.Editor {
    public struct Utility {
        public const char ArrowUpChar = '\u25B2';
        public const char ArrowDownChar = '\u25BC';

        public static Texture2D blankTexture {
            get {
                return EditorGUIUtility.whiteTexture;
            }
        }

        public static IEnumerable<GameObject> sceneRoots {
            get {
                var prop = new HierarchyProperty(HierarchyType.GameObjects);
                var expanded = new int[0];
                while(prop.Next(expanded))
                    yield return prop.pptrValue as GameObject;
            }
        }

        public static int sceneRootCount {
            get {
                int count = 0;
                var prop = new HierarchyProperty(HierarchyType.GameObjects);
                var expanded = new int[0];
                while(prop.Next(expanded))
                    count++;
                return count;
            }
        }

        /// <summary>
        /// Iterate through all transforms in the scene, breath-first.
        /// </summary>
        public static IEnumerable<Transform> sceneAllObjects {
            get {
                var q = new Queue<Transform>();
                foreach(var root in sceneRoots) {
                    var t = root.transform;
                    yield return t;
                    q.Enqueue(t);
                }

                while(q.Count > 0) {
                    foreach(Transform child in q.Dequeue()) {
                        yield return child;
                        q.Enqueue(child);
                    }
                }
            }
        }

        public static string[] GenerateGenericMaskString() {
            string[] ret = new string[32];
            for(int i = 0; i < ret.Length; i++) {
                ret[i] = i.ToString();
            }

            return ret;
        }

        public static string[] GenerateLayerMaskString() {
            string[] ret = new string[32];
            for(int i = 0; i < 32; i++) {
                string layerName = LayerMask.LayerToName(i);
                ret[i] = !string.IsNullOrEmpty(layerName) ? layerName : i.ToString();
            }

            return ret;
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

        public static bool DrawAddButton(string toolTip = "Click to add") {
            return GUILayout.Button(new GUIContent("+", toolTip), EditorStyles.toolbarButton);
        }

        public static bool DrawRemoveButton(string toolTip = "Click to remove") {
            return GUILayout.Button(new GUIContent("-", toolTip), EditorStyles.toolbarButton);
        }

        public static bool DrawCopyButton(string toolTip = "Click to copy") {
            return GUILayout.Button(new GUIContent("C", toolTip), EditorStyles.toolbarButton);
        }

        /// <summary>
        /// Used by AddFoldOutListItemButtons to return which button was pressed, and by 
        /// UpdateFoldOutListOnButtonPressed to process the pressed button for regular lists
        /// </summary>
        public enum ListButtonResult { None, Up, Down, Add, Remove }

        /// <summary>
        /// Adds the buttons which control a list item
        /// </summary>
        /// <returns>LIST_BUTTONS - The LIST_BUTTONS pressed or LIST_BUTTONS.None</returns>
        public static ListButtonResult DrawListButtons() {
            #region Layout
            int buttonSpacer = 6;

            EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(100));
            // The up arrow will move things towards the beginning of the List
            var upArrow = '\u25B2'.ToString();
            bool upPressed = GUILayout.Button(new GUIContent(upArrow, "Click to shift up"),
                                              EditorStyles.toolbarButton);

            // The down arrow will move things towards the end of the List
            var dnArrow = '\u25BC'.ToString();
            bool downPressed = GUILayout.Button(new GUIContent(dnArrow, "Click to shift down"),
                                                EditorStyles.toolbarButton);

            // A little space between button groups
            GUILayout.Space(buttonSpacer);

            // Remove Button - Process presses later
            bool removePressed = GUILayout.Button(new GUIContent("-", "Click to remove"),
                                                  EditorStyles.toolbarButton);

            // Add button - Process presses later
            bool addPressed = GUILayout.Button(new GUIContent("+", "Click to insert new"),
                                               EditorStyles.toolbarButton);

            EditorGUILayout.EndHorizontal();
            #endregion Layout

            // Return the pressed button if any
            if(upPressed == true) return ListButtonResult.Up;
            if(downPressed == true) return ListButtonResult.Down;
            if(removePressed == true) return ListButtonResult.Remove;
            if(addPressed == true) return ListButtonResult.Add;

            return ListButtonResult.None;
        }

        /// <summary>
        /// Helper function for editing a string array, note that 'size' should not be data.Length, this needs to be separate.
        /// Also, label is used as the focus control when editing the count.
        /// </summary>
        public static void DrawStringArray(string label, ref bool foldout, ref string[] data, ref int size) {
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();

            if(data == null) {
                data = new string[0];
            }

            foldout = EditorGUILayout.Foldout(foldout, label);

            GUILayout.FlexibleSpace();

            //EditorGUIUtility.LookLikeControls(0, 50);

            bool enterPressed = Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Return;

            GUILayout.Label("count", GUILayout.MaxWidth(50));

            string ctrlName = label;

            GUI.SetNextControlName(ctrlName);
            size = EditorGUILayout.IntField(size, GUILayout.MaxWidth(50));

            GUILayout.EndHorizontal();

            if(GUI.GetNameOfFocusedControl() == ctrlName) {
                if(enterPressed && size != data.Length) {
                    if(size < 0) size = 0;

                    System.Array.Resize(ref data, size);

                    foldout = true;
                }
            }
            else {
                size = data.Length;
            }

            if(foldout) {
                EditorGUIUtility.LookLikeControls(20, 200);

                ListButtonResult act = ListButtonResult.None;
                int actInd = -1;

                for(int i = 0; i < data.Length; i++) {
                    GUILayout.BeginHorizontal();

                    data[i] = EditorGUILayout.TextField(i.ToString(), data[i]);

                    GUILayout.FlexibleSpace();

                    if(act == Editor.Utility.ListButtonResult.None) {
                        act = DrawListButtons();
                        if(act != ListButtonResult.None)
                            actInd = i;
                    }

                    GUILayout.EndHorizontal();
                }

                if(actInd != -1) {
                    switch(act) {
                        case ListButtonResult.Add:
                            string copy = data[actInd];
                            M8.ArrayUtil.InsertAfter(ref data, actInd, copy);
                            break;

                        case ListButtonResult.Remove:
                            M8.ArrayUtil.RemoveAt(ref data, actInd);
                            break;

                        case ListButtonResult.Up:
                            if(actInd > 0) {
                                string t = data[actInd - 1];
                                data[actInd - 1] = data[actInd];
                                data[actInd] = t;
                            }
                            break;

                        case ListButtonResult.Down:
                            if(actInd < data.Length - 1) {
                                string t = data[actInd + 1];
                                data[actInd + 1] = data[actInd];
                                data[actInd] = t;
                            }
                            break;
                    }
                }

                EditorGUIUtility.LookLikeControls();
            }

            GUILayout.EndVertical();
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

        public static string GetProjectName() {
            string[] pathSplit = Application.dataPath.Split('/');
            return pathSplit[pathSplit.Length - 2];
        }

        public static string PreferenceKey(string klass, string field) {
            return string.Format("m8.{0}.{1}.{2}", GetProjectName(), klass, field);
        }
    }
}

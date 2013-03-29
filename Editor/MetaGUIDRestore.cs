using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

namespace M8.Editor {
    public class MetaGUIDRestore : EditorWindow {
        public const string textPathPrefs = "m8.renegadeware.MetaGUIDRestore.textPath";

        public class Item {
            public string path;
            public string guid;
        }

        private TextAsset mTextFile;
        private string mTextName = "";
        private string mTextFilePath = "";

        private string[] mMetaFilePaths = null;

        private Vector2 mMetaFilesScroll;

        [MenuItem("M8/Meta GUID Restore")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(MetaGUIDRestore));
        }

        void OnSelectionChange() {
            if(mTextFile != null && Selection.objects.Length > 0) {
                HashSet<string> filteredPaths = new HashSet<string>();

                foreach(Object obj in Selection.objects) {
                    string objMetaPath = AssetDatabase.GetTextMetaDataPathFromAssetPath(AssetDatabase.GetAssetPath(obj));

                    if(!string.IsNullOrEmpty(objMetaPath))
                        filteredPaths.Add(objMetaPath);

                    if(Utility.IsDirectory(obj)) {
                        string[] paths = Utility.GetPaths(obj, "*.meta", true);
                        foreach(string path in paths) {
                            filteredPaths.Add(path);
                        }
                    }
                }

                mMetaFilePaths = new string[filteredPaths.Count];
                filteredPaths.CopyTo(mMetaFilePaths);
            }
            else {
                mMetaFilePaths = null;
            }

            Repaint();
        }

        void OnEnable() {
            string path = EditorPrefs.GetString(textPathPrefs, "");
            if(!string.IsNullOrEmpty(path)) {
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
                if(obj != null)
                    mTextFile = (TextAsset)obj;
            }
        }

        void OnDisable() {
            if(mTextFile != null)
                EditorPrefs.SetString(textPathPrefs, AssetDatabase.GetAssetPath(mTextFile));
        }

        void OnGUI() {
            EditorGUIUtility.LookLikeControls(80.0f);

            GUILayout.Space(6f);
            GUILayout.BeginHorizontal();

            bool doCreate = false;

            if(mTextFile == null) {
                GUI.backgroundColor = Color.green;
                doCreate = GUILayout.Button("Create", GUILayout.Width(76f));

                GUI.backgroundColor = Color.white;
                mTextName = GUILayout.TextField(mTextName);
            }

            GUILayout.EndHorizontal();
                        
            if(mTextFile != null) {
                mTextName = mTextFile.name;
                mTextFilePath = AssetDatabase.GetAssetPath(mTextFile);
            }
            else if(!string.IsNullOrEmpty(mTextName)) {
                mTextFilePath = Utility.GetSelectionFolder() + mTextName + ".txt";
            }

            if(doCreate && !string.IsNullOrEmpty(mTextName)) {
                File.WriteAllText(mTextFilePath, "");

                AssetDatabase.Refresh();

                mTextFile = (TextAsset)AssetDatabase.LoadAssetAtPath(mTextFilePath, typeof(TextAsset));
            }

            GUILayout.BeginHorizontal();

            GUILayout.Label("Select: ");

            mTextFile = (TextAsset)EditorGUILayout.ObjectField(mTextFile, typeof(TextAsset), false);

            GUILayout.EndHorizontal();

            if(!string.IsNullOrEmpty(mTextFilePath))
                GUILayout.Label("Path: " + mTextFilePath);
            else {
                GUILayout.Label("Path: <none>" + mTextFilePath);
            }

            Utility.DrawSeparator();

            if(mTextFile != null) {
                GUILayout.BeginHorizontal();
                
                if(GUILayout.Button("Generate", GUILayout.Width(76f)) && mMetaFilePaths != null) {
                    //
                    StringBuilder sb = new StringBuilder();
                    
                    for(int i = 0; i < mMetaFilePaths.Length; i++) {
                        string path = mMetaFilePaths[i];

                        string s = Utility.GetTextContent(path);
                        string[] lines = s.Split('\n');

                        foreach(string line in lines) {
                            if(line.Contains("guid")) {
                                int sInd = line.IndexOf(' ');
                                if(sInd != -1 && sInd+1 < line.Length) {
                                    sb.Append(path+'\n');

                                    if(i == mMetaFilePaths.Length - 1)
                                        sb.Append(line.Substring(sInd + 1));
                                    else
                                        sb.Append(line.Substring(sInd + 1) + '\n');
                                }
                                else {
                                    Debug.LogWarning("Unable to parse guid line for: ["+path+"] line: "+line);
                                }
                                break;
                            }
                        }
                    }

                    using(StreamWriter output = new StreamWriter(mTextFilePath)) {
                        output.Write(sb.ToString());
                    }

                    AssetDatabase.Refresh();
                }

                if(GUILayout.Button("Restore", GUILayout.Width(76f))) {
                    string[] lines = mTextFile.text.Split('\n');

                    for(int lInd = 0; lInd < lines.Length; lInd += 2) {
                        string filePath = lines[lInd];
                        string guid = lines[lInd + 1];

                        string metaText = Utility.GetTextContent(filePath);

                        if(metaText == null) {
                            //try to find it
                            int startInd = filePath.LastIndexOf('/');
                            if(startInd != -1)
                                filePath = filePath.Substring(startInd);

                            string otherPath = Utility.FindPath(filePath, "Assets", true);
                            if(otherPath == null) {
                                Debug.LogWarning("Unable to find meta file: " + filePath);
                                continue;
                            }
                            else {
                                metaText = Utility.GetTextContent(otherPath);
                            }
                        }

                        string[] metaTextLines = metaText.Split('\n');

                        StringBuilder sb = new StringBuilder();

                        //put lines to string buffer
                        //look for the line with a guid and replace it
                        for(int i = 0; i < metaTextLines.Length; i++) {
                            if(metaTextLines[i].Contains("guid")) {
                                metaTextLines[i] = "guid: " + guid;
                            }

                            sb.Append(metaTextLines[i]+'\n');
                        }

                        using(StreamWriter output = new StreamWriter(filePath)) {
                            output.Write(sb.ToString());
                        }
                    }

                    AssetDatabase.Refresh();
                }

                GUILayout.EndHorizontal();
            }
            else {
                GUILayout.Label("Create or select an existing text file first.");
            }

            if(mMetaFilePaths != null) {
                GUILayout.Label("Meta files to generate from:");

                mMetaFilesScroll = GUILayout.BeginScrollView(mMetaFilesScroll);

                foreach(string path in mMetaFilePaths) {
                    GUILayout.Label(path);
                }

                GUILayout.EndScrollView();
            }
            else {
                GUILayout.Label("Select file(s) and folder(s) to gather guids from meta files.");
            }
        }
    }
}
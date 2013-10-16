using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace M8.Editor {
    public class InputMapper : EditorWindow {
        public const string PrefKlass = "InputMapper";
        public const string PrefFile = "File";

        private static List<string> mActions = null;
        
        private static string[] mActionEditNames;
        private static int[] mActionEditVals;

        private TextAsset mTextFile;
        private string mTextName = "";
        private string mTextFilePath = "";

        private Vector2 mActionListScroll;

        private uint mUnknownCount = 0;

        public static List<string> actions {
            get {
                if(mActions == null) {
                    //try to load it
                    string path = EditorPrefs.GetString(Utility.PreferenceKey(PrefKlass, PrefFile), "");
                    if(!string.IsNullOrEmpty(path)) {
                        Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
                        if(obj != null) {
                            mActions = GetInputActions((TextAsset)obj);
                            RefreshInputActionEdits();
                        }
                    }
                }

                return mActions;
            }
        }

        public static int GUISelectInputAction(string label, int selectedValue) {
            List<string> acts = actions;

            if(acts != null) {
                return EditorGUILayout.IntPopup(label, selectedValue, mActionEditNames, mActionEditVals);
            }
            else {
                GUILayout.BeginHorizontal();

                GUILayout.Label(label);
                
                //let user know they need to configure input actions
                if(GUILayout.Button("[Edit Input Actions]", GUILayout.Width(200))) {
                    EditorWindow.GetWindow(typeof(InputMapper));
                }

                GUILayout.EndHorizontal();
            }

            return selectedValue;
        }

        public static List<string> GetInputActions(TextAsset textFile) {
            List<string> ret = new List<string>();

            string[] lines = textFile.text.Split('\n');

            for(int i = 2; i < lines.Length-2; i++) {
                string[] words = lines[i].Split(' ', '\t', ';', '=');

                bool publicFound = false;
                bool constFound = false;
                bool declFound = false;

                string name = null;

                foreach(string word in words) {
                    if(word.Length > 0) {
                        if(publicFound && constFound && declFound) {
                            name = word;
                            break;
                        }
                        else {
                            if(word == "public")
                                publicFound = true;
                            else if(word == "const")
                                constFound = true;
                            else if(word == "int")
                                declFound = true;
                        }
                    }
                }

                if(!string.IsNullOrEmpty(name)) {
                    ret.Add(name);
                }
            }

            return ret;
        }

        private static void RefreshInputActionEdits() {
            if(mActions != null) {
                mActionEditNames = new string[mActions.Count + 1];
                mActionEditVals = new int[mActions.Count + 1];

                mActionEditNames[0] = "Invalid";
                mActionEditVals[0] = InputManager.ActionInvalid;

                for(int i = 0; i < mActions.Count; i++) {
                    mActionEditNames[i + 1] = mActions[i];
                    mActionEditVals[i + 1] = i;
                }
            }
        }

        [MenuItem("M8/Input Mapper")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(InputMapper));
        }

        void OnSelectionChange() {
            Repaint();
        }

        void OnEnable() {
            string path = EditorPrefs.GetString(Utility.PreferenceKey(PrefKlass, PrefFile), "");
            if(!string.IsNullOrEmpty(path)) {
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
                if(obj != null)
                    mTextFile = (TextAsset)obj;
            }
        }

        void OnDisable() {
            if(mTextFile != null)
                EditorPrefs.SetString(Utility.PreferenceKey(PrefKlass, PrefFile), AssetDatabase.GetAssetPath(mTextFile));
        }

        void OnGUI() {
            TextAsset prevTextFile = mTextFile;

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
                mTextFilePath = Utility.GetSelectionFolder() + mTextName + ".cs";
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
                if(prevTextFile != mTextFile || mActions == null) {

                    mActions = GetInputActions(mTextFile);
                    RefreshInputActionEdits();
                }

                //list actions
                mActionListScroll = GUILayout.BeginScrollView(mActionListScroll);//, GUILayout.MinHeight(100));

                int removeInd = -1;

                Regex r = new Regex("^[a-zA-Z0-9]*$");

                for(int i = 0; i < mActions.Count; i++) {
                    GUILayout.BeginHorizontal();

                    GUILayout.Label(i.ToString(), GUILayout.MaxWidth(20));

                    string text = GUILayout.TextField(mActions[i], 255);

                    if(text.Length > 0 && (r.IsMatch(text) && !char.IsDigit(text[0])))
                        mActions[i] = text;

                    if(GUILayout.Button("DEL", GUILayout.MaxWidth(40))) {
                        removeInd = i;
                    }

                    GUILayout.EndHorizontal();
                }

                if(removeInd != -1)
                    mActions.RemoveAt(removeInd);

                if(GUILayout.Button("Add")) {
                    mActions.Add("Unknown" + (mUnknownCount++));
                }

                GUILayout.EndScrollView();

                Utility.DrawSeparator();

                if(GUILayout.Button("Save")) {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("//AUTOGENERATED - DO NOT TOUCH!");
                    sb.AppendLine("public struct InputAction {");

                    for(int i = 0; i < mActions.Count; i++) {
                        sb.AppendFormat("    public const int {0} = {1};", mActions[i], i);
                        sb.AppendLine();
                    }

                    sb.AppendFormat("    public const int _count = {0};", mActions.Count);
                    sb.AppendLine();

                    sb.AppendLine("}");

                    using(StreamWriter output = new StreamWriter(mTextFilePath)) {
                        output.Write(sb.ToString());
                    }

                    AssetDatabase.Refresh();

                    RefreshInputActionEdits();
                }
            }
        }
    }
}
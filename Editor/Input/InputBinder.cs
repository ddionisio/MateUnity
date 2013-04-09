using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text;

namespace M8.Editor {
    public class InputBinder : EditorWindow {
        public const string PrefKlass = "InputBinder";
        public const string PrefText = "Text";

        private TextAsset mTextFile;
        private string mTextName = "";
        private string mTextFilePath = "";

        private enum InputType {
            Unity,
            KeyCode,
            InputMap
        }

        private struct BindData {
            public InputManager.Bind bind;
            public InputType[] keyTypes;
            public bool foldOut;

            public void DeleteKey(int ind) {
                bind.keys.RemoveAt(ind);
                keyTypes = M8.ArrayUtil.RemoveAt(keyTypes, ind);
            }

            public void ResizeKeys(int newLen) {
                if(keyTypes == null) {
                    if(bind.keys == null) {
                        bind.keys = new List<InputManager.Key>();
                        keyTypes = new InputType[0];
                    }
                    else if(bind.keys.Count == 0) {
                        keyTypes = new InputType[0];
                    }
                    else {
                        RefreshKeyTypes();
                    }
                }

                int prevLen = keyTypes.Length;

                if(prevLen != newLen) {
                    System.Array.Resize(ref keyTypes, newLen);

                    if(prevLen > keyTypes.Length) {
                        bind.keys.RemoveRange(newLen - 1, bind.keys.Count - newLen);
                    }
                    else {
                        for(int i = prevLen; i < keyTypes.Length; i++) {
                            keyTypes[i] = InputType.Unity;
                            bind.keys.Add(new InputManager.Key());
                        }
                    }
                }
            }

            public void RefreshKeyType(int ind) {
                if(bind.keys[ind].map != InputKeyMap.None)
                    keyTypes[ind] = InputType.InputMap;
                else if(bind.keys[ind].code != KeyCode.None)
                    keyTypes[ind] = InputType.KeyCode;
                else
                    keyTypes[ind] = InputType.Unity;
            }

            public void RefreshKeyTypes() {
                if(bind.keys != null && bind.keys.Count > 0) {
                    if(keyTypes == null || keyTypes.Length != bind.keys.Count) {
                        keyTypes = new InputType[bind.keys.Count];
                    }

                    for(int i = 0; i < keyTypes.Length; i++)
                        RefreshKeyType(i);
                }
            }

            //call before saving, basically clears out specific key binds based on key type
            public void ApplyKeyTypes() {
                if(keyTypes != null && bind.keys != null) {
                    for(int i = 0; i < keyTypes.Length; i++) {
                        switch(keyTypes[i]) {
                            case InputType.Unity:
                                bind.keys[i].code = KeyCode.None;
                                bind.keys[i].map = InputKeyMap.None;
                                break;

                            case InputType.KeyCode:
                                bind.keys[i].input = "";
                                bind.keys[i].map = InputKeyMap.None;
                                break;

                            case InputType.InputMap:
                                bind.keys[i].input = "";
                                bind.keys[i].code = KeyCode.None;
                                break;
                        }
                    }
                }
            }
        }

        private BindData[] mBinds;

        private Vector2 mBindsScroll;

        [MenuItem("M8/Input Binder")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(InputBinder));
        }

        void OnSelectionChange() {
            Repaint();
        }

        void OnEnable() {
            string path = EditorPrefs.GetString(Utility.PreferenceKey(PrefKlass, PrefText), "");
            if(!string.IsNullOrEmpty(path)) {
                Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset));
                if(obj != null)
                    mTextFile = (TextAsset)obj;
            }
        }

        void OnDisable() {
            if(mTextFile != null)
                EditorPrefs.SetString(Utility.PreferenceKey(PrefKlass, PrefText), AssetDatabase.GetAssetPath(mTextFile));
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

            if(GUILayout.Button("Edit Input Actions")) {
                EditorWindow.GetWindow(typeof(InputMapper));
            }

            Utility.DrawSeparator();

            bool refreshBinds = mTextFile != prevTextFile;

            if(mTextFile != null) {
                List<string> actions = InputMapper.actions;

                //initialize bind data
                if(mBinds == null || mBinds.Length != actions.Count) {
                    if(mBinds == null) {
                        mBinds = new BindData[actions.Count];
                    }
                    else {
                        System.Array.Resize<BindData>(ref mBinds, actions.Count);
                    }

                    refreshBinds = true;
                }

                //load from file
                if(refreshBinds && mTextFile.text.Length > 0) {
                    //load data
                    fastJSON.JSON.Instance.Parameters.UseExtensions = false;
                    List<InputManager.Bind> loadBinds = fastJSON.JSON.Instance.ToObject<List<InputManager.Bind>>(mTextFile.text);
                    foreach(InputManager.Bind bind in loadBinds) {
                        if(bind.action < mBinds.Length) {
                            mBinds[bind.action].bind = bind;
                            mBinds[bind.action].RefreshKeyTypes();
                        }
                    }
                }

                //display
                mBindsScroll = GUILayout.BeginScrollView(mBindsScroll);

                for(int i = 0; i < mBinds.Length; i++) {
                    if(mBinds[i].bind == null) {
                        mBinds[i].bind = new InputManager.Bind();
                    }

                    mBinds[i].bind.action = i;

                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(actions[i]);

                    GUILayout.FlexibleSpace();

                    mBinds[i].bind.control = (InputManager.Control)EditorGUILayout.EnumPopup(mBinds[i].bind.control);
                                        
                    GUILayout.EndHorizontal();

                    if(mBinds[i].bind.control == InputManager.Control.Axis) {
                        mBinds[i].bind.deadZone = EditorGUILayout.FloatField("Deadzone", mBinds[i].bind.deadZone);
                    }

                    int keyCount = mBinds[i].keyTypes != null ? mBinds[i].keyTypes.Length : 0;

                    mBinds[i].foldOut = EditorGUILayout.Foldout(mBinds[i].foldOut, string.Format("Binds [{0}]", keyCount));

                    if(mBinds[i].foldOut) {
                        int delKey = -1;

                        for(int key = 0; key < keyCount; key++) {
                            GUILayout.BeginVertical(GUI.skin.box);

                            GUILayout.BeginHorizontal();
                            mBinds[i].bind.keys[key].player = EditorGUILayout.IntField("Player", mBinds[i].bind.keys[key].player, GUILayout.MaxWidth(200));
                            
                            GUILayout.FlexibleSpace();

                            if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                                delKey = key;

                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();
                                                        
                            //key bind
                            mBinds[i].keyTypes[key] = (InputType)EditorGUILayout.EnumPopup(mBinds[i].keyTypes[key]);
                            
                            switch(mBinds[i].keyTypes[key]) {
                                case InputType.Unity:
                                    mBinds[i].bind.keys[key].input = EditorGUILayout.TextField(mBinds[i].bind.keys[key].input, GUILayout.MinWidth(250));
                                    break;

                                case InputType.KeyCode:
                                    mBinds[i].bind.keys[key].code = (KeyCode)EditorGUILayout.EnumPopup(mBinds[i].bind.keys[key].code);
                                    break;

                                case InputType.InputMap:
                                    mBinds[i].bind.keys[key].map = (InputKeyMap)EditorGUILayout.EnumPopup(mBinds[i].bind.keys[key].map);
                                    break;
                            }

                            GUILayout.EndHorizontal();

                            //other configs
                            if(mBinds[i].keyTypes[key] != InputType.Unity)
                                mBinds[i].bind.keys[key].axis = (InputManager.ButtonAxis)EditorGUILayout.EnumPopup("Axis", mBinds[i].bind.keys[key].axis, GUILayout.MaxWidth(200));

                            mBinds[i].bind.keys[key].index = EditorGUILayout.IntField("Index", mBinds[i].bind.keys[key].index, GUILayout.MaxWidth(200));

                            GUILayout.EndVertical();
                        }

                        if(delKey != -1) {
                            mBinds[i].DeleteKey(delKey);
                        }

                        if(GUILayout.Button("Add")) {
                            mBinds[i].ResizeKeys(keyCount + 1);
                        }
                    }

                    GUILayout.EndVertical();
                }

                GUILayout.EndScrollView();

                if(GUILayout.Button("Save")) {
                    fastJSON.JSON.Instance.Parameters.UseExtensions = false;
                    
                    List<InputManager.Bind> saveBinds = new List<InputManager.Bind>(mBinds.Length);

                    for(int i = 0; i < mBinds.Length; i++) {
                        mBinds[i].ApplyKeyTypes();

                        saveBinds.Add(mBinds[i].bind);
                    }

                    string output = fastJSON.JSON.Instance.ToJSON(saveBinds);

                    File.WriteAllText(mTextFilePath, output);

                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
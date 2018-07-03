using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomEditor(typeof(SceneState))]
    public class SceneStateInspector : Editor {

        private bool initFoldout = true;
        private bool initGlobalFoldout = true;
        private bool runtimeFoldout = true;
        private bool runtimeGlobalFoldout = true;

        private string[] mMasks;

        private string mApplyName = "";
        private SceneState.Type mApplyType;
        private int mApplyValue = 0;
        private float mApplyFValue = 0.0f;
        private string mApplySValue = "";
        private bool mApplyToGlobal = false;
        private bool mApplyPersistent = false;

        public override void OnInspectorGUI() {
            if(mMasks == null) {
                mMasks = M8.EditorExt.Utility.GenerateGenericMaskString();
            }

            SceneState data = target as SceneState;

            if(!Application.isPlaying) {
                var newUserData = EditorGUILayout.ObjectField("User Data", data.userData, typeof(UserData), false) as UserData;
                if(data.userData != newUserData) {
                    Undo.RecordObject(data, "Change User Data");
                    data.userData = newUserData;
                }

                var newUserDataAutoSave = EditorGUILayout.Toggle("Auto Save", data.autoSave);
                if(data.autoSave != newUserDataAutoSave) {
                    Undo.RecordObject(data, "Change Auto Save");
                    data.autoSave = newUserDataAutoSave;
                }

                //Globals 
                initGlobalFoldout = EditorGUILayout.Foldout(initGlobalFoldout, "Scene Global Data");

                if(initGlobalFoldout) {
                    GUILayout.BeginVertical(GUI.skin.box);

                    int delSubKey = -1;
                    for(int j = 0; j < data.globalStartData.Length; j++) {
                        SceneState.InitData initDat = data.globalStartData[j];

                        EditorGUI.BeginChangeCheck();

                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.BeginHorizontal();
                                                
                        initDat.name = GUILayout.TextField(initDat.name, GUILayout.MinWidth(200));

                        GUILayout.Space(32);

                        if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                            delSubKey = j;

                        GUILayout.EndHorizontal();

                        initDat.type = (SceneState.Type)EditorGUILayout.EnumPopup("Type", initDat.type);
                        switch(initDat.type) {
                            case SceneState.Type.Integer:
                                initDat.ival = EditorGUILayout.IntField("Value", initDat.ival);
                                initDat.ival = EditorGUILayout.MaskField("Flags", initDat.ival, mMasks);
                                break;
                            case SceneState.Type.Float:
                                initDat.fval = EditorGUILayout.FloatField("Float", initDat.fval);
                                break;
                            case SceneState.Type.String:
                                initDat.sval = EditorGUILayout.TextField("String", initDat.sval);
                                break;
                        }

                        GUILayout.EndVertical();

                        if(delSubKey == -1 && EditorGUI.EndChangeCheck()) {
                            Undo.RecordObject(target, "SceneState - Edit ["+data.globalStartData[j].name+"]");

                            data.globalStartData[j] = initDat;
                        }
                    }

                    if(delSubKey != -1) {
                        Undo.RecordObject(target, "SceneState - Removed ["+data.globalStartData[delSubKey].name+"]");

                        M8.ArrayUtil.RemoveAt(ref data.globalStartData, delSubKey);
                    }

                    if(GUILayout.Button("New Global Value")) {
                        Undo.RecordObject(target, "SceneState - New Global Value");

                        System.Array.Resize(ref data.globalStartData, data.globalStartData.Length + 1);

                        data.globalStartData[data.globalStartData.Length - 1] = new SceneState.InitData(SceneState.Type.Integer);
                    }

                    GUILayout.EndVertical();
                }

                M8.EditorExt.Utility.DrawSeparator();

                //Scene Specifics
                initFoldout = EditorGUILayout.Foldout(initFoldout, "Scene Data");

                if(initFoldout) {
                    //Cache count

                    EditorGUI.BeginChangeCheck();

                    int cacheCount = EditorGUILayout.IntField("Cache Count", data.localStateCache);

                    if(EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(target, "SceneState - Cache Count Change");

                        data.localStateCache = cacheCount;
                    }

                    //Scenes

                    int delKey = -1;

                    for(int i = 0; i < data.startData.Length; i++) {
                        SceneState.InitSceneData sceneDat = data.startData[i];
                                                
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.BeginHorizontal();

                        sceneDat.editFoldout = EditorGUILayout.Foldout(sceneDat.editFoldout, "Scene:");

                        GUILayout.Space(4);

                        //Scene name
                        EditorGUI.BeginChangeCheck();

                        string newScene = GUILayout.TextField(sceneDat.scene, GUILayout.MinWidth(200));

                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RecordObject(target, "SceneState - Change Scene Name from "+sceneDat.scene+" to "+newScene);

                            sceneDat.scene = newScene;
                        }

                        GUILayout.Space(32);

                        if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                            delKey = i;

                        GUILayout.EndHorizontal();

                        if(sceneDat.editFoldout) {
                            GUILayout.Label("Values:");

                            if(sceneDat.data != null) {
                                int delSubKey = -1;
                                for(int j = 0; j < sceneDat.data.Length; j++) {
                                    EditorGUI.BeginChangeCheck();

                                    SceneState.InitData initDat = sceneDat.data[j];

                                    GUILayout.BeginVertical(GUI.skin.box);

                                    GUILayout.BeginHorizontal();

                                    initDat.name = GUILayout.TextField(initDat.name, GUILayout.MinWidth(200));

                                    GUILayout.Space(32);

                                    if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                                        delSubKey = j;

                                    GUILayout.EndHorizontal();

                                    initDat.type = (SceneState.Type)EditorGUILayout.EnumPopup("Type", initDat.type);
                                    switch(initDat.type) {
                                        case SceneState.Type.Integer:
                                            initDat.ival = EditorGUILayout.IntField("Value", initDat.ival);
                                            initDat.ival = EditorGUILayout.MaskField("Flags", initDat.ival, mMasks);
                                            break;
                                        case SceneState.Type.Float:
                                            initDat.fval = EditorGUILayout.FloatField("Float", initDat.fval);
                                            break;
                                        case SceneState.Type.String:
                                            initDat.sval = EditorGUILayout.TextField("String", initDat.sval);
                                            break;
                                    }

                                    GUILayout.EndVertical();

                                    if(delKey != -1 && delSubKey != -1 && EditorGUI.EndChangeCheck()) {
                                        Undo.RecordObject(target, "SceneState ("+sceneDat.scene+") - Edit ["+sceneDat.data[j].name+"]");

                                        sceneDat.data[j] = initDat;
                                    }
                                }

                                if(delSubKey != -1) {
                                    Undo.RecordObject(target, "SceneState ("+sceneDat.scene+") - Removed ["+sceneDat.data[delSubKey].name+"]");

                                    M8.ArrayUtil.RemoveAt(ref sceneDat.data, delSubKey);
                                }
                            }

                            if(GUILayout.Button("New Value")) {
                                Undo.RecordObject(target, "SceneState ("+sceneDat.scene+") - New Value");

                                if(sceneDat.data == null)
                                    sceneDat.data = new SceneState.InitData[1];
                                else
                                    System.Array.Resize(ref sceneDat.data, sceneDat.data.Length + 1);

                                sceneDat.data[sceneDat.data.Length - 1] = new SceneState.InitData(SceneState.Type.Integer);
                            }
                        }

                        GUILayout.EndVertical();
                    }

                    if(delKey != -1) {
                        Undo.RecordObject(target, "SceneState - Removed ["+data.startData[delKey].scene+"]");

                        M8.ArrayUtil.RemoveAt(ref data.startData, delKey);
                    }

                    if(GUILayout.Button("New Scene Data")) {
                        Undo.RecordObject(target, "SceneState - New Scene Set Added");

                        System.Array.Resize(ref data.startData, data.startData.Length + 1);
                        data.startData[data.startData.Length - 1] = new SceneState.InitSceneData();
                    }
                }
            }
            else {
                M8.EditorExt.Utility.DrawSeparator();

                //global scene data
                runtimeGlobalFoldout = EditorGUILayout.Foldout(runtimeGlobalFoldout, "Global Scene Data");

                if(runtimeGlobalFoldout && data.global != null) {
                    foreach(var pair in data.global) {
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.Label(pair.Key);

                        switch(pair.Value.type) {
                            case SceneState.Type.Integer:
                                EditorGUILayout.LabelField("Value", pair.Value.ival.ToString());
                                EditorGUILayout.MaskField("Flags", pair.Value.ival, mMasks);
                                break;
                            case SceneState.Type.Float:
                                EditorGUILayout.LabelField("Float", pair.Value.fval.ToString());
                                break;
                            case SceneState.Type.String:
                                EditorGUILayout.LabelField("String", pair.Value.sval);
                                break;
                            case SceneState.Type.Invalid:
                                EditorGUILayout.LabelField("Invalid!");
                                break;
                        }

                        GUILayout.EndVertical();
                    }
                }

                M8.EditorExt.Utility.DrawSeparator();

                //Scene data
                runtimeFoldout = EditorGUILayout.Foldout(runtimeFoldout, "Scene Data");

                if(runtimeFoldout && data.local != null) {
                    foreach(var pair in data.local) {
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.Label(pair.Key);

                        switch(pair.Value.type) {
                            case SceneState.Type.Integer:
                                EditorGUILayout.LabelField("Value", pair.Value.ival.ToString());
                                EditorGUILayout.MaskField("Flags", pair.Value.ival, mMasks);
                                break;
                            case SceneState.Type.Float:
                                EditorGUILayout.LabelField("Float", pair.Value.fval.ToString());
                                break;
                            case SceneState.Type.String:
                                EditorGUILayout.LabelField("String", pair.Value.sval);
                                break;
                            case SceneState.Type.Invalid:
                                EditorGUILayout.LabelField("Invalid!");
                                break;
                        }

                        GUILayout.EndVertical();
                    }
                }

                M8.EditorExt.Utility.DrawSeparator();

                //value change
                GUILayout.BeginVertical(GUI.skin.box);

                GUILayout.Label("Override");

                mApplyName = GUILayout.TextField(mApplyName);
                mApplyType = (SceneState.Type)EditorGUILayout.EnumPopup("Type", mApplyType);
                switch(mApplyType) {
                    case SceneState.Type.Integer:
                        mApplyValue = EditorGUILayout.IntField("Value", mApplyValue);
                        mApplyValue = EditorGUILayout.MaskField("Flags", mApplyValue, mMasks);
                        break;
                    case SceneState.Type.Float:
                        mApplyFValue = EditorGUILayout.FloatField("Float", mApplyFValue);
                        break;
                    case SceneState.Type.String:
                        mApplySValue = EditorGUILayout.TextField("String", mApplySValue);
                        break;
                }


                mApplyToGlobal = GUILayout.Toggle(mApplyToGlobal, "Global");
                mApplyPersistent = GUILayout.Toggle(mApplyPersistent, "Persistent");

                var table = mApplyToGlobal ? data.global : data.local;

                if(GUILayout.Button("Apply") && !string.IsNullOrEmpty(mApplyName)) {
                    switch(mApplyType) {
                        case SceneState.Type.Integer:
                            table.SetValue(mApplyName, mApplyValue, mApplyPersistent);
                            break;
                        case SceneState.Type.Float:
                            table.SetValueFloat(mApplyName, mApplyFValue, mApplyPersistent);
                            break;
                        case SceneState.Type.String:
                            table.SetValueString(mApplyName, mApplySValue, mApplyPersistent);
                            break;
                    }

                    Repaint();
                }

                GUILayout.EndVertical();

                M8.EditorExt.Utility.DrawSeparator();

                //refresh
                if(GUILayout.Button("Refresh")) {
                    if(!string.IsNullOrEmpty(mApplyName)) {
                        var val = table.GetValueRaw(mApplyName);
                        mApplyValue = val.ival;
                        mApplyFValue = val.fval;
                        mApplySValue = val.sval;
                    }
                    Repaint();
                }
            }
        }
    }
}
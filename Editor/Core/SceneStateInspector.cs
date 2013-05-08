using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SceneState))]
public class SceneStateInspector : Editor {

    private bool initFoldout = true;
    private bool initGlobalFoldout = true;
    private bool runtimeFoldout = true;
    private bool runtimeGlobalFoldout = true;

    private string[] mMasks;

    private string mApplyName = "";
    private int mApplyValue = 0;
    private float mApplyFValue = 0.0f;
    private bool mApplyToGlobal = false;
    private bool mApplyPersistent = false;

    public override void OnInspectorGUI() {
        if(mMasks == null) {
            mMasks = M8.Editor.Utility.GenerateGenericMaskString();
        }

        SceneState data = target as SceneState;

        if(!Application.isPlaying) {
            GUI.changed = false;

            //Globals 
            initGlobalFoldout = EditorGUILayout.Foldout(initGlobalFoldout, "Scene Global Data");

            if(initGlobalFoldout) {
                if(data.globalStartData == null)
                    data.globalStartData = new SceneState.InitData[0];

                GUILayout.BeginVertical(GUI.skin.box);

                int delSubKey = -1;
                for(int j = 0; j < data.globalStartData.Length; j++) {
                    SceneState.InitData initDat = data.globalStartData[j];

                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();

                    initDat.name = GUILayout.TextField(initDat.name, GUILayout.MinWidth(200));

                    GUILayout.Space(32);

                    if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                        delSubKey = j;

                    GUILayout.EndHorizontal();

                    initDat.ival = EditorGUILayout.IntField("Value", initDat.ival);
                    initDat.ival = EditorGUILayout.MaskField("Flags", initDat.ival, mMasks);
                    initDat.fval = EditorGUILayout.FloatField("Float", initDat.fval);
                    initDat.persistent = GUILayout.Toggle(initDat.persistent, "Persistent");

                    GUILayout.EndVertical();
                }

                if(delSubKey != -1)
                    M8.ArrayUtil.RemoveAt(ref data.globalStartData, delSubKey);

                if(GUILayout.Button("New Global Value")) {
                    System.Array.Resize(ref data.globalStartData, data.globalStartData.Length + 1);

                    data.globalStartData[data.globalStartData.Length - 1] = new SceneState.InitData();
                }

                GUILayout.EndVertical();
            }

            M8.Editor.Utility.DrawSeparator();

            //Scene Specifics
            initFoldout = EditorGUILayout.Foldout(initFoldout, "Scene Data");

            if(initFoldout) {
                if(data.startData == null)
                    data.startData = new SceneState.InitSceneData[0];

                int delKey = -1;

                for(int i = 0; i < data.startData.Length; i++) {
                    SceneState.InitSceneData sceneDat = data.startData[i];

                    GUILayout.BeginVertical(GUI.skin.box);

                    GUILayout.BeginHorizontal();

                    sceneDat.editFoldout = EditorGUILayout.Foldout(sceneDat.editFoldout, "Scene:");

                    GUILayout.Space(4);
                                        
                    sceneDat.scene = GUILayout.TextField(sceneDat.scene, GUILayout.MinWidth(200));

                    GUILayout.Space(32);

                    if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                        delKey = i;

                    GUILayout.EndHorizontal();

                    if(sceneDat.editFoldout) {
                        GUILayout.Label("Values:");

                        if(sceneDat.data != null) {
                            int delSubKey = -1;
                            for(int j = 0; j < sceneDat.data.Length; j++) {
                                SceneState.InitData initDat = sceneDat.data[j];

                                GUILayout.BeginVertical(GUI.skin.box);

                                GUILayout.BeginHorizontal();

                                initDat.name = GUILayout.TextField(initDat.name, GUILayout.MinWidth(200));

                                GUILayout.Space(32);

                                if(GUILayout.Button("DEL", GUILayout.MaxWidth(40)))
                                    delSubKey = j;

                                GUILayout.EndHorizontal();

                                initDat.ival = EditorGUILayout.IntField("Value", initDat.ival);
                                initDat.ival = EditorGUILayout.MaskField("Flags", initDat.ival, mMasks);
                                initDat.fval = EditorGUILayout.FloatField("Float", initDat.fval);
                                initDat.persistent = GUILayout.Toggle(initDat.persistent, "Persistent");

                                GUILayout.EndVertical();
                            }

                            if(delSubKey != -1)
                                M8.ArrayUtil.RemoveAt(ref sceneDat.data, delSubKey);
                        }

                        if(GUILayout.Button("New Value: "+sceneDat.scene)) {
                            if(sceneDat.data == null)
                                sceneDat.data = new SceneState.InitData[1];
                            else
                                System.Array.Resize(ref sceneDat.data, sceneDat.data.Length + 1);

                            sceneDat.data[sceneDat.data.Length - 1] = new SceneState.InitData();
                        }
                    }
                                        
                    GUILayout.EndVertical();
                }

                if(delKey != -1)
                    M8.ArrayUtil.RemoveAt(ref data.startData, delKey);

                if(GUILayout.Button("New Scene Data")) {
                    System.Array.Resize(ref data.startData, data.startData.Length + 1);
                    data.startData[data.startData.Length - 1] = new SceneState.InitSceneData();
                }
            }

            if(GUI.changed)
                EditorUtility.SetDirty(target);
        }
        else {
            M8.Editor.Utility.DrawSeparator();

            //global scene data
            runtimeGlobalFoldout = EditorGUILayout.Foldout(runtimeGlobalFoldout, "Global Scene Data");

            if(runtimeGlobalFoldout) {
                if(data.globalStates != null) {
                    foreach(KeyValuePair<string, SceneState.StateValue> dat in data.globalStates) {
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.Label(dat.Key);

                        EditorGUILayout.LabelField("Value", dat.Value.ival.ToString());
                        EditorGUILayout.MaskField("Flags", dat.Value.ival, mMasks);
                        EditorGUILayout.LabelField("Float", dat.Value.fval.ToString());

                        GUILayout.EndVertical();
                    }
                }
            }

            M8.Editor.Utility.DrawSeparator();

            //Scene data
            runtimeFoldout = EditorGUILayout.Foldout(runtimeFoldout, "Scene Data");

            if(runtimeFoldout) {
                if(data.states != null) {
                    foreach(KeyValuePair<string, SceneState.StateValue> dat in data.states) {
                        GUILayout.BeginVertical(GUI.skin.box);

                        GUILayout.Label(dat.Key);

                        EditorGUILayout.LabelField("Value", dat.Value.ival.ToString());
                        EditorGUILayout.MaskField("Flags", dat.Value.ival, mMasks);
                        EditorGUILayout.LabelField("Float", dat.Value.fval.ToString());

                        GUILayout.EndVertical();
                    }
                }
            }

            M8.Editor.Utility.DrawSeparator();

            //value change
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("Override");

            mApplyName = GUILayout.TextField(mApplyName);

            mApplyValue = EditorGUILayout.IntField("Value", mApplyValue);
            mApplyValue = EditorGUILayout.MaskField("Flags", mApplyValue, mMasks);
            mApplyFValue = EditorGUILayout.FloatField("Float", mApplyFValue);
            mApplyToGlobal = GUILayout.Toggle(mApplyToGlobal, "Global");
            mApplyPersistent = GUILayout.Toggle(mApplyPersistent, "Persistent");

            if(GUILayout.Button("Apply") && !string.IsNullOrEmpty(mApplyName)) {
                if(mApplyToGlobal) {
                    data.SetGlobalValue(mApplyName, mApplyValue, mApplyPersistent);
                    data.SetGlobalValueFloat(mApplyName, mApplyFValue, mApplyPersistent);
                }
                else {
                    data.SetValue(mApplyName, mApplyValue, mApplyPersistent);
                    data.SetValueFloat(mApplyName, mApplyFValue, mApplyPersistent);
                }

                Repaint();
            }

            GUILayout.EndVertical();

            M8.Editor.Utility.DrawSeparator();

            //refresh
            if(GUILayout.Button("Refresh")) {
                if(!string.IsNullOrEmpty(mApplyName)) {
                    mApplyValue = data.GetValue(mApplyName);
                    mApplyFValue = data.GetValueFloat(mApplyName);
                }
                Repaint();
            }
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomEditor(typeof(StatsController))]
    public class StatsControllerInspector : Editor {
        public struct State {
            public bool isExpanded;
        }

        private bool mIsEditMode;
        private List<int> mCheckedIds;
        private State[] mEditStates;

        private Vector2 mScroll;

        private GUIStyle mFoldoutStyle;

        public override void OnInspectorGUI() {
            var statsCtrl = (StatsController)target;

            var stats = serializedObject.FindProperty("_stats");

            var statsItems = stats.FindPropertyRelative("_statItems");

            //Items
            GUILayout.BeginVertical();

            if(Application.isPlaying) {
                mIsEditMode = false;
                                
                foreach(var stat in statsCtrl) {
                    //grab name
                    string name;

                    int statDataIndex = StatsTemplateConfig.GetStatsIndex(stat.id);
                    if(statDataIndex != -1)
                        name = StatsTemplateConfig.stats[statDataIndex].name;
                    else
                        name = "<Unknown> ID: "+stat.id;

                    string valStr = string.Format("{0}/{1}", stat.currentValue, stat.valueMax);

                    GUILayout.BeginHorizontal(GUI.skin.box);

                    EditorGUILayout.LabelField(name, valStr);

                    GUILayout.EndHorizontal();
                }
            }
            else {
                if(mIsEditMode) {                    
                    foreach(var statData in StatsTemplateConfig.stats) {
                        int idIndex = mCheckedIds.IndexOf(statData.id);

                        GUILayout.BeginHorizontal(GUI.skin.box);

                        bool check = GUILayout.Toggle(idIndex != -1, statData.name);
                        if(check) {
                            if(idIndex == -1)
                                mCheckedIds.Add(statData.id);
                        }
                        else {
                            if(idIndex != -1)
                                mCheckedIds.RemoveAt(idIndex);
                        }

                        GUILayout.Space(10f);

                        GUILayout.Label("ID: "+statData.id.ToString(), GUILayout.Width(40f));

                        GUILayout.EndHorizontal();
                    }
                }
                else {
                    if(mEditStates == null || mEditStates.Length != statsItems.arraySize)
                        mEditStates = new State[statsItems.arraySize];

                    for(int i = 0; i < statsItems.arraySize; i++) {
                        var editState = mEditStates[i];

                        var idValue = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_id").intValue;

                        var valMin = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_valueMin");
                        var valMax = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_valueMax");
                        var valDefault = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_valueDefault");
                        
                        //grab name
                        string name;

                        int statDataIndex = StatsTemplateConfig.GetStatsIndex(idValue);
                        if(statDataIndex != -1)
                            name = StatsTemplateConfig.stats[statDataIndex].name;
                        else
                            name = "<Unknown> ID: "+idValue;

                        if(mFoldoutStyle == null) {
                            mFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                            mFoldoutStyle.fontStyle = FontStyle.Bold;
                        }
                        
                        if(editState.isExpanded) {
                            editState.isExpanded = EditorGUILayout.Foldout(editState.isExpanded, new GUIContent(name), true, mFoldoutStyle);

                            GUILayout.BeginVertical(GUI.skin.box);

                            EditorGUILayout.PropertyField(valMin, new GUIContent("Min"));

                            EditorGUILayout.PropertyField(valMax, new GUIContent("Max"));

                            if(valMin.floatValue > valMax.floatValue) {
                                valDefault.floatValue = valMax.floatValue = valMin.floatValue;
                            }
                            else if(valMax.floatValue < valMin.floatValue) {
                                valDefault.floatValue = valMin.floatValue = valMax.floatValue;
                            }
                            else if(valMin.floatValue < valMax.floatValue) {
                                EditorExt.Utility.PropertyFieldFloatSliderLayout(valDefault, valMin.floatValue, valMax.floatValue, new GUIContent("Default"));
                            }

                            GUILayout.EndVertical();
                        }
                        else {
                            GUILayout.BeginHorizontal(GUI.skin.box);

                            string str;

                            if(valMin.floatValue == valMax.floatValue)
                                str = string.Format("{0} [{1}]", name, valMax.floatValue);
                            else
                                str = string.Format("{0} [{1}, {2}] Default: {3}", name, valMin.floatValue, valMax.floatValue, valDefault.floatValue);

                            editState.isExpanded = EditorGUILayout.Foldout(editState.isExpanded, new GUIContent(str), true, mFoldoutStyle);

                            GUILayout.EndHorizontal();
                        }

                        mEditStates[i] = editState;
                    }

                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.EndVertical();

            var lastEnabled = GUI.enabled;
            var lastBkgrndClr = GUI.backgroundColor;

            GUILayout.Space(8f);
             
            //Settings
            if(mIsEditMode) {
                GUILayout.BeginHorizontal();

                //Save
                GUI.backgroundColor = Color.green;
                if(GUILayout.Button("Save")) {
                    mIsEditMode = false;

                    //go through and restructure stat items
                    var newStats = new List<StatItem>();

                    foreach(var statData in StatsTemplateConfig.stats) {
                        if(mCheckedIds.Contains(statData.id)) {
                            //grab existing stat item
                            var statItem = statsCtrl[statData.id];
                            if(statItem == null)
                                statItem = new StatItem(statData.id);

                            newStats.Add(statItem);
                        }
                    }

                    Undo.RecordObject(target, "Save Stat Settings For " + target.name);

                    statsCtrl.Override(newStats.ToArray());

                    mEditStates = new State[newStats.Count];
                }

                GUILayout.Space(16f);

                //Cancel
                GUI.backgroundColor = Color.red;
                if(GUILayout.Button("Cancel")) {
                    mIsEditMode = false;
                }

                GUILayout.EndHorizontal();
            }
            else {
                GUI.enabled = lastEnabled && !Application.isPlaying && StatsTemplateConfig.stats != null;
                
                if(GUILayout.Button("Settings")) {                    
                    mIsEditMode = true;

                    //setup checked list
                    mCheckedIds = new List<int>();

                    foreach(var stat in statsCtrl)
                        mCheckedIds.Add(stat.id);
                }
            }


            GUI.enabled = lastEnabled;
            GUI.backgroundColor = lastBkgrndClr;
            
            if(GUILayout.Button("Edit Template"))
                StatsTemplateConfig.Open();
        }
    }
}
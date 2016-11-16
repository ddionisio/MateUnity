using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomEditor(typeof(StatsController))]
    public class StatsControllerInspector : Editor {

        private bool mIsEditMode;
        private List<int> mCheckedIds;

        private Vector2 mScroll;

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

                    string valStr = string.Format("{0}/{1}", stat.currentValue, stat.value);

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
                    for(int i = 0; i < statsItems.arraySize; i++) {
                        var idValue = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_id").intValue;

                        var val = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_value");
                        var clamp = statsItems.GetArrayElementAtIndex(i).FindPropertyRelative("_clamp");
                        
                        //grab name
                        string name;

                        int statDataIndex = StatsTemplateConfig.GetStatsIndex(idValue);
                        if(statDataIndex != -1)
                            name = StatsTemplateConfig.stats[statDataIndex].name;
                        else
                            name = "<Unknown> ID: "+idValue;

                        GUILayout.BeginHorizontal(GUI.skin.box);
                        
                        EditorGUILayout.PropertyField(val, new GUIContent(name));

                        GUILayout.FlexibleSpace();
                        
                        EditorGUILayout.PropertyField(clamp, new GUIContent("Clamped"));

                        GUILayout.EndHorizontal();
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
                GUI.enabled = lastEnabled && !Application.isPlaying;
                
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
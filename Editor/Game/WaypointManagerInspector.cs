using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(WaypointManager))]
public class WaypointManagerInspector : Editor {
    public enum AddOption {
        Begin,
        InsertAfter,
        End
    }

    public class Data {
        public bool foldout = false;
        public int selInd = 0;
    }

    private Dictionary<Transform, Data> mData;
    private string mNewName;

    void OnEnable() {
        WaypointManager input = target as WaypointManager;
        int numChild = input.transform.GetChildCount();
        mData = new Dictionary<Transform, Data>(numChild);
        foreach(Transform t in input.transform) {
            mData.Add(t, new Data());
        }
    }

    void OnDisable() {
        WaypointManager input = target as WaypointManager;
        input.__inspectorSelected = null;
        mData = null;
    }

    public override void OnInspectorGUI() {
        GUI.changed = false;

        base.OnInspectorGUI();

        M8.Editor.Utility.DrawSeparator();

        WaypointManager input = target as WaypointManager;

        foreach(Transform t in input.transform) {
            Data dat = mData[t];
                        
            GUILayout.BeginVertical(GUI.skin.box);

            bool foldout = EditorGUILayout.Foldout(dat.foldout, t.name);
            if(foldout != dat.foldout) {
                dat.foldout = foldout;
                input.__inspectorSelected = foldout ? t : null;
            }

            if(dat.foldout) {
                GUILayout.BeginHorizontal();
                t.name = EditorGUILayout.TextField("New Name", t.name);

                if(GUILayout.Button("SEL", GUILayout.MaxWidth(50.0f))) {
                    Selection.activeGameObject = t.gameObject;
                }

                if(GUILayout.Button("DEL", GUILayout.MaxWidth(50.0f))) {
                    DestroyImmediate(t.gameObject);
                    Selection.activeGameObject = null;
                }
                GUILayout.EndHorizontal();

                int numChild = t.GetChildCount();

                if(numChild == 0) {
                    if(GUILayout.Button("Add New Point")) {
                        //we need to make two, the first will basically be the first point as the parent
                        Vector3 pt = t.position;
                        t.position = Vector3.zero;

                        GameObject newPt1 = new GameObject("0");
                        newPt1.transform.parent = t;
                        newPt1.transform.position = pt;

                        GameObject newPt2 = new GameObject("1");
                        newPt2.transform.parent = t;
                        newPt2.transform.position = pt;

                        input.__inspectorSelected = newPt2.transform;

                        Selection.activeGameObject = newPt2;
                    }
                }
                else {
                    GUILayout.BeginVertical(GUI.skin.box);

                    int newInd = EditorGUILayout.IntSlider("index", dat.selInd, 0, numChild - 1);
                    if(newInd != dat.selInd) {
                        dat.selInd = newInd;
                        input.__inspectorSelected = t.GetChild(dat.selInd);
                    }

                    GUILayout.BeginHorizontal();
                    if(GUILayout.Button("Select")) {
                        Selection.activeGameObject = t.GetChild(dat.selInd).gameObject;
                    }

                    if(GUILayout.Button("Insert After")) {
                        //add after
                        GameObject newPt1 = new GameObject(dat.selInd.ToString());
                        newPt1.transform.parent = t;

                        if(dat.selInd + 1 < numChild) {
                            t.GetChild(dat.selInd + 1).position = input.__inspectorSelected.position;
                            newPt1 = t.GetChild(dat.selInd + 1).gameObject;
                        }

                        //shift others
                        for(int i = numChild; i > dat.selInd; i--) {
                            t.GetChild(i).name = i.ToString();
                        }

                        Selection.activeGameObject = newPt1.gameObject;
                    }

                    if(GUILayout.Button("Delete")) {
                        if(numChild == 2) {
                            if(dat.selInd == 0)
                                t.position = t.GetChild(1).position;
                            else
                                t.position = t.GetChild(0).position;

                            //delete both
                            GameObject go1 = t.GetChild(0).gameObject, go2 = t.GetChild(1).gameObject;
                            DestroyImmediate(go1);
                            DestroyImmediate(go2);
                            input.__inspectorSelected = null;
                            Repaint();

                        }
                        else {
                            DestroyImmediate(t.GetChild(dat.selInd).gameObject);
                            numChild--;

                            //shift others
                            for(int i = dat.selInd; i < numChild; i++) {
                                t.GetChild(i).name = i.ToString();
                            }
                        }

                        Repaint();
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.EndVertical();
                }
            }

            GUILayout.EndVertical();
        }

        M8.Editor.Utility.DrawSeparator();

        GUILayout.BeginHorizontal();
        mNewName = EditorGUILayout.TextField("Create New", mNewName);

        if(!string.IsNullOrEmpty(mNewName) && GUILayout.Button("ADD")) {
            GameObject newGO = new GameObject(mNewName);
            newGO.transform.parent = input.transform;
            newGO.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = newGO;
        }
        GUILayout.EndHorizontal();

        /*if(Application.isPlaying) {
            

            EditorGUILayout.LabelField("Current Index", input.curInd.ToString());
        }*/

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

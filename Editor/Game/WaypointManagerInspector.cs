using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
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

        private static string mLastWaypoint = null;
        private static int mLastWaypointInd = -1;

        void OnEnable() {
            WaypointManager input = target as WaypointManager;

            int numChild = input.transform.childCount;
            mData = new Dictionary<Transform, Data>(numChild);

            foreach(Transform t in input.transform) {
                Data dat = new Data();

                if(t.name == mLastWaypoint) {
                    dat.foldout = true;

                    if(mLastWaypointInd != -1) {
                        dat.selInd = Mathf.Clamp(mLastWaypointInd, 0, t.childCount);
                        input.__inspectorSelected = t.GetChild(dat.selInd);
                    }
                    else {
                        input.__inspectorSelected = t;
                    }

                    mLastWaypoint = null;
                    mLastWaypointInd = -1;
                }

                mData.Add(t, dat);
            }
        }

        void OnDisable() {
            WaypointManager input = target as WaypointManager;
            input.__inspectorSelected = null;
            mData = null;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            M8.EditorExt.Utility.DrawSeparator();

            WaypointManager input = target as WaypointManager;

            foreach(Transform t in input.transform) {
                Data dat;// = mData[t];
                if(!mData.TryGetValue(t, out dat))
                    mData.Add(t, dat = new Data());

                GUILayout.BeginVertical(GUI.skin.box);

                bool foldout = EditorGUILayout.Foldout(dat.foldout, t.name);
                if(foldout != dat.foldout) {
                    dat.foldout = foldout;
                    input.__inspectorSelected = foldout ? t : null;
                }

                if(dat.foldout) {
                    GUILayout.BeginHorizontal();

                    EditorGUI.BeginChangeCheck();
                    var newName = EditorGUILayout.TextField("New Name", t.name);
                    if(EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(t, "Change Waypoint Name");
                        
                        mLastWaypoint = t.name = newName;
                    }

                    if(GUILayout.Button("SEL", GUILayout.MaxWidth(50.0f))) {
                        Selection.activeGameObject = t.gameObject;
                    }

                    if(GUILayout.Button("DEL", GUILayout.MaxWidth(50.0f))) {
                        Undo.DestroyObjectImmediate(t.gameObject);
                        Selection.activeGameObject = null;
                    }
                    GUILayout.EndHorizontal();

                    int numChild = t.childCount;

                    if(numChild == 0) {
                        if(GUILayout.Button("Add New Point")) {
                            //we need to make two, the first will basically be the first point as the parent
                            Vector3 pt = t.position;
                            t.position = Vector3.zero;

                            GameObject newPt1 = new GameObject("0");
                            Undo.RegisterCreatedObjectUndo(newPt1, "Add New Point");
                            Undo.SetTransformParent(newPt1.transform, t, "Add New Point");

                            Undo.RecordObject(newPt1.transform, "Add New Point");
                            newPt1.transform.position = pt;

                            GameObject newPt2 = new GameObject("1");
                            Undo.RegisterCreatedObjectUndo(newPt2, "Add New Point");
                            Undo.SetTransformParent(newPt2.transform, t, "Add New Point");

                            Undo.RecordObject(newPt2.transform, "Add New Point");
                            newPt2.transform.position = pt;

                            input.__inspectorSelected = newPt2.transform;

                            mLastWaypointInd = 1;
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
                            mLastWaypointInd = dat.selInd;
                            Selection.activeGameObject = t.GetChild(dat.selInd).gameObject;
                        }

                        if(GUILayout.Button("Insert After")) {
                            //add after
                            GameObject newPt1 = new GameObject(dat.selInd.ToString());
                            Undo.RegisterCreatedObjectUndo(newPt1, "Insert Waypoint After");

                            Undo.SetTransformParent(newPt1.transform, t, "Insert Waypoint After");

                            if(dat.selInd + 1 < numChild) {
                                var child = t.GetChild(dat.selInd + 1);

                                Undo.RecordObject(child, "Insert Waypoint After");
                                child.position = input.__inspectorSelected.position;

                                newPt1 = t.GetChild(dat.selInd + 1).gameObject;
                            }
                            else {
                                Undo.RecordObject(newPt1.transform, "Insert Waypoint After");
                                newPt1.transform.position = input.__inspectorSelected.position;
                            }

                            //shift others
                            for(int i = numChild; i > dat.selInd; i--) {
                                var child = t.GetChild(i);
                                Undo.RecordObject(child, "Insert Waypoint After");
                                child.name = i.ToString();
                            }

                            mLastWaypointInd = dat.selInd + 1;
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
                                Undo.DestroyObjectImmediate(go1);
                                Undo.DestroyObjectImmediate(go2);
                                input.__inspectorSelected = null;
                                Repaint();

                            }
                            else {
                                Undo.DestroyObjectImmediate(t.GetChild(dat.selInd).gameObject);
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

            M8.EditorExt.Utility.DrawSeparator();

            GUILayout.BeginHorizontal();
            mNewName = EditorGUILayout.TextField("Create New", mNewName);

            if(!string.IsNullOrEmpty(mNewName) && GUILayout.Button("ADD")) {
                GameObject newGO = new GameObject(mNewName);
                Undo.RegisterCreatedObjectUndo(newGO, "Add Waypoint");
                Undo.SetTransformParent(newGO.transform, input.transform, "Add Waypoint");

                Undo.RecordObject(newGO.transform, "Add Waypoint");
                newGO.transform.localPosition = Vector3.zero;

                mLastWaypoint = mNewName;
                mLastWaypointInd = -1;
                Selection.activeGameObject = newGO;
            }
            GUILayout.EndHorizontal();

            /*if(Application.isPlaying) {
            

                EditorGUILayout.LabelField("Current Index", input.curInd.ToString());
            }*/
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.EditorExt {
    public class Projectinator : EditorWindow {
        private int mAxisInd = 0;
        private string[] mAxisNames = { "X", "Y", "Z" };

        private bool mInv = false;

        private float mOfs = 0.01f;

        private float mDist = 10000.0f;

        private int mLayer = 0;

        [MenuItem("M8/Tools/Projectinator")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(Projectinator));
        }

        void OnEnable() {
            mAxisInd = EditorPrefs.GetInt(M8.EditorExt.Utility.PreferenceKey("projectinator", "axis"), mAxisInd);
            mInv = EditorPrefs.GetBool(M8.EditorExt.Utility.PreferenceKey("projectinator", "inv"), mInv);
            mOfs = EditorPrefs.GetFloat(M8.EditorExt.Utility.PreferenceKey("projectinator", "ofs"), mOfs);
            mDist = EditorPrefs.GetFloat(M8.EditorExt.Utility.PreferenceKey("projectinator", "dist"), mDist);
            mLayer = EditorPrefs.GetInt(M8.EditorExt.Utility.PreferenceKey("projectinator", "layer"), mLayer);
        }

        void OnDisable() {
            EditorPrefs.SetInt(M8.EditorExt.Utility.PreferenceKey("projectinator", "axis"), mAxisInd);
            EditorPrefs.SetBool(M8.EditorExt.Utility.PreferenceKey("projectinator", "inv"), mInv);
            EditorPrefs.SetFloat(M8.EditorExt.Utility.PreferenceKey("projectinator", "ofs"), mOfs);
            EditorPrefs.SetFloat(M8.EditorExt.Utility.PreferenceKey("projectinator", "dist"), mDist);
            EditorPrefs.SetInt(M8.EditorExt.Utility.PreferenceKey("projectinator", "layer"), mLayer);
        }

        void OnGUI() {
            GUILayout.BeginVertical();

			EditorGUIUtility.labelWidth = 20.0f; EditorGUIUtility.fieldWidth = 50.0f;

            GUILayout.BeginHorizontal();

            //axis select
            GUILayout.BeginHorizontal(GUI.skin.box);
            mAxisInd = GUILayout.SelectionGrid(mAxisInd, mAxisNames, mAxisNames.Length, GUI.skin.toggle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUI.skin.box);
            mInv = GUILayout.Toggle(mInv, "Inv", GUILayout.MaxWidth(35));
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            //offset
            GUILayout.BeginHorizontal(GUI.skin.box);
            mOfs = EditorGUILayout.FloatField("ofs", mOfs);
            GUILayout.EndHorizontal();

            //distance
            GUILayout.BeginHorizontal(GUI.skin.box);
            mDist = EditorGUILayout.FloatField("dist", mDist);
            GUILayout.EndHorizontal();

            //layer
            mLayer = EditorGUILayout.LayerField(mLayer);

            //do it
            if(GUILayout.Button("Do It!")) {
                foreach(GameObject go in Selection.gameObjects) {
                    Transform t = go.transform;

					Undo.RecordObject(t, "Projectinate " + go.name);

                    Vector3 dir = Vector3.forward;

                    switch(mAxisInd) {
                        case 0:
                            dir = mInv ? -t.right : t.right;
                            break;
                        case 1:
                            dir = mInv ? -t.up : t.up;
                            break;
                        case 2:
                            dir = mInv ? -t.forward : t.forward;
                            break;
                    }

                    RaycastHit hit;
                    if(Physics.Raycast(t.position, dir, out hit, mDist, (1<<mLayer))) {
                        t.position = hit.point + hit.normal*mOfs;

                        switch(mAxisInd) {
                            case 0:
                                t.right = mInv ? hit.normal : -hit.normal;
                                break;
                            case 1:
                                t.up = mInv ? hit.normal : -hit.normal;
                                break;
                            case 2:
                                t.forward = mInv ? hit.normal : -hit.normal;
                                break;
                        }
                    }

					EditorUtility.SetDirty(t);
                }
            }

			EditorGUIUtility.labelWidth = 0.0f; EditorGUIUtility.fieldWidth = 0.0f;

            GUILayout.EndVertical();
        }
    }
}

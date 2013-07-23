using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.Editor {
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

        void OnGUI() {
            GUILayout.BeginVertical();

            EditorGUIUtility.LookLikeControls(20, 50);

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

                    Undo.RegisterUndo(t, "Projectinate " + go.name);

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
                }
            }

            EditorGUIUtility.LookLikeControls();

            GUILayout.EndVertical();
        }
    }
}

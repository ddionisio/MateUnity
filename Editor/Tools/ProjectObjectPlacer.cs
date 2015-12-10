using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.EditorExt {
    public class ProjectObjectPlacer : EditorWindow {
        const string prefKey = "projectobjplace";

        static int mDupCounter = 0;

        private int mAxisInd = 0;
        private string[] mAxisNames = { "X", "Y", "Z" };
        private string[] mLayerMasks;

        private bool mInv = false;

        private float mOfs = 0.01f;

        private int mLayerMask = 0;

        private GameObject mPrefab;

        private bool mIsPlacingObject = false;
        private Transform mPrefabPreviewer;

        private RaycastHit mHit;
        private bool mIsHit;

        private float mRotAxis;

        private bool mPlaceUnderSelection;
        private bool mUpdateOriginal;

        [MenuItem("M8/Tools/ProjectObjectPlacer")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(ProjectObjectPlacer));
        }

        void OnEnable() {
            mAxisInd = EditorPrefs.GetInt(M8.EditorExt.Utility.PreferenceKey(prefKey, "axis"), mAxisInd);
            mInv = EditorPrefs.GetBool(M8.EditorExt.Utility.PreferenceKey(prefKey, "inv"), mInv);
            mOfs = EditorPrefs.GetFloat(M8.EditorExt.Utility.PreferenceKey(prefKey, "ofs"), mOfs);
            mLayerMask = EditorPrefs.GetInt(M8.EditorExt.Utility.PreferenceKey(prefKey, "layer"), mLayerMask);

            if(SceneView.onSceneGUIDelegate != OnSceneGUI)
                SceneView.onSceneGUIDelegate += OnSceneGUI;

            mLayerMasks = M8.EditorExt.Utility.GenerateLayerMaskString();
        }

        void OnDisable() {
            EditorPrefs.SetInt(M8.EditorExt.Utility.PreferenceKey(prefKey, "axis"), mAxisInd);
            EditorPrefs.SetBool(M8.EditorExt.Utility.PreferenceKey(prefKey, "inv"), mInv);
            EditorPrefs.SetFloat(M8.EditorExt.Utility.PreferenceKey(prefKey, "ofs"), mOfs);
            EditorPrefs.SetInt(M8.EditorExt.Utility.PreferenceKey(prefKey, "layer"), mLayerMask);

            if(mPrefabPreviewer != null) {
                DestroyImmediate(mPrefabPreviewer.gameObject);
                mPrefabPreviewer = null;
            }

            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        void OnFocus() {
            mLayerMasks = M8.EditorExt.Utility.GenerateLayerMaskString();
        }

        void OnSelectionChange() {
            Repaint();
        }

        void OnGUI() {
            GUILayout.BeginVertical();
            
            EditorGUIUtility.labelWidth = 20f;
            EditorGUIUtility.fieldWidth = 50f;

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
            mRotAxis = EditorGUILayout.FloatField("rot", mRotAxis);
            mOfs = EditorGUILayout.FloatField("ofs", mOfs);
            GUILayout.EndHorizontal();
                        
            //layer
            mLayerMask = EditorGUILayout.MaskField(mLayerMask, mLayerMasks);

            //prefab
            EditorGUIUtility.labelWidth = 0f;
            EditorGUIUtility.fieldWidth = 0f;

            GameObject prevPrefab = mPrefab;
            mPrefab = EditorGUILayout.ObjectField("Object", mPrefab, typeof(GameObject), true) as GameObject;

            if(mPrefab != prevPrefab) {
                mIsPlacingObject = false;

                if(mPrefabPreviewer != null) {
                    DestroyImmediate(mPrefabPreviewer.gameObject);
                    mPrefabPreviewer = null;
                }
            }

            bool prevEnabled = GUI.enabled;

            //
            GUILayout.BeginVertical(GUI.skin.box);

            GUI.enabled = prevEnabled
                && mPrefab != null
                && Selection.activeGameObject != null
                && PrefabUtility.GetPrefabType(Selection.activeGameObject) != PrefabType.Prefab
                && PrefabUtility.GetPrefabType(Selection.activeGameObject) != PrefabType.ModelPrefab
                && (PrefabUtility.GetPrefabType(mPrefab) == PrefabType.Prefab
                    || PrefabUtility.GetPrefabType(mPrefab) == PrefabType.ModelPrefab);

            mPlaceUnderSelection = GUILayout.Toggle(mPlaceUnderSelection, "Place under selection.");
            
            GUI.enabled = prevEnabled
                && mPrefab != null
                && PrefabUtility.GetPrefabType(mPrefab) != PrefabType.Prefab
                && PrefabUtility.GetPrefabType(mPrefab) != PrefabType.ModelPrefab;

            mUpdateOriginal = GUILayout.Toggle(mUpdateOriginal, "Update object.");

            GUILayout.EndVertical();
                        
            //do it
            GUI.enabled = prevEnabled && mPrefab != null;

            if(GUILayout.Button(mIsPlacingObject ? "Stop Placing Object" : "Start Placing Object")) {
                if(mPrefabPreviewer != null) {
                    DestroyImmediate(mPrefabPreviewer.gameObject);
                    mPrefabPreviewer = null;
                }

                mIsPlacingObject = !mIsPlacingObject;
                mIsHit = false;

                if(mIsPlacingObject) {
                    //now placing objects
                    GameObject newGo = Instantiate(mPrefab) as GameObject;
                    newGo.hideFlags |= HideFlags.HideAndDontSave;

                    mPrefabPreviewer = newGo.transform;

                    M8.Util.SetPhysicsLayerRecursive(mPrefabPreviewer, LayerMask.NameToLayer("Ignore Raycast"));
                }
                else
                    HandleUtility.Repaint();
            }

            GUI.enabled = prevEnabled;

            EditorGUIUtility.labelWidth = 0f;
            EditorGUIUtility.fieldWidth = 0f;

            GUILayout.EndVertical();
        }

        void OnSceneGUI(SceneView sceneView) {
            if(mIsPlacingObject && mPrefabPreviewer != null) {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                switch(Event.current.type) {
                    case EventType.KeyDown:
                        if(Event.current.keyCode == KeyCode.Escape) {
                            mIsPlacingObject = false;
                            mIsHit = false;

                            if(mPrefabPreviewer != null) {
                                DestroyImmediate(mPrefabPreviewer.gameObject);
                                mPrefabPreviewer = null;
                            }

                            Repaint();
                        }
                        break;

                    case EventType.MouseDown:
                        if(Event.current.button == 0)
                            Stamp();
                        break;

                    case EventType.MouseMove:
                        if(Event.current.shift) {
                            mRotAxis += Event.current.delta.x;// / 10.0f;
                            mRotAxis %= 360.0f;

                            if(mIsHit)
                                ApplyRot();

                            Repaint();
                        }
                        else if(Event.current.control) {
                            mOfs += Event.current.delta.y * 0.01f;

                            if(mIsHit)
                                mPrefabPreviewer.position = mHit.point + mHit.normal * mOfs;

                            Repaint();
                        }
                        else {
                            UpdatePreview(sceneView.camera);
                        }
                        break;
                }

                if(Event.current.isMouse && Event.current.button == 0) {
                    Event.current.Use();
                }
            }
        }

        void Stamp() {
            if(mIsHit) {
                GameObject newGo;
                Transform newT;

                if(mUpdateOriginal
                    && PrefabUtility.GetPrefabType(mPrefab) != PrefabType.Prefab
                    && PrefabUtility.GetPrefabType(mPrefab) != PrefabType.ModelPrefab) {
                    newGo = mPrefab;
                    newT = newGo.transform;
                }
                else {
                    newGo = Instantiate(mPrefab) as GameObject;
                    newT = newGo.transform;

                    newGo.name = mPrefab.name + mDupCounter;

                    newT.parent = mPlaceUnderSelection 
                        && Selection.activeGameObject != null 
                        && PrefabUtility.GetPrefabType(Selection.activeGameObject) != PrefabType.Prefab
                        && PrefabUtility.GetPrefabType(Selection.activeGameObject) != PrefabType.ModelPrefab 
                            ? Selection.activeGameObject.transform : null;
                                        
                    mDupCounter++;
                }

                newT.position = mPrefabPreviewer.position;
                newT.rotation = mPrefabPreviewer.rotation;
            }
        }

        void UpdatePreview(Camera cam) {
            Vector2 mousePos = Event.current.mousePosition;
            mousePos.y = Screen.height - Event.current.mousePosition.y;
            Ray ray = cam.ScreenPointToRay(mousePos);

            mIsHit = Physics.Raycast(ray, out mHit, Mathf.Infinity, mLayerMask);

            if(mIsHit) {
                mPrefabPreviewer.position = mHit.point + mHit.normal * mOfs;

                ApplyRot();
            }
        }

        void ApplyRot() {
            switch(mAxisInd) {
                case 0:
                    mPrefabPreviewer.right = mInv ? mHit.normal : -mHit.normal;
                    break;
                case 1:
                    mPrefabPreviewer.up = mInv ? mHit.normal : -mHit.normal;
                    mPrefabPreviewer.rotation *= Quaternion.AngleAxis(mRotAxis, Vector3.up);
                    break;
                case 2:
                    mPrefabPreviewer.forward = mInv ? mHit.normal : -mHit.normal;
                    break;
            }
        }
    }
}
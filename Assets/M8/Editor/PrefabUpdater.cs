using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

namespace M8.Editor {
    public class PrefabUpdater : EditorWindow {

        [MenuItem("M8/Prefab Updater")]
        static void DoIt() {
            EditorWindow.GetWindow(typeof(PrefabUpdater));
        }

        private GameObject mMasterPrefab = null;

        void OnGUI() {
            EditorGUIUtility.LookLikeControls(80.0f);

            GUI.skin.label.wordWrap = true;

            GUILayout.Label("First select a prefab as the master. Then select folder(s) and other prefab(s) to update.");

            NGUIEditorTools.DrawSeparator();

            GUILayout.Label("Master Prefab:");

            Object[] objs = Selection.objects;

            if(mMasterPrefab != null) {
                GUILayout.Label(mMasterPrefab.name);
            }
            else {
                GUILayout.Label("<none>");
            }

            if(GUILayout.Button("Update")) {
                textList.Clear();

                if(mMasterPrefab != null) {
                    for(int i = 0; i < objs.Length; i++) {
                        if(IsPrefab(objs[i])) {
                            //it's a prefab
                            GameObject go = objs[i] as GameObject;

                            if(go.GetInstanceID() != mMasterPrefab.GetInstanceID()) {
                                RefreshPrefab(go, mMasterPrefab);
                            }
                        }
                        else {
                            //it might be a folder
                            string[] paths = GetPathsFromSelectedFolder(objs[i]);

                            ArrayList prefabs = new ArrayList(paths.Length);

                            foreach(string path in paths) {
                                GameObject go = AssetDatabase.LoadMainAssetAtPath(path) as GameObject;

                                if(go != null && IsPrefab(go)) {
                                    prefabs.Add(go);
                                }
                            }

                            RefreshPrefabs(prefabs, mMasterPrefab);
                        }
                    }
                }
                else {
                    textList.Add("No master prefab selected.");
                }
            }

            NGUIEditorTools.DrawSeparator();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach(string text in textList) {
                GUILayout.Label(text);
            }

            GUILayout.EndScrollView();
        }

        bool IsPrefab(Object obj) {
            return PrefabUtility.GetPrefabType(obj) == PrefabType.Prefab
                || PrefabUtility.GetPrefabType(obj) == PrefabType.ModelPrefab;
        }

        void OnSelectionChange() {
            if(Selection.objects.Length == 1) {
                if(IsPrefab(Selection.objects[0])) {
                    mMasterPrefab = Selection.objects[0] as GameObject;
                }
                else {
                    mMasterPrefab = null;
                }
            }
            else if(Selection.objects.Length == 0) {
                mMasterPrefab = null;
            }

            Repaint();
        }

        string[] GetPathsFromSelectedFolder(Object obj) {
            HashSet<string> filePaths = new HashSet<string>();

            string path = AssetDatabase.GetAssetPath(obj);

            FileAttributes attr = File.GetAttributes(path);
            if((attr & FileAttributes.Directory) == FileAttributes.Directory) {
                string[] paths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);

                foreach(string _path in paths) {
                    string thePath = _path.Replace('\\', '/');

                    //Object thing = AssetDatabase.LoadMainAssetAtPath(thePath);
                    filePaths.Add(thePath);
                }
            }

            string[] ret = new string[filePaths.Count];
            filePaths.CopyTo(ret);

            return ret;
        }

        void RefreshPrefabs(ArrayList prefabs, GameObject masterPrefab) {
            foreach(GameObject go in prefabs) {
                if(go.GetInstanceID() != masterPrefab.GetInstanceID())
                    RefreshPrefab(go, masterPrefab);
            }
        }

        void RefreshPrefab(GameObject prefab, GameObject masterPrefab) {
            //if the prefab is the same name as masterPrefab, then just instantiate masterPrefab
            //and replace the prefab with the instantiated object
            /*if(prefab.name == masterPrefab.name) {
                GameObject masterInstance = PrefabUtility.InstantiatePrefab(masterPrefab) as GameObject;
			
                GameObject newPrefab = PrefabUtility.ReplacePrefab(masterInstance, prefab);
                EditorUtility.SetDirty(newPrefab);
			
                textList.Add(newPrefab.name+" has been updated.");
			
                DestroyImmediate(masterInstance);
            }
            else*/
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                if(RefreshGameObjectWithPrefab(instance, masterPrefab)) {
                    PrefabUtility.ReplacePrefab(instance, prefab);
                    EditorUtility.SetDirty(prefab);

                    textList.Add(prefab.name + " has been updated.");
                }

                DestroyImmediate(instance);
            }
        }

        bool RefreshGameObjectWithPrefab(GameObject go, GameObject prefab) {
            ArrayList addList = new ArrayList(go.transform.childCount);
            ArrayList deleteList = new ArrayList(go.transform.childCount);

            bool hasSubChanges = false;

            foreach(Transform child in go.transform) {

                GameObject childGo = child.gameObject;

                if(childGo.name == prefab.name) {
                    PrefabCopy copy = new PrefabCopy();

                    copy.go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                    copy.pos = child.localPosition;
                    copy.rot = child.localRotation;
                    copy.scale = child.localScale;

                    addList.Add(copy);

                    deleteList.Add(childGo);
                }
                else {
                    if(RefreshGameObjectWithPrefab(childGo, prefab)) {
                        hasSubChanges = true;
                    }
                }
            }

            foreach(GameObject trash in deleteList) {
                DestroyImmediate(trash);
            }

            foreach(PrefabCopy copy in addList) {
                copy.go.transform.parent = go.transform;
                copy.go.transform.localPosition = copy.pos;
                copy.go.transform.localRotation = copy.rot;
                copy.go.transform.localScale = copy.scale;

                go.BroadcastMessage("OnPrefabUpdate", copy.go, SendMessageOptions.DontRequireReceiver);
            }

            //fail-safe for duplicates
            deleteList.Clear();

            bool foundAlready = false;
            foreach(Transform child in go.transform) {
                GameObject childGo = child.gameObject;

                if(child.name == prefab.name) {
                    if(foundAlready) {
                        deleteList.Add(childGo);
                    }
                    else {
                        foundAlready = true;
                    }
                }
            }

            foreach(GameObject trash in deleteList) {
                DestroyImmediate(trash);
            }
            //

            return addList.Count > 0 || hasSubChanges;
        }

        struct PrefabCopy {
            public GameObject go;
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scale;
        }

        private Vector2 scrollPos = Vector2.zero;

        private ArrayList textList = new ArrayList();
    }
}

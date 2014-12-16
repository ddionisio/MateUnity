using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8.EditorExt {
    public class HierarchyHelper {
        [MenuItem("M8/Selection/Move Up &UP")]
        public static void MoveUp() {
            SelectionMoveGameObjects(-1);
        }

        [MenuItem("M8/Selection/Move Down &DOWN")]
        public static void MoveDown() {
            SelectionMoveGameObjects(1);
        }

        [MenuItem("M8/Selection/Move To Top %&UP")]
        public static void MoveToTop() {
            Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.Editable);
            System.Array.Sort<Object>(objs, (obj1, obj2) => (obj1 as GameObject).transform.GetSiblingIndex() - (obj2 as GameObject).transform.GetSiblingIndex());
            for(int i = 0; i < objs.Length; i++) {
                GameObject go = objs[i] as GameObject;
                if(go)
                    go.transform.SetSiblingIndex(i);
            }
        }

        [MenuItem("M8/Selection/Move To Bottom %&DOWN")]
        public static void MoveToBottom() {
            int rootCount = Utility.sceneRootCount;
            Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.Editable);
            System.Array.Sort<Object>(objs, (obj1, obj2) => (obj1 as GameObject).transform.GetSiblingIndex() - (obj2 as GameObject).transform.GetSiblingIndex());
            for(int i = 0; i < objs.Length; i++) {
                GameObject go = objs[i] as GameObject;
                if(go) {
                    int c = go.transform.parent ? go.transform.parent.childCount : rootCount;
                    go.transform.SetSiblingIndex(c - 1);
                }
            }
        }

        [MenuItem("M8/Selection/Sort Children &S")]
        public static void Sort() {
            Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.Editable);
            foreach(Object obj in objs) {
                GameObject go = obj as GameObject;
                if(go) {
                    int childCount = go.transform.childCount;
                    if(childCount > 1) {
                        List<Transform> children = new List<Transform>(childCount);
                        for(int i = 0; i < childCount; i++)
                            children.Add(go.transform.GetChild(i));

                        children.Sort((t1, t2) => t1.name.CompareTo(t2.name));

                        for(int i = 0; i < childCount; i++)
                            children[i].SetSiblingIndex(i);
                    }
                }
            }
        }

        static void SelectionMoveGameObjects(int ofs) {
            Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel | SelectionMode.Editable);
            if(ofs < 0)
                System.Array.Sort<Object>(objs, (obj1, obj2) => (obj1 as GameObject).transform.GetSiblingIndex() - (obj2 as GameObject).transform.GetSiblingIndex());
            else
                System.Array.Sort<Object>(objs, (obj1, obj2) => (obj2 as GameObject).transform.GetSiblingIndex() - (obj1 as GameObject).transform.GetSiblingIndex());

            foreach(Object obj in objs) {
                GameObject go = obj as GameObject;
                if(go) {
                    int ind = go.transform.GetSiblingIndex() + ofs;
                    if(ind >= 0)
                        go.transform.SetSiblingIndex(ind);
                }
            }
        }
    }
}
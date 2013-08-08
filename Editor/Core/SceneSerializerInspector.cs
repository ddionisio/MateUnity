using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SceneSerializer))]
[CanEditMultipleObjects]
public class SerializedIDInspector : Editor {
    private static Dictionary<int, SceneSerializer> mRefs = null;

    public override void OnInspectorGUI() {
        //this shouldn't be in a prefab...
        
        if(targets != null && targets.Length > 1) {
            foreach(Object obj in targets) {
                PrefabType prefabType = PrefabUtility.GetPrefabType(obj);
                if(prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
                    continue;
                }

                CheckID(obj);
            }

            EditorGUILayout.LabelField("id", "--");
        }
        else {
            PrefabType prefabType = PrefabUtility.GetPrefabType(target);
            if(prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab) {
                EditorGUILayout.LabelField("id", "??");
                return;
            }

            CheckID(target);

            SceneSerializer data = target as SceneSerializer;

            EditorGUILayout.LabelField("id", data.id.ToString());
        }

        //sanity check
        if(GUILayout.Button("Verify Integrity")) {
            RegenerateIDs();
            Repaint();
        }
    }

    void CheckID(Object obj) {
        SceneSerializer data = obj as SceneSerializer;

        //check if we need to generate keys
        if(mRefs == null)
            RegenerateIDs();
        else if(data.id == SceneSerializer.invalidID || mRefs[data.id] != data) {
            int nid = 1;
            for(; ; nid++) {
                if(mRefs.ContainsKey(nid)) {
                    if(mRefs[nid] == null) { //was deleted for some reason
                        mRefs.Remove(nid);
                        break;
                    }
                }
                else
                    break;
            }

            data.__EditorSetID(nid);
            mRefs.Add(nid, data);
            EditorUtility.SetDirty(data);
        }
    }

    void RegenerateIDs() {
        if(mRefs == null)
            mRefs = new Dictionary<int, SceneSerializer>();

        //get all objects in scene with serializedID
        Object[] objs = FindObjectsOfType(typeof(SceneSerializer));

        int nid = 1;

        foreach(SceneSerializer sid in objs) {
            bool genNewKey = false;

            //generate a new id if it's not generated or conflicted for some reason
            if(sid.id != SceneSerializer.invalidID) {
                if(mRefs.ContainsKey(sid.id)) {
                    genNewKey = mRefs[sid.id] != sid;
                }
                else {
                    mRefs.Add(sid.id, sid);
                }
            }
            else {
                genNewKey = true;
            }

            if(genNewKey) {
                for(; mRefs.ContainsKey(nid); nid++);
                sid.__EditorSetID(nid);
                mRefs.Add(nid, sid);
                EditorUtility.SetDirty(sid);
            }
        }
    }
}

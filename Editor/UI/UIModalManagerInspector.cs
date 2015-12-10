using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using UIModalManager = M8.UIModal.Manager;
using UIController = M8.UIModal.Controller;

[CustomEditor(typeof(UIModalManager))]
public class UIModalManagerInspector : Editor {
    private UIController mNewUI = null;
    private TextEditor mTE = null;

    void OnEnable() {
        mTE = new TextEditor();
    }

    public override void OnInspectorGUI() {
        GUI.changed = false;

        UIModalManager input = target as UIModalManager;
        if(input.uis == null)
            input.uis = new UIModalManager.UIData[0];

        int delInd = -1;

        for(int i = 0; i < input.uis.Length; i++) {
            UIModalManager.UIData dat = input.uis[i];

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();
                        
            if(dat.e_ui != null) {
                GUILayout.Label(dat.e_ui.name);
            }
            else {
                GUILayout.Label("(Need target!)");
            }

            GUILayout.FlexibleSpace();

            if(dat.e_ui != null) {
                if(M8.EditorExt.Utility.DrawCopyButton("Click to copy name.")) {
                    mTE.text = dat.e_ui.name;
                    mTE.SelectAll();
                    mTE.Copy();
                }

                GUILayout.Space(4);
            }

            if(M8.EditorExt.Utility.DrawRemoveButton()) {
                delInd = i;
            }

            GUILayout.EndHorizontal();

            dat.e_ui = EditorGUILayout.ObjectField("target", dat.e_ui, typeof(UIController), true) as UIController;
            if(dat.e_ui) {
                dat.name = dat.e_ui.name;

                dat.isPrefab = PrefabUtility.GetPrefabType(dat.e_ui) == PrefabType.Prefab;
                if(dat.isPrefab) {
                    dat.instantiateTo = EditorGUILayout.ObjectField("instantiateTo", dat.instantiateTo, typeof(Transform), true) as Transform;
                }
            }

            bool openOnStart = dat.name == input.openOnStart;
            bool newOpenOnStart = EditorGUILayout.Toggle("Open On Start", openOnStart);
            if(openOnStart != newOpenOnStart) {
                input.openOnStart = newOpenOnStart ? dat.name : "";
                GUI.changed = true;
            }

            GUILayout.EndVertical();
        }

        if(delInd != -1) {
            M8.ArrayUtil.RemoveAt(ref input.uis, delInd);
            GUI.changed = true;
        }

        //add new
        GUILayout.BeginVertical(GUI.skin.box);

        mNewUI = EditorGUILayout.ObjectField(mNewUI, typeof(UIController), true) as UIController;

        bool lastEnabled = GUI.enabled;

        GUI.enabled = lastEnabled && mNewUI != null;
        if(GUILayout.Button("Add")) {
            System.Array.Resize(ref input.uis, input.uis.Length + 1);
            UIModalManager.UIData newDat = new UIModalManager.UIData();
            newDat.e_ui = mNewUI;
            input.uis[input.uis.Length - 1] = newDat;
            mNewUI = null;

            GUI.changed = true;
        }

        GUI.enabled = lastEnabled;

        GUILayout.EndVertical();

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

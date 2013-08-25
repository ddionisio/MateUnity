using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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

        int delInd = -1;

        for(int i = 0; i < input.uis.Length; i++) {
            UIModalManager.UIData dat = input.uis[i];

            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.BeginHorizontal();

            if(dat.ui != null) {
                GUILayout.Label(dat.ui.name);
            }
            else {
                GUILayout.Label("(Need target!)");
            }

            GUILayout.FlexibleSpace();

            if(dat.ui != null) {
                if(M8.Editor.Utility.DrawCopyButton()) {
                    mTE.content = new GUIContent(dat.ui.name);
                    mTE.SelectAll();
                    mTE.Copy();
                }

                GUILayout.Space(4);
            }

            if(M8.Editor.Utility.DrawRemoveButton()) {
                delInd = i;
            }

            GUILayout.EndHorizontal();

            dat.ui = EditorGUILayout.ObjectField("target", dat.ui, typeof(UIController), true) as UIController;
            dat.exclusive = EditorGUILayout.Toggle("exclusive", dat.exclusive);

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
            newDat.ui = mNewUI;
            newDat.exclusive = true;
            input.uis[input.uis.Length - 1] = newDat;
            mNewUI = null;

            GUI.changed = true;
        }

        GUI.enabled = lastEnabled;

        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUI.skin.box);

        input.openOnStart = EditorGUILayout.TextField("Start", input.openOnStart);

        GUILayout.EndVertical();

        if(GUI.changed)
            EditorUtility.SetDirty(target);
    }
}

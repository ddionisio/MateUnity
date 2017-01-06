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
        var input = target as UIModalManager;
        if(input.uis == null) {
            input.uis = new UIModalManager.UIData[0];
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }

        int delInd = -1;

        //Reset certain values if there are no uis
        if(input.uis.Length == 0) {
            if(!string.IsNullOrEmpty(input.openOnStart)) {
                input.openOnStart = "";
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }

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

            //Fields
            var uiCtrl = dat.e_ui;
            var _name = "";
            var _isPrefab = dat.isPrefab;
            var _instantiateTo = dat.instantiateTo;

            EditorGUI.BeginChangeCheck();

            uiCtrl = EditorGUILayout.ObjectField("target", uiCtrl, typeof(UIController), true) as UIController;
            if(uiCtrl) {
                _name = uiCtrl.name;

                _isPrefab = PrefabUtility.GetPrefabType(uiCtrl) == PrefabType.Prefab;
                if(_isPrefab) {
                    _instantiateTo = EditorGUILayout.ObjectField("instantiateTo", _instantiateTo, typeof(Transform), true) as Transform;                    
                }
            }

            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Change UI Modal Manager");

                dat.e_ui = uiCtrl;
                dat.name = _name;
                dat.isPrefab = _isPrefab;
                dat.instantiateTo = _instantiateTo;
            }

            //Toggle which ui to open on start
            bool openOnStart = dat.name == input.openOnStart;
            bool newOpenOnStart = EditorGUILayout.Toggle("Open On Start", openOnStart);
            if(openOnStart != newOpenOnStart) {
                Undo.RecordObject(target, "Change UI Modal Manager");
                input.openOnStart = newOpenOnStart ? dat.name : "";
            }

            GUILayout.EndVertical();
        }

        if(delInd != -1) {
            Undo.RecordObject(target, "UI Modal Manager Remove");
            M8.ArrayUtil.RemoveAt(ref input.uis, delInd);
        }

        //add new
        GUILayout.BeginVertical(GUI.skin.box);

        mNewUI = EditorGUILayout.ObjectField(mNewUI, typeof(UIController), true) as UIController;

        bool lastEnabled = GUI.enabled;

        GUI.enabled = lastEnabled && mNewUI != null;
        if(GUILayout.Button("Add")) {
            Undo.RecordObject(target, "UI Modal Manager Add");

            System.Array.Resize(ref input.uis, input.uis.Length + 1);
            UIModalManager.UIData newDat = new UIModalManager.UIData();
            newDat.e_ui = mNewUI;
            newDat.name = mNewUI.name;
            newDat.isPrefab = PrefabUtility.GetPrefabType(mNewUI) == PrefabType.Prefab;
            input.uis[input.uis.Length - 1] = newDat;
            mNewUI = null;
        }

        GUI.enabled = lastEnabled;
                
        GUILayout.EndVertical();

        var instantiateTo = (Transform)EditorGUILayout.ObjectField(new GUIContent("Instantiate To", "Default parent for instantiating prefab."), input.instantiateTo, typeof(Transform), true);
        if(input.instantiateTo != instantiateTo) {
            Undo.RecordObject(target, "Change UI Modal Manager");
            input.instantiateTo = instantiateTo;
        }
    }
}

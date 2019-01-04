using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(InputAction))]
    public class InputActionInspector : Editor {
        public enum KeyType {
            Input,
            KeyCode
        }

        private EditorExt.UnityInputManagerAxesDisplay mInputAxesDisplay;

        void OnEnable() {
            mInputAxesDisplay = new EditorExt.UnityInputManagerAxesDisplay();
            mInputAxesDisplay.Init();
        }

        public override void OnInspectorGUI() {
            var dat = (InputAction)this.target;

            bool isDirty = false;

            //Axis Settings
            GUILayout.BeginVertical();

            GUILayout.Label("Axis Settings");

            var newDeadZone = EditorGUILayout.FloatField("Deadzone", dat.deadZone);
            if(newDeadZone != dat.deadZone) {
                Undo.RecordObject(dat, "Input Action Change Deadzone");
                dat.deadZone = newDeadZone;
                isDirty = true;
            }

            var newForceRaw = EditorGUILayout.Toggle("Force Raw", dat.forceRaw);
            if(newForceRaw != dat.forceRaw) {
                Undo.RecordObject(dat, "Input Action Change Force Raw");
                dat.forceRaw = newForceRaw;
                isDirty = true;
            }

            GUILayout.EndVertical();
            //
            
            EditorExt.Utility.DrawSeparator();

            //Binds

            GUILayout.BeginVertical();

            GUILayout.Label("Binds");
                        
            int deleteIndex = -1, dupIndex = -1;

            for(int i = 0; i < dat.defaultBinds.Length; i++) {
                var bind = dat.defaultBinds[i];
                if(bind == null)
                    continue;

                GUILayout.BeginVertical(GUI.skin.box);

                //header
                GUILayout.BeginHorizontal();
                GUILayout.Label("Bind " + i);
                GUILayout.FlexibleSpace();
                if(EditorExt.Utility.DrawRemoveButton()) {
                    deleteIndex = i;
                }
                if(EditorExt.Utility.DrawSimpleButton("D", "Duplicate")) {
                    dupIndex = i;
                }
                GUILayout.EndHorizontal();
                //

                //settings
                EditorGUI.BeginChangeCheck();

                //code
                GUILayout.BeginHorizontal();

                KeyType curKeyType;
                if(!string.IsNullOrEmpty(bind.input))
                    curKeyType = KeyType.Input;
                else
                    curKeyType = KeyType.KeyCode;

                var newKeyType = (KeyType)EditorGUILayout.EnumPopup(curKeyType, GUILayout.Width(80f));
                if(curKeyType != newKeyType) {
                    //changed, reset based on type
                    curKeyType = newKeyType;
                    switch(curKeyType) {
                        case KeyType.Input:
                            bind.SetAsInput(mInputAxesDisplay.FirstAxisName());
                            break;
                        case KeyType.KeyCode:
                            bind.SetAsKey(0);
                            break;
                    }
                }

                switch(curKeyType) {
                    case KeyType.Input:
                        bind.input = mInputAxesDisplay.AxisNamePopupLayout(bind.input);
                        break;
                    case KeyType.KeyCode:
                        bind.code = (int)(KeyCode)EditorGUILayout.EnumPopup((KeyCode)bind.code);
                        break;
                }

                GUILayout.EndHorizontal();

                //axis settings
                GUILayout.Label("Axis Settings");

                bind.invert = EditorGUILayout.Toggle("Invert", bind.invert);

                if(curKeyType == KeyType.KeyCode)
                    bind.axis = (InputAction.ButtonAxis)EditorGUILayout.EnumPopup("Axis", bind.axis);

                if(EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(dat, "Input Action Change Bind "+i);
                    dat.defaultBinds[i] = bind;
                    dat.ResetBind(i);
                    isDirty = true;
                }

                GUILayout.EndVertical();
            }

            if(GUILayout.Button("Add New Bind")) {
                Undo.RecordObject(dat, "Input Action Add New Bind");
                System.Array.Resize(ref dat.defaultBinds, dat.defaultBinds.Length + 1);
                dat.ResetBinds();
                isDirty = true;
            }

            if(deleteIndex != -1) {
                Undo.RecordObject(dat, "Input Action Delete Bind");
                dat.defaultBinds = ArrayUtil.RemoveAt(dat.defaultBinds, deleteIndex);
                dat.ResetBinds();
                isDirty = true;
            }

            if(dupIndex != -1) {
                Undo.RecordObject(dat, "Input Action Duplicate Bind");
                var newBind = dat.defaultBinds[dupIndex];
                ArrayUtil.InsertAfter(ref dat.defaultBinds, dupIndex, newBind);
                dat.ResetBinds();
                isDirty = true;
            }

            if(isDirty)
                EditorUtility.SetDirty(dat);

            GUILayout.EndVertical();
        }
    }
}
 
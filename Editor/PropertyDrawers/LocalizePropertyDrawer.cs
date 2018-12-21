using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomPropertyDrawer(typeof(LocalizeAttribute))]
    public class LocalizePropertyDrawer : PropertyDrawer {

        private int[] mKeyInds;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            //make sure a localize asset exists
            if(!LocalizeEdit.isLocalizeFileExists) {
                GUI.Label(position, Localize.assetPath + " not found.");
            }
            else {
                var loc = Localize.instance;
                var locKeys = loc.GetKeysCustom("- None -");

                if(mKeyInds == null || mKeyInds.Length != locKeys.Length) {
                    mKeyInds = new int[locKeys.Length];
                    for(int i = 0; i < locKeys.Length; i++)
                        mKeyInds[i] = i;
                }

                const float editSize = 20f;
                const float editSpace = 4f;

                var popUpPos = new Rect(position.x, position.y, position.width - editSize - editSpace, position.height);
                var editPos = new Rect(position.x + position.width - editSize, position.y, editSize, position.height);

                var curStringVal = property.stringValue;

                int selectInd = 0;

                if(!string.IsNullOrEmpty(curStringVal)) {
                    for(int i = 0; i < locKeys.Length; i++) {
                        if(property.stringValue == locKeys[i]) {
                            selectInd = i;
                            break;
                        }
                    }
                }

                selectInd = EditorGUI.IntPopup(popUpPos, selectInd, locKeys, mKeyInds);

                if(GUI.Button(editPos, new GUIContent("E", "Configure localization."), EditorStyles.toolbarButton)) {
                    Selection.activeObject = loc;
                }

                property.stringValue = selectInd > 0 ? locKeys[selectInd] : "";
            }

            EditorGUI.EndProperty();
        }
    }
}
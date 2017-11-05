using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomPropertyDrawer(typeof(LocalizeAttribute))]
    public class LocalizePropertyDrawer : PropertyDrawer {
        private string[] mKeys;
        private int[] mKeyInds;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            bool showSelector = false;

            if(mKeys == null) {
                Localize loc = LocalizeSelector.localize;
                if(loc) {
                    var locKeys = loc.GetKeys();

                    List<string> keys = new List<string>(locKeys.Length + 1);
                    keys.Add("-None-");
                    keys.AddRange(locKeys);

                    mKeys = keys.ToArray();

                    mKeyInds = new int[mKeys.Length];
                    for(int i = 0; i < mKeyInds.Length; i++)
                        mKeyInds[i] = i;

                    showSelector = true;
                }
                else {
                    //let user select a localize object as reference
                    if(GUI.Button(position, "Select Localizer"))
                        LocalizeSelector.Open();
                }
            }
            else
                showSelector = true;

            if(showSelector) {
                string key = property.stringValue;

                int selectInd = 0;
                if(!string.IsNullOrEmpty(key)) {
                    for(int i = 1; i < mKeys.Length; i++) {
                        if(mKeys[i] == key) {
                            selectInd = i;
                            break;
                        }
                    }
                }

                const float editSize = 20f;
                const float editSpace = 4f;

                var popUpPos = new Rect(position.x, position.y, position.width - editSize - editSpace, position.height);
                var editPos = new Rect(position.x + position.width - editSize, position.y, editSize, position.height);
                
                selectInd = EditorGUI.IntPopup(popUpPos, selectInd, mKeys, mKeyInds);

                if(GUI.Button(editPos, new GUIContent("E", "Configure localization."), EditorStyles.toolbarButton)) {
                    LocalizeSelector.Open();
                }

                property.stringValue = selectInd > 0 ? mKeys[selectInd] : "";
            }

            EditorGUI.EndProperty();
        }
    }
}
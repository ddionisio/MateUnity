using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.UI {
    [CustomEditor(typeof(Texts.Localizer))]
    public class TextLocalizerInspector : Editor {
        public override void OnInspectorGUI() {
            var dat = target as Texts.Localizer;

            //selection
            string key = LocalizeConfig.DrawSelector(dat.key);
            if(dat.key != key) {
                Undo.RecordObject(target, "Set Text Localizer Key");
                dat.key = key;
            }

            EditorExt.Utility.DrawSeparator();

            //preview (for now, just base)
            if(GUILayout.Button("Preview")) {
                var textUI = dat.GetComponent<UnityEngine.UI.Text>();
                if(textUI) {
                    textUI.text = LocalizeConfig.GetBaseValue(dat.key);
                    EditorUtility.SetDirty(textUI);
                }
            }
        }
    }
}
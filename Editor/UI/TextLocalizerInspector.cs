using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.UI {
    [CustomEditor(typeof(Texts.Localizer))]
    public class TextLocalizerInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorExt.Utility.DrawSeparator();

            //preview (for now, just base)
            GUI.enabled = LocalizeEdit.isLocalizeFileExists;

            if(GUILayout.Button("Preview")) {
                var dat = target as Texts.Localizer;

                var textUI = dat.GetComponent<UnityEngine.UI.Text>();
                if(textUI) {
                    textUI.text = Localize.Get(dat.key);
                    EditorUtility.SetDirty(textUI);
                }
            }

            GUI.enabled = true;
        }
    }
}
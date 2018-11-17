using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.UI {
    [CustomEditor(typeof(Texts.LocalizerMultiFormat), true)]
    public class TextLocalizerMultiFormatInspector : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorExt.Utility.DrawSeparator();

            //preview (for now, just base)
            GUI.enabled = LocalizeEdit.isLocalizeFileExists;

            if(GUILayout.Button("Preview")) {
                var dat = target as Texts.LocalizerMultiFormat;

                var textUI = dat.GetComponent<UnityEngine.UI.Text>();
                if(textUI) {
                    dat.Apply();
                    EditorUtility.SetDirty(textUI);
                }
            }

            GUI.enabled = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace M8.UI.Graphics {
    [CustomEditor(typeof(ColorFromPalette))]
    public class ColorFromPaletteInspector : ColorFromPaletteBaseInspector {
        public override void OnInspectorGUI() {
            var dat = target as ColorFromPalette;

            var graphic = (Graphic)EditorGUILayout.ObjectField("Target", dat.target, typeof(Graphic), true);
            if(dat.target != graphic) {
                Undo.RecordObject(dat, "Changed Target");
                dat.target = graphic;
            }

            base.OnInspectorGUI();
        }
    }
}
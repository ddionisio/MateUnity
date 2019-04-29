using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(SpriteColorFromPalette))]
    public class SpriteColorFromPaletteInspector : ColorFromPaletteBaseInspector {
        public override void OnInspectorGUI() {
            var dat = target as SpriteColorFromPalette;

            var sprRender = (SpriteRenderer)EditorGUILayout.ObjectField("Target", dat.target, typeof(SpriteRenderer), true);
            if(dat.target != sprRender) {
                Undo.RecordObject(dat, "Changed Target");
                dat.target = sprRender;
            }

            base.OnInspectorGUI();
        }
    }
}
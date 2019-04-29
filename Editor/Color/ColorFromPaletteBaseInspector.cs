using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(ColorFromPaletteBase), true)]
    public class ColorFromPaletteBaseInspector : Editor {
        public override void OnInspectorGUI() {
            var dat = target as ColorFromPaletteBase;

            //Palette Edit
            EditorGUILayout.BeginVertical(GUI.skin.box);

            var clrPalette = (ColorPalette)EditorGUILayout.ObjectField("Palette", dat.palette, typeof(ColorPalette), true);
            if(dat.palette != clrPalette) {
                Undo.RecordObject(dat, "Changed Palette");
                dat.palette = clrPalette;
            }

            int ind;

            if(clrPalette)
                ind = EditorGUILayout.IntSlider("Index", dat.index, 0, clrPalette.count);
            else
                ind = EditorGUILayout.IntField("Index", dat.index);

            if(dat.index != ind) {
                Undo.RecordObject(dat, "Changed Palette Index");
                dat.index = ind;
            }

            EditorGUILayout.EndVertical();

            //Settings
            EditorGUILayout.BeginVertical(GUI.skin.box);

            var brightness = EditorGUILayout.Slider("Brightness", dat.brightness, 0f, 1f);
            if(dat.brightness != brightness) {
                Undo.RecordObject(dat, "Changed Brightness");
                dat.brightness = brightness;
            }

            var alpha = EditorGUILayout.Slider("Alpha", dat.alpha, 0f, 1f);
            if(dat.alpha != alpha) {
                Undo.RecordObject(dat, "Changed Alpha");
                dat.alpha = alpha;
            }

            EditorGUILayout.EndVertical();
        }
    }
}
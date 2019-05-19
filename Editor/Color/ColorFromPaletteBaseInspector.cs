using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ColorFromPaletteBase), true)]
    public class ColorFromPaletteBaseInspector : Editor {
        SerializedProperty mPalette;
        SerializedProperty mPaletteIndex;
        SerializedProperty mBrightness;
        SerializedProperty mAlpha;

        void OnDisable() {
            Undo.undoRedoPerformed -= ApplyColor;
        }

        void OnEnable() {
            mPalette = serializedObject.FindProperty("_palette");
            mPaletteIndex = serializedObject.FindProperty("_index");
            mBrightness = serializedObject.FindProperty("_brightness");
            mAlpha = serializedObject.FindProperty("_alpha");

            ApplyColor();

            Undo.undoRedoPerformed += ApplyColor;
        }

        void ApplyColor() {
            //refresh
            var dats = serializedObject.targetObjects;
            for(int i = 0; i < dats.Length; i++) {
                var dat = (ColorFromPaletteBase)dats[i];
                dat.ApplyColor();
            }
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            //var dat = target as ColorFromPaletteBase;

            serializedObject.Update();

            //Palette Edit
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.PropertyField(mPalette);

            if(mPalette.objectReferenceValue) {
                var palette = (ColorPalette)mPalette.objectReferenceValue;
                EditorGUILayout.IntSlider(mPaletteIndex, 0, palette.count - 1);
            }
            else
                EditorGUILayout.PropertyField(mPaletteIndex);

            EditorGUILayout.EndVertical();

            //Settings
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.Slider(mBrightness, 0f, 2f);

            EditorGUILayout.Slider(mAlpha, 0f, 1f);

            EditorGUILayout.EndVertical();

            if(serializedObject.ApplyModifiedProperties())
                ApplyColor();
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(SpriteColorOverride))]
    public class SpriteColorOverrideInspector : Editor {
        /*SpriteRenderer[] _targets;

        [SerializeField]
        bool _recursive = false;

        [SerializeField]
        bool _includeInactive = false;

        [SerializeField]
        Color _color;*/

        private SerializedProperty mTargets;
        private SerializedProperty mRecursive;
        private SerializedProperty mIncludeInactive;
        private SerializedProperty mColor;

        void OnEnable() {
            mTargets = serializedObject.FindProperty("_targets");
            mRecursive = serializedObject.FindProperty("_recursive");
            mIncludeInactive = serializedObject.FindProperty("_includeInactive");
            mColor = serializedObject.FindProperty("_color");

            ApplyColor();
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            if(GUI.changed) {
                ApplyColor();
            }
        }

        void ApplyColor() {
            Color clr = mColor.colorValue;

            if(mTargets.arraySize > 0) {
                for(int i = 0; i < mTargets.arraySize; i++) {
                    SpriteRenderer render = mTargets.GetArrayElementAtIndex(i).objectReferenceValue as SpriteRenderer;
                    if(render)
                        render.color = clr;
                }
            }
            else {
                bool recursive = mRecursive.boolValue;
                bool includeInactive = mIncludeInactive.boolValue;

                if(recursive) {
                    SpriteRenderer[] renders = (target as MonoBehaviour).GetComponentsInChildren<SpriteRenderer>(includeInactive);
                    for(int i = 0; i < renders.Length; i++) {
                        if(renders[i]) renders[i].color = clr;
                    }
                }
                else {
                    SpriteRenderer render = (target as MonoBehaviour).GetComponent<SpriteRenderer>();
                    if(render) render.color = clr;
                }
            }
        }
    }
}
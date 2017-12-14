using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(RendererSortingLayer))]
    public class RendererSortingLayerInspector : Editor {
        public override void OnInspectorGUI() {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if(EditorGUI.EndChangeCheck()) {
                var dat = target as RendererSortingLayer;
                dat.Apply();
            }
        }
    }
}
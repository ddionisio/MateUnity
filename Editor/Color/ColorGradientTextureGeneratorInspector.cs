using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace M8 {
    [CustomEditor(typeof(ColorGradientTextureGenerator))]
    public class ColorGradientTextureGeneratorInspector : Editor {
        private SerializedProperty mGradientProp;
        private SerializedProperty mTextureWidthProp;

        private GUIContent[] mTextureWidthLabels;
        private int[] mTextureWidths;

        void OnEnable() {
            mGradientProp = serializedObject.FindProperty("_gradient");
            mTextureWidthProp = serializedObject.FindProperty("_textureWidth");

            mTextureWidthLabels = new GUIContent[13];
            mTextureWidths = new int[13];
            for(int i = 0; i < mTextureWidths.Length; i++) {
                int val = (int)Mathf.Pow(2, i);
                mTextureWidthLabels[i] = new GUIContent(val.ToString());
                mTextureWidths[i] = val;
            }
        }

        /*private string GetFilename(string path) {
            int sInd = path.LastIndexOf('/');
            if(sInd == -1)
                sInd = 0;

            int eInd = path.LastIndexOf('.');
            if(eInd == -1)
                eInd = path.Length - 1;

            return path.Substring(sInd, path.Length - eInd + 1);
        }*/

        private string GetTextureFilePath() {
            var objName = target.name;
            var objPath = AssetDatabase.GetAssetPath(target);

            string path;

            int extInd = objPath.LastIndexOf('.');
            if(extInd != -1)
                path = objPath.Substring(0, extInd);
            else
                path = objPath.Substring(0, objPath.Length);

            path += "_texture.png";

            return path;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(mGradientProp);

            EditorGUILayout.IntPopup(mTextureWidthProp, mTextureWidthLabels, mTextureWidths);

            serializedObject.ApplyModifiedProperties();

            if(GUILayout.Button("Generate")) {
                var obj = target as ColorGradientTextureGenerator;

                var tex = obj.Generate();

                var texDat = tex.EncodeToPNG();
                if(texDat != null) {
                    var textureFilePath = GetTextureFilePath();

                    File.WriteAllBytes(textureFilePath, texDat);

                    AssetDatabase.Refresh();
                }

                DestroyImmediate(tex);
            }
        }
    }
}
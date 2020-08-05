using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace M8 {
    [CustomEditor(typeof(ColorGradientTextureGenerator))]
    public class ColorGradientTextureGeneratorInspector : Editor {
        private SerializedProperty mModeProp;

        private SerializedProperty mMonoProp;

        private SerializedProperty mClrBeginProp;
        private SerializedProperty mClrEndProp;

        private SerializedProperty mGradientProp;

        private SerializedProperty mCurveProp;

        private SerializedProperty mStepsProp;

        private SerializedProperty mTextureWidthProp;
        private SerializedProperty mTextureFilterProp;
        private SerializedProperty mTextureWrapProp;

        private SerializedProperty mTextureProp;

        private GUIContent[] mTextureWidthLabels;
        private int[] mTextureWidths;

        void OnEnable() {
            mModeProp = serializedObject.FindProperty("_mode");

            mMonoProp = serializedObject.FindProperty("_mono");

            mClrBeginProp = serializedObject.FindProperty("_colorBegin");
            mClrEndProp = serializedObject.FindProperty("_colorEnd");

            mGradientProp = serializedObject.FindProperty("_gradient");

            mCurveProp = serializedObject.FindProperty("_curve");

            mStepsProp = serializedObject.FindProperty("_steps");

            mTextureWidthProp = serializedObject.FindProperty("_textureWidth");            

            mTextureWidthLabels = new GUIContent[13];
            mTextureWidths = new int[13];
            for(int i = 0; i < mTextureWidths.Length; i++) {
                int val = (int)Mathf.Pow(2, i);
                mTextureWidthLabels[i] = new GUIContent(val.ToString());
                mTextureWidths[i] = val;
            }

            mTextureFilterProp = serializedObject.FindProperty("_textureFilterMode");
            mTextureWrapProp = serializedObject.FindProperty("_textureWrapMode");
            mTextureProp = serializedObject.FindProperty("_textureTarget");
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
            string path;

            if(mTextureProp.objectReferenceValue) {
                path = AssetDatabase.GetAssetPath(mTextureProp.objectReferenceValue);
            }
            else {
                //var objName = target.name;
                var objPath = AssetDatabase.GetAssetPath(target);

                int extInd = objPath.LastIndexOf('.');
                if(extInd != -1)
                    path = objPath.Substring(0, extInd);
                else
                    path = objPath.Substring(0, objPath.Length);

                path += "_texture.png";
            }

            return path;
        }

        private string GetTextureDir() {
            string path;

            if(mTextureProp.objectReferenceValue)
                path = AssetDatabase.GetAssetPath(mTextureProp.objectReferenceValue);
            else
                path = AssetDatabase.GetAssetPath(target);

            int extInd = path.LastIndexOf('/');
            if(extInd == -1)
                return "Assets";

            return path.Substring(0, extInd);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            //Mode
            EditorGUILayout.PropertyField(mModeProp);

            var mode = (ColorGradientTextureGenerator.Mode)mModeProp.enumValueIndex;
            var isMono = mode != ColorGradientTextureGenerator.Mode.Gradient && mMonoProp.boolValue;

            //Mode Settings            
            switch(mode) {
                case ColorGradientTextureGenerator.Mode.Gradient:
                    EditorGUILayout.PropertyField(mGradientProp);
                    break;

                case ColorGradientTextureGenerator.Mode.Curve:
                    EditorGUILayout.PropertyField(mCurveProp);
                    break;

                case ColorGradientTextureGenerator.Mode.Steps:
                    EditorGUILayout.IntSlider(mStepsProp, 1, 256);
                    break;
            }

            //Mode Shared Settings
            switch(mode) {
                case ColorGradientTextureGenerator.Mode.Curve:
                case ColorGradientTextureGenerator.Mode.Steps:
                    EditorGUILayout.PropertyField(mMonoProp);

                    if(!mMonoProp.boolValue) {
                        EditorGUILayout.PropertyField(mClrBeginProp);
                        EditorGUILayout.PropertyField(mClrEndProp);
                    }
                    break;
            }

            //Texture Properties
            if(mode != ColorGradientTextureGenerator.Mode.Steps)
                EditorGUILayout.IntPopup(mTextureWidthProp, mTextureWidthLabels, mTextureWidths);

            EditorGUILayout.PropertyField(mTextureWrapProp);
            EditorGUILayout.PropertyField(mTextureFilterProp);            

            EditorExt.Utility.DrawSeparator();

            EditorGUILayout.PropertyField(mTextureProp, new GUIContent("Output"));

            serializedObject.ApplyModifiedProperties();

            //Texture Generate
            string savePath = null;

            if(GUILayout.Button("Save"))
                savePath = GetTextureFilePath();

            if(GUILayout.Button("Save As...")) {
                var defaultName = mTextureProp.objectReferenceValue ? mTextureProp.objectReferenceValue.name : target.name + "_texture";

                savePath = EditorUtility.SaveFilePanel("Save Texture", GetTextureDir(), defaultName, "png");

                int assetInd = savePath.IndexOf("Assets", System.StringComparison.Ordinal);
                if(assetInd != -1)
                    savePath = savePath.Remove(0, assetInd);
            }

            if(!string.IsNullOrEmpty(savePath)) {
                var obj = target as ColorGradientTextureGenerator;

                var tex = obj.Generate();

                var texDat = tex.EncodeToPNG();
                if(texDat != null) {
                    File.WriteAllBytes(savePath, texDat);
                    AssetDatabase.Refresh();

                    var texImport = (TextureImporter)AssetImporter.GetAtPath(savePath);
                    if(texImport != null) {
                        texImport.wrapMode = tex.wrapMode;
                        texImport.filterMode = tex.filterMode;
                        texImport.textureType = isMono ? TextureImporterType.SingleChannel : TextureImporterType.Default;
                        texImport.alphaIsTransparency = !isMono;                        
                        texImport.textureCompression = TextureImporterCompression.Uncompressed;
                        texImport.mipmapEnabled = false;
                        var textureSettings = new TextureImporterPlatformSettings {
                            format = isMono ? TextureImporterFormat.R8 : TextureImporterFormat.Automatic
                        };
                        texImport.SetPlatformTextureSettings(textureSettings);
                        EditorUtility.SetDirty(texImport);
                        texImport.SaveAndReimport();

                        mTextureProp.objectReferenceValue = AssetDatabase.LoadAssetAtPath(texImport.assetPath, typeof(Texture2D));
                        serializedObject.ApplyModifiedProperties();
                    }
                }

                DestroyImmediate(tex);
            }
        }
    }
}
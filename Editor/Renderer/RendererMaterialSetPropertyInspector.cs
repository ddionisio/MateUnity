using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(RendererMaterialSetProperty))]
    public class RendererMaterialSetPropertyInspector : Editor {

        public override void OnInspectorGUI() {
            var dat = target as RendererMaterialSetProperty;

            //General Settings
            var applyOnAwake = EditorGUILayout.Toggle("Apply On Awake", dat.applyOnAwake);
            if(dat.applyOnAwake != applyOnAwake) {
                Undo.RecordObject(dat, "Apply On Awake");
                dat.applyOnAwake = applyOnAwake;
            }

            EditorExt.Utility.DrawSeparator();

            //Material Properties
            if(dat.renderTarget) {
                var mat = dat.renderTarget.sharedMaterial;

                if(mat) {
                    bool isChanged = false;
                    List<RendererMaterialSetProperty.PropertyInfo> propertyList = null;

                    if(dat.properties != null)
                        propertyList = new List<RendererMaterialSetProperty.PropertyInfo>(dat.properties);
                    else {
                        propertyList = new List<RendererMaterialSetProperty.PropertyInfo>();
                        isChanged = true;
                    }

                    string[] names, details;
                    RendererMaterialSetProperty.ValueType[] valueTypes;
                    int[] inds;

                    GetPropertyInfos(mat, out names, out details, out valueTypes, out inds);

                    for(int i = 0; i < propertyList.Count; i++) {
                        GUILayout.BeginHorizontal(GUI.skin.box);

                        var itm = propertyList[i];

                        //select variable
                        int curInd = System.Array.IndexOf(names, itm.name);

                        int ind = EditorGUILayout.IntPopup(curInd, details, inds);

                        if(curInd != ind) {
                            itm.name = names[ind];
                            itm.valueType = valueTypes[ind];

                            //apply initial value
                            itm.SetValueFrom(mat);

                            isChanged = true;
                        }

                        var valueVector = itm.valueVector;

                        switch(itm.valueType) {
                            case RendererMaterialSetProperty.ValueType.Color:
                                var curClr = new Color(valueVector.x, valueVector.y, valueVector.z, valueVector.w);
                                var clr = EditorGUILayout.ColorField(curClr);
                                if(curClr != clr) {
                                    itm.valueVector = clr;
                                    isChanged = true;
                                }
                                break;

                            case RendererMaterialSetProperty.ValueType.Vector:
                                var vec = EditorGUILayout.Vector4Field("", valueVector);
                                if(valueVector != vec) {
                                    itm.valueVector = vec;
                                    isChanged = true;
                                }
                                break;

                            case RendererMaterialSetProperty.ValueType.Float:
                            case RendererMaterialSetProperty.ValueType.Range:
                                GUILayout.Space(4f);

                                EditorGUIUtility.labelWidth = 16f;

                                var fval = EditorGUILayout.FloatField("=", valueVector.x);
                                if(valueVector.x != fval) {
                                    valueVector.x = fval;
                                    itm.valueVector = valueVector;
                                    isChanged = true;
                                }

                                EditorGUIUtility.labelWidth = 0f;
                                break;

                            case RendererMaterialSetProperty.ValueType.TexEnv:
                                var tex = (Texture)EditorGUILayout.ObjectField(itm.valueTexture, typeof(Texture), false);
                                if(itm.valueTexture != tex) {
                                    itm.valueTexture = tex;
                                    isChanged = true;
                                }
                                break;

                            case RendererMaterialSetProperty.ValueType.TexOfs:
                            case RendererMaterialSetProperty.ValueType.TexScale:
                                var vec2 = EditorGUILayout.Vector2Field("", valueVector);
                                if(valueVector.x != vec2.x || valueVector.y != vec2.y) {
                                    itm.valueVector = vec2;
                                    isChanged = true;
                                }
                                break;
                        }

                        if(EditorExt.Utility.DrawSimpleButton("R", "Reset Value.")) {
                            itm.SetValueFrom(mat);
                            isChanged = true;
                        }

                        GUILayout.Space(16f);

                        bool isRemove = EditorExt.Utility.DrawRemoveButton();

                        if(isChanged)
                            propertyList[i] = itm;

                        GUILayout.EndHorizontal();

                        if(isRemove) {
                            propertyList.RemoveAt(i);
                            isChanged = true;
                            break;
                        }
                    }

                    if(GUILayout.Button("Add Property")) {
                        propertyList.Add(new RendererMaterialSetProperty.PropertyInfo { name = "", valueType = RendererMaterialSetProperty.ValueType.None });
                        isChanged = true;
                    }

                    if(isChanged) {
                        Undo.RecordObject(dat, "Property Changed");

                        if(propertyList != null)
                            dat.properties = propertyList.ToArray();

                        if(Application.isPlaying)
                            dat.Apply();
                    }
                }
                else
                    GUILayout.Label("Renderer needs a Material.");
            }
            else {
                GUILayout.Label("No Renderer found.");
            }
        }

        private void GetPropertyInfos(Material mat, out string[] names, out string[] details, out RendererMaterialSetProperty.ValueType[] types, out int[] inds) {
            Shader shader = mat.shader;
            int count = ShaderUtil.GetPropertyCount(shader);

            List<string> _names = new List<string>();
            List<RendererMaterialSetProperty.ValueType> _types = new List<RendererMaterialSetProperty.ValueType>();

            for(int i = 0; i < count; i++) {
                var name = ShaderUtil.GetPropertyName(shader, i);
                var type = ShaderUtil.GetPropertyType(shader, i);

                _names.Add(name);
                _types.Add((RendererMaterialSetProperty.ValueType)type);

                if(type == ShaderUtil.ShaderPropertyType.TexEnv) {
                    _names.Add(name); _types.Add(RendererMaterialSetProperty.ValueType.TexOfs);
                    _names.Add(name); _types.Add(RendererMaterialSetProperty.ValueType.TexScale);
                }
            }

            count = _names.Count;

            names = new string[count];
            details = new string[count];
            types = new RendererMaterialSetProperty.ValueType[count];
            inds = new int[count];

            for(int i = 0; i < count; i++) {
                names[i] = _names[i];
                types[i] = _types[i];

                switch(types[i]) {
                    case RendererMaterialSetProperty.ValueType.TexOfs:
                        details[i] = string.Format("{0} Offset", names[i]);
                        break;
                    case RendererMaterialSetProperty.ValueType.TexScale:
                        details[i] = string.Format("{0} Scale", names[i]);
                        break;
                    default:
                        details[i] = names[i];
                        break;
                }

                inds[i] = i;
            }
        }
    }
}
using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(ShaderGlobalVariables))]
    public class ShaderGlobalVariablesInspector : Editor {
        private bool mValuesFoldout = true;

        public override void OnInspectorGUI() {
            ShaderGlobalVariables dat = target as ShaderGlobalVariables;

            //Values
            mValuesFoldout = EditorGUILayout.Foldout(mValuesFoldout, "Values");
            if(mValuesFoldout) {
                if(dat.values != null) {
                    for(int i = 0; i < dat.values.Length; i++) {
                        var val = dat.values[i];

                        EditorGUI.BeginChangeCheck();

                        GUILayout.BeginVertical(GUI.skin.box);
                                                                                                
                        //name
                        GUILayout.BeginHorizontal();

                        val.name = EditorGUILayout.TextField(val.name);

                        GUILayout.FlexibleSpace();

                        if(EditorExt.Utility.DrawAddButton()) {
                            Undo.RecordObject(target, "Shader Global Variables Add");
                            ArrayUtil.InsertAfter(ref dat.values, i, val.Clone());                            
                            break;
                        }

                        GUILayout.Space(4f);

                        if(EditorExt.Utility.DrawRemoveButton()) {
                            Undo.RecordObject(target, "Shader Global Variables Remove");
                            ArrayUtil.RemoveAt(ref dat.values, i);
                            break;
                        }

                        GUILayout.EndHorizontal();

                        //type
                        val.type = (ShaderGlobalVariables.Type)EditorGUILayout.EnumPopup(val.type);

                        //value
                        switch(val.type) {
                            case ShaderGlobalVariables.Type.Float:
                                val.val = EditorGUILayout.FloatField("Value", val.val);
                                val.texture = null;
                                break;
                            case ShaderGlobalVariables.Type.Vector:
                                val.val4 = EditorGUILayout.Vector4Field("Value", val.val4);
                                val.texture = null;
                                break;
                            case ShaderGlobalVariables.Type.Color:
                                val.color = EditorGUILayout.ColorField(val.color);
                                val.texture = null;
                                break;
                            case ShaderGlobalVariables.Type.Texture:
                                val.texture = EditorGUILayout.ObjectField(val.texture, typeof(Texture), false) as Texture;
                                break;
                        }
                        
                        GUILayout.EndVertical();

                        if(EditorGUI.EndChangeCheck()) {
                            Undo.RecordObject(target, "Shader Global Variables Edit - "+dat.values[i].name);
                            dat.values[i] = val;
                        }
                    }
                }

                //add new
                if(GUILayout.Button("Add New Value")) {
                    Undo.RecordObject(target, "Shader Global Variables Add");

                    if(dat.values == null || dat.values.Length == 0) {
                        dat.values = new ShaderGlobalVariables.Data[1] { new ShaderGlobalVariables.Data() };
                    }
                    else {
                        var newDat = dat.values[dat.values.Length - 1].Clone();
                        System.Array.Resize(ref dat.values, dat.values.Length + 1);
                        dat.values[dat.values.Length - 1] = newDat;
                    }
                }
            }

            EditorExt.Utility.DrawSeparator();

            //Component settings
            EditorGUI.BeginChangeCheck();

            var applyOnAwake = GUILayout.Toggle(dat.applyOnAwake, "Apply On Awake");

            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Change Shader Global Variables Component Setting");

                dat.applyOnAwake = applyOnAwake;
            }
            

            //Controls
            if(GUILayout.Button("Refresh")) {
                dat.Apply();
            }
        }
    }
}
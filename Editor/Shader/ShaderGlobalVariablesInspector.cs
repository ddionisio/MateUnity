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
                        GUILayout.BeginVertical(GUI.skin.box);

                        var val = dat.values[i];

                        //name
                        GUILayout.BeginHorizontal();

                        val.name = EditorGUILayout.TextField(val.name);

                        GUILayout.FlexibleSpace();

                        if(EditorExt.Utility.DrawAddButton()) {
                            ArrayUtil.InsertAfter(ref dat.values, i, val.Clone());
                            GUI.changed = true;
                            break;
                        }

                        GUILayout.Space(4f);

                        if(EditorExt.Utility.DrawRemoveButton()) {
                            ArrayUtil.RemoveAt(ref dat.values, i);
                            GUI.changed = true;
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
                    }
                }

                //add new
                if(GUILayout.Button("Add New Value")) {
                    if(dat.values == null || dat.values.Length == 0) {
                        dat.values = new ShaderGlobalVariables.Data[1] { new ShaderGlobalVariables.Data() };
                    }
                    else {
                        var newDat = dat.values[dat.values.Length - 1].Clone();
                        System.Array.Resize(ref dat.values, dat.values.Length + 1);
                        dat.values[dat.values.Length - 1] = newDat;
                    }

                    GUI.changed = true;
                }
            }

            EditorExt.Utility.DrawSeparator();

            //Component settings
            dat.applyOnAwake = GUILayout.Toggle(dat.applyOnAwake, "Apply On Awake");

            if(GUI.changed) {
                EditorUtility.SetDirty(dat);
            }

            //Controls
            if(GUILayout.Button("Refresh")) {
                dat.Apply();
            }
        }
    }
}
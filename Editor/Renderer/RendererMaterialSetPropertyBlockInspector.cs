using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CustomEditor(typeof(RendererMaterialSetPropertyBlock))]
	public class RendererMaterialSetPropertyBlockInspector : Editor {
		public override void OnInspectorGUI() {
			var dat = target as RendererMaterialSetPropertyBlock;

			if(dat.renderTarget) {
				var mat = dat.renderTarget.sharedMaterial;

				if(mat) {
					bool isChanged = false;
					List<RendererMaterialSetPropertyBlock.PropertyInfo> propertyList = null;

					if(dat.properties != null)
						propertyList = new List<RendererMaterialSetPropertyBlock.PropertyInfo>(dat.properties);
					else {
						propertyList = new List<RendererMaterialSetPropertyBlock.PropertyInfo>();
						isChanged = true;
					}

					string[] names, details;
					RendererMaterialSetPropertyBlock.ValueType[] valueTypes;
					int[] inds;

					GetPropertyInfos(mat, out names, out details, out valueTypes, out inds);

					for(int i = 0; i < propertyList.Count; i++) {
						GUILayout.BeginHorizontal(GUI.skin.box);

						var itm = propertyList[i];

						//select variable
						int curInd = -1;

						for(int j = 0; j < names.Length; j++) {
							if(names[j] == itm.name) {
								curInd = j;
								break;
							}
						}

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
							case RendererMaterialSetPropertyBlock.ValueType.Color:
								var curClr = new Color(valueVector.x, valueVector.y, valueVector.z, valueVector.w);
								var clr = EditorGUILayout.ColorField(curClr);
								if(curClr != clr) {
									itm.valueVector = clr;
									isChanged = true;
								}
								break;

							case RendererMaterialSetPropertyBlock.ValueType.Vector:
								var vec = EditorGUILayout.Vector4Field("", valueVector);
								if(valueVector != vec) {
									itm.valueVector = vec;
									isChanged = true;
								}
								break;

							case RendererMaterialSetPropertyBlock.ValueType.Float:
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

							case RendererMaterialSetPropertyBlock.ValueType.Range:
								GUILayout.Space(4f);

								EditorGUIUtility.labelWidth = 16f;

								float rval;
								Vector2 range;

								if(GetPropertyRange(mat, itm.name, out range))
									rval = EditorGUILayout.Slider("=", valueVector.x, range.x, range.y);
								else
									rval = EditorGUILayout.FloatField("=", valueVector.x);

								if(valueVector.x != rval) {
									valueVector.x = rval;
									itm.valueVector = valueVector;
									isChanged = true;
								}

								EditorGUIUtility.labelWidth = 0f;
								break;

							case RendererMaterialSetPropertyBlock.ValueType.TexEnv:
								var tex = (Texture)EditorGUILayout.ObjectField(itm.valueTexture, typeof(Texture), false);
								if(itm.valueTexture != tex) {
									itm.valueTexture = tex;
									isChanged = true;
								}
								break;

							case RendererMaterialSetPropertyBlock.ValueType.Int:
								GUILayout.Space(4f);

								EditorGUIUtility.labelWidth = 16f;

								var _fval = (float)EditorGUILayout.IntField("=", (int)valueVector.x);
								if(valueVector.x != _fval) {
									valueVector.x = _fval;
									itm.valueVector = valueVector;
									isChanged = true;
								}

								EditorGUIUtility.labelWidth = 0f;
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
						propertyList.Add(new RendererMaterialSetPropertyBlock.PropertyInfo { name = "", valueType = RendererMaterialSetPropertyBlock.ValueType.None });
						isChanged = true;
					}

					if(isChanged) {
						Undo.RecordObject(dat, "Property Changed");

						if(propertyList != null)
							dat.properties = propertyList.ToArray();

						dat.Apply();
					}

					if(GUILayout.Button("Refresh"))
						dat.Apply();
				}
				else
					GUILayout.Label("Renderer needs a Material.");
			}
			else {
				GUILayout.Label("No Renderer found.");
			}
		}

		private bool GetPropertyRange(Material mat, string propName, out Vector2 range) {
			var shader = mat.shader;
			var count = ShaderUtil.GetPropertyCount(shader);

			for(int i = 0; i < count; i++) {
				var name = ShaderUtil.GetPropertyName(shader, i);
				var type = ShaderUtil.GetPropertyType(shader, i);

				if(name == propName && type == ShaderUtil.ShaderPropertyType.Range) {
					range = new Vector2(ShaderUtil.GetRangeLimits(shader, i, 1), ShaderUtil.GetRangeLimits(shader, i, 2));
					return true;
				}
			}

			range = Vector2.zero;
			return false;
		}

		private void GetPropertyInfos(Material mat, out string[] names, out string[] details, out RendererMaterialSetPropertyBlock.ValueType[] types, out int[] inds) {
			Shader shader = mat.shader;
			int count = ShaderUtil.GetPropertyCount(shader);

			var _names = new List<string>();
			var _details = new List<string>();
			var _types = new List<RendererMaterialSetPropertyBlock.ValueType>();

			for(int i = 0; i < count; i++) {
				var isHidden = ShaderUtil.IsShaderPropertyHidden(shader, i);
				if(isHidden)
					continue;

				var name = ShaderUtil.GetPropertyName(shader, i);
				var type = ShaderUtil.GetPropertyType(shader, i);
				var detail = ShaderUtil.GetPropertyDescription(shader, i);

				_names.Add(name);

				if(!string.IsNullOrEmpty(detail))
					_details.Add(detail);
				else
					_details.Add(name);

				_types.Add((RendererMaterialSetPropertyBlock.ValueType)type);
			}

			count = _names.Count;

			names = new string[count];
			details = new string[count];
			types = new RendererMaterialSetPropertyBlock.ValueType[count];
			inds = new int[count];

			for(int i = 0; i < count; i++) {
				names[i] = _names[i];
				details[i] = _details[i];
				types[i] = _types[i];
				inds[i] = i;
			}
		}
	}
}
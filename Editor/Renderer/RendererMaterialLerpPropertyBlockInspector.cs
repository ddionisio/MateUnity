using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NPOI.SS.Formula.Functions;

namespace M8 {
    [CustomEditor(typeof(RendererMaterialLerpPropertyBlock))]
    public class RendererMaterialLerpPropertyBlockInspector : Editor {
		public override void OnInspectorGUI() {
			var targetProp = serializedObject.FindProperty("_target");

			var nameProp = serializedObject.FindProperty("_name");

			var valTypeProp = serializedObject.FindProperty("_valueType");

			var valVectorStartProp = serializedObject.FindProperty("_valueVectorStart");
			var valVectorEndProp = serializedObject.FindProperty("_valueVectorEnd");

			var timeProp = serializedObject.FindProperty("_time");

			var isReset = false;

			EditorGUILayout.PropertyField(targetProp);

			var renderTarget = targetProp.objectReferenceValue as Renderer;
			if(renderTarget) {
				var mat = renderTarget.sharedMaterial;
				if(mat) {
					string[] names, details;
					RendererMaterialLerpPropertyBlock.ValueType[] valueTypes;
					int[] inds;

					GetPropertyInfos(mat, out names, out details, out valueTypes, out inds);

					//select variable
					var propName = nameProp.stringValue;

					int curInd = -1;

					for(int i = 0; i < names.Length; i++) {
						if(names[i] == propName) {
							curInd = i;
							break;
						}
					}

					int ind = EditorGUILayout.IntPopup("Property", curInd, details, inds);

					//apply property id and value type
					if(curInd != ind) {
						nameProp.stringValue = propName = names[ind];

						valTypeProp.intValue = (int)valueTypes[ind];

						isReset = true;
					}
					else if(ind != -1) {
						//variable setup
						var valVectorStart = valVectorStartProp.vector4Value;
						var valVectorEnd = valVectorEndProp.vector4Value;

						switch(valueTypes[ind]) {
							case RendererMaterialLerpPropertyBlock.ValueType.Color:
								valVectorStartProp.vector4Value = EditorGUILayout.ColorField("Start", valVectorStart);
								valVectorEndProp.vector4Value = EditorGUILayout.ColorField("End", valVectorEnd);
								break;

							case RendererMaterialLerpPropertyBlock.ValueType.Vector:
								valVectorStartProp.vector4Value = EditorGUILayout.Vector4Field("Start", valVectorStart);
								valVectorEndProp.vector4Value = EditorGUILayout.Vector4Field("End", valVectorEnd);
								break;

							case RendererMaterialLerpPropertyBlock.ValueType.Float:
								valVectorStart.x = EditorGUILayout.FloatField("Start", valVectorStart.x);
								valVectorEnd.x = EditorGUILayout.FloatField("End", valVectorEnd.x);

								valVectorStartProp.vector4Value = valVectorStart;
								valVectorEndProp.vector4Value = valVectorEnd;
								break;

							case RendererMaterialLerpPropertyBlock.ValueType.Range:
								Vector2 range;
								if(GetPropertyRange(mat, propName, out range)) {
									valVectorStart.x = EditorGUILayout.Slider("Start", valVectorStart.x, range.x, range.y);
									valVectorEnd.x = EditorGUILayout.Slider("End", valVectorEnd.x, range.x, range.y);
								}
								else {
									valVectorStart.x = EditorGUILayout.FloatField("Start", valVectorStart.x);
									valVectorEnd.x = EditorGUILayout.FloatField("End", valVectorEnd.x);
								}

								valVectorStartProp.vector4Value = valVectorStart;
								valVectorEndProp.vector4Value = valVectorEnd;
								break;

							case RendererMaterialLerpPropertyBlock.ValueType.Int:
								valVectorStart.x = EditorGUILayout.IntField("Start", Mathf.RoundToInt(valVectorStart.x));
								valVectorEnd.x = EditorGUILayout.IntField("End", Mathf.RoundToInt(valVectorEnd.x));

								valVectorStartProp.vector4Value = valVectorStart;
								valVectorEndProp.vector4Value = valVectorEnd;
								break;
						}

						timeProp.floatValue = EditorGUILayout.Slider("Time", timeProp.floatValue, 0f, 1f);
					}
				}
				else
					GUILayout.Label("Renderer target needs a Material.");
			}
			else
				GUILayout.Label("Renderer target not set.");

			if(serializedObject.ApplyModifiedProperties()) {
				var dat = target as RendererMaterialLerpPropertyBlock;

				if(isReset)
					dat.ResetValue();
				else
					dat.Apply();
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

		private void GetPropertyInfos(Material mat, out string[] names, out string[] details, out RendererMaterialLerpPropertyBlock.ValueType[] types, out int[] inds) {
			Shader shader = mat.shader;
			int count = ShaderUtil.GetPropertyCount(shader);

			var _names = new List<string>();
			var _details = new List<string>();
			var _types = new List<RendererMaterialLerpPropertyBlock.ValueType>();

			for(int i = 0; i < count; i++) {
				var isHidden = ShaderUtil.IsShaderPropertyHidden(shader, i);
				if(isHidden)
					continue;

				var type = ShaderUtil.GetPropertyType(shader, i);
				if(type == ShaderUtil.ShaderPropertyType.TexEnv)
					continue;

				var name = ShaderUtil.GetPropertyName(shader, i);				
				var detail = ShaderUtil.GetPropertyDescription(shader, i);

				_names.Add(name);

				if(!string.IsNullOrEmpty(detail))
					_details.Add(detail);
				else
					_details.Add(name);

				_types.Add((RendererMaterialLerpPropertyBlock.ValueType)type);
			}

			count = _names.Count;

			names = new string[count];
			details = new string[count];
			types = new RendererMaterialLerpPropertyBlock.ValueType[count];
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
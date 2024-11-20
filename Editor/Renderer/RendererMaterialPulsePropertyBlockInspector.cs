using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CustomEditor(typeof(RendererMaterialPulsePropertyBlock))]
	public class RendererMaterialPulsePropertyBlockInspector : Editor {
		public override void OnInspectorGUI() {
			var targetProp = serializedObject.FindProperty("_target");

			var nameProp = serializedObject.FindProperty("_name");

			var valTypeProp = serializedObject.FindProperty("_valueType");

			var valVectorStartProp = serializedObject.FindProperty("_valueVectorStart");
			var valVectorEndProp = serializedObject.FindProperty("_valueVectorEnd");

			var pulsePerSecProp = serializedObject.FindProperty("_pulsePerSecond");
			var isRealtimeProp = serializedObject.FindProperty("_isRealTime");

			EditorGUILayout.PropertyField(targetProp);

			var renderTarget = targetProp.objectReferenceValue as Renderer;
			if(renderTarget) {
				var mat = renderTarget.sharedMaterial;
				if(mat) {
					string[] names, details;
					RendererMaterialPulsePropertyBlock.ValueType[] valueTypes;
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
					}
					else if(ind != -1) {
						//variable setup
						var valVectorStart = valVectorStartProp.vector4Value;
						var valVectorEnd = valVectorEndProp.vector4Value;

						switch(valueTypes[ind]) {
							case RendererMaterialPulsePropertyBlock.ValueType.Color:
								valVectorStartProp.vector4Value = EditorGUILayout.ColorField("Start", valVectorStart);
								valVectorEndProp.vector4Value = EditorGUILayout.ColorField("End", valVectorEnd);
								break;

							case RendererMaterialPulsePropertyBlock.ValueType.Vector:
								valVectorStartProp.vector4Value = EditorGUILayout.Vector4Field("Start", valVectorStart);
								valVectorEndProp.vector4Value = EditorGUILayout.Vector4Field("End", valVectorEnd);
								break;

							case RendererMaterialPulsePropertyBlock.ValueType.Float:
								valVectorStart.x = EditorGUILayout.FloatField("Start", valVectorStart.x);
								valVectorEnd.x = EditorGUILayout.FloatField("End", valVectorEnd.x);

								valVectorStartProp.vector4Value = valVectorStart;
								valVectorEndProp.vector4Value = valVectorEnd;
								break;

							case RendererMaterialPulsePropertyBlock.ValueType.Range:
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

							case RendererMaterialPulsePropertyBlock.ValueType.Int:
								valVectorStart.x = EditorGUILayout.IntField("Start", Mathf.RoundToInt(valVectorStart.x));
								valVectorEnd.x = EditorGUILayout.IntField("End", Mathf.RoundToInt(valVectorEnd.x));

								valVectorStartProp.vector4Value = valVectorStart;
								valVectorEndProp.vector4Value = valVectorEnd;
								break;
						}
					}
				}
				else
					GUILayout.Label("Renderer target needs a Material.");
			}
			else
				GUILayout.Label("Renderer target not set.");

			EditorExt.Utility.DrawSeparator();

			pulsePerSecProp.floatValue = EditorGUILayout.FloatField("Pulse Per Second", pulsePerSecProp.floatValue);
			isRealtimeProp.boolValue = EditorGUILayout.Toggle("Is Realtime", isRealtimeProp.boolValue);

			serializedObject.ApplyModifiedProperties();
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

		private void GetPropertyInfos(Material mat, out string[] names, out string[] details, out RendererMaterialPulsePropertyBlock.ValueType[] types, out int[] inds) {
			Shader shader = mat.shader;
			int count = ShaderUtil.GetPropertyCount(shader);

			var _names = new List<string>();
			var _details = new List<string>();
			var _types = new List<RendererMaterialPulsePropertyBlock.ValueType>();

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

				_types.Add((RendererMaterialPulsePropertyBlock.ValueType)type);
			}

			count = _names.Count;

			names = new string[count];
			details = new string[count];
			types = new RendererMaterialPulsePropertyBlock.ValueType[count];
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
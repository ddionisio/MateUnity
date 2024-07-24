using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CustomEditor(typeof(RendererMaterialSetPropertyBlockProxy))]
	public class RendererMaterialSetPropertyBlockProxyInspector : Editor {
		public override void OnInspectorGUI() {
			var targetProp = serializedObject.FindProperty("_target");

			var nameProp = serializedObject.FindProperty("_name");

			var valTypeProp = serializedObject.FindProperty("_valueType");

			var valVectorProp = serializedObject.FindProperty("_valueVector");
			var valTexProp = serializedObject.FindProperty("_valueTexture");

			var isReset = false;

			EditorGUILayout.PropertyField(targetProp);

			var renderTarget = targetProp.objectReferenceValue as Renderer;
			if(renderTarget) {
				var mat = renderTarget.sharedMaterial;
				if(mat) {
					string[] names, details;
					RendererMaterialSetPropertyBlockProxy.ValueType[] valueTypes;
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
						Vector4 valVector = valVectorProp.vector4Value;

						switch(valueTypes[ind]) {
							case RendererMaterialSetPropertyBlockProxy.ValueType.Color:
								valVectorProp.vector4Value = EditorGUILayout.ColorField("Value", valVector);
								break;

							case RendererMaterialSetPropertyBlockProxy.ValueType.Vector:
								valVectorProp.vector4Value = EditorGUILayout.Vector4Field("Value", valVector);
								break;

							case RendererMaterialSetPropertyBlockProxy.ValueType.Float:
								valVector.x = EditorGUILayout.FloatField("Value", valVector.x);
								valVectorProp.vector4Value = valVector;
								break;

							case RendererMaterialSetPropertyBlockProxy.ValueType.Range:
								Vector2 range;
								if(GetPropertyRange(mat, propName, out range))
									valVector.x = EditorGUILayout.Slider("Value", valVector.x, range.x, range.y);								
								else
									valVector.x = EditorGUILayout.FloatField("Value", valVector.x);

								valVectorProp.vector4Value = valVector;
								break;

							case RendererMaterialSetPropertyBlockProxy.ValueType.TexEnv:
								EditorGUILayout.PropertyField(valTexProp, new GUIContent("Value"));
								break;

							case RendererMaterialSetPropertyBlockProxy.ValueType.Int:
								valVector.x = EditorGUILayout.IntField("Value", (int)valVector.x);
								valVectorProp.vector4Value = valVector;
								break;
						}
					}
				}
				else
					GUILayout.Label("Renderer target needs a Material.");
			}
			else
				GUILayout.Label("Renderer target not set.");

			if(serializedObject.ApplyModifiedProperties()) {
				var dat = target as RendererMaterialSetPropertyBlockProxy;

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

		private void GetPropertyInfos(Material mat, out string[] names, out string[] details, out RendererMaterialSetPropertyBlockProxy.ValueType[] types, out int[] inds) {
			Shader shader = mat.shader;
			int count = ShaderUtil.GetPropertyCount(shader);

			var _names = new List<string>();
			var _details = new List<string>();
			var _types = new List<RendererMaterialSetPropertyBlockProxy.ValueType>();

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

				_types.Add((RendererMaterialSetPropertyBlockProxy.ValueType)type);
			}

			count = _names.Count;

			names = new string[count];
			details = new string[count];
			types = new RendererMaterialSetPropertyBlockProxy.ValueType[count];
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
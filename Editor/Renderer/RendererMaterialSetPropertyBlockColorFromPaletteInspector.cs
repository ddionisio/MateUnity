using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CanEditMultipleObjects]
	[CustomEditor(typeof(RendererMaterialSetPropertyBlockColorFromPalette))]
    public class RendererMaterialSetPropertyBlockColorFromPaletteInspector : ColorFromPaletteBaseInspector {
		public override void OnInspectorGUI() {
			serializedObject.Update();

			var targetProp = serializedObject.FindProperty("_target");

			var nameProp = serializedObject.FindProperty("_name");

			EditorGUILayout.PropertyField(targetProp);

			var renderTarget = targetProp.objectReferenceValue as Renderer;
			if(renderTarget) {
				var mat = renderTarget.sharedMaterial;
				if(mat) {
					string[] names, details;
					int[] inds;

					GetPropertyInfos(mat, out names, out details, out inds);

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
					}
				}
				else
					GUILayout.Label("Renderer target needs a Material.");
			}
			else
				GUILayout.Label("Renderer target not set.");

			OnPaletteInspectorGUI();

			if(serializedObject.ApplyModifiedProperties())
				ApplyColor();
		}

		private void GetPropertyInfos(Material mat, out string[] names, out string[] details, out int[] inds) {
			Shader shader = mat.shader;
			int count = ShaderUtil.GetPropertyCount(shader);

			var _names = new List<string>();
			var _details = new List<string>();

			for(int i = 0; i < count; i++) {
				var isHidden = ShaderUtil.IsShaderPropertyHidden(shader, i);
				if(isHidden)
					continue;

				var type = ShaderUtil.GetPropertyType(shader, i);

				if(type != ShaderUtil.ShaderPropertyType.Color)
					continue;

				var name = ShaderUtil.GetPropertyName(shader, i);				
				var detail = ShaderUtil.GetPropertyDescription(shader, i);

				_names.Add(name);

				if(!string.IsNullOrEmpty(detail))
					_details.Add(detail);
				else
					_details.Add(name);
			}

			count = _names.Count;

			names = new string[count];
			details = new string[count];
			inds = new int[count];

			for(int i = 0; i < count; i++) {
				names[i] = _names[i];
				details[i] = _details[i];
				inds[i] = i;
			}
		}
	}
}
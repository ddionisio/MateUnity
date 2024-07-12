using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using UnityEditorInternal;

namespace M8 {
	[CustomEditor(typeof(SpriteGroupControl))]
	public class SpriteGroupControlInspector : Editor {
		public override void OnInspectorGUI() {
			//targets
			var spriteRenderersAutoFillProp = serializedObject.FindProperty("_spriteRendersAutoFill");
			var spriteRenderersProp = serializedObject.FindProperty("_spriteRenders");
			
			EditorGUILayout.PropertyField(spriteRenderersAutoFillProp);

			if(!spriteRenderersAutoFillProp.boolValue)
				EditorGUILayout.PropertyField(spriteRenderersProp);

			EditorExt.Utility.DrawSeparator();

			//color
			var colorApplyProp = serializedObject.FindProperty("_colorApply");
			var colorProp = serializedObject.FindProperty("_color");

			EditorGUILayout.BeginHorizontal();

			colorApplyProp.boolValue = EditorGUILayout.BeginToggleGroup("Color", colorApplyProp.boolValue);

			colorProp.colorValue = EditorGUILayout.ColorField(colorProp.colorValue);

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			//flip X
			var flipXApplyProp = serializedObject.FindProperty("_flipXApply");
			var flipXProp = serializedObject.FindProperty("_flipX");

			EditorGUILayout.BeginHorizontal();

			flipXApplyProp.boolValue = EditorGUILayout.BeginToggleGroup("Flip X", flipXApplyProp.boolValue);

			flipXProp.boolValue = EditorGUILayout.Toggle(flipXProp.boolValue);

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			//flip Y
			var flipYApplyProp = serializedObject.FindProperty("_flipYApply");
			var flipYProp = serializedObject.FindProperty("_flipY");

			EditorGUILayout.BeginHorizontal();

			flipYApplyProp.boolValue = EditorGUILayout.BeginToggleGroup("Flip Y", flipYApplyProp.boolValue);

			flipYProp.boolValue = EditorGUILayout.Toggle(flipYProp.boolValue);

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			//sort layer
			var layerIDApplyProp = serializedObject.FindProperty("_sortLayerIDApply");
			var layerIDProp = serializedObject.FindProperty("_sortLayerID");

			EditorGUILayout.BeginHorizontal();

			layerIDApplyProp.boolValue = EditorGUILayout.BeginToggleGroup("Sorting Layer", layerIDApplyProp.boolValue);

			var layerNames = GetSortingLayerNames();
			var layerInd = -1;

			for(int i = 0; i < layerNames.Length; i++) {
				if(SortingLayer.NameToID(layerNames[i]) == layerIDProp.intValue) {
					layerInd = i;
					break;
				}
			}

			if(layerInd == -1) {
				for(int i = 0; i < layerNames.Length; i++) {
					if(layerNames[i] == "Default") {
						layerInd = i;
						break;
					}
				}
			}

			layerInd = EditorGUILayout.Popup(layerInd, layerNames);

			layerIDProp.intValue = SortingLayer.NameToID(layerNames[layerInd]);

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			//sort layer order
			var sortOrderApplyProp = serializedObject.FindProperty("_sortOrderApply");
			var sortOrderProp = serializedObject.FindProperty("_sortOrder");

			EditorGUILayout.BeginHorizontal();

			sortOrderApplyProp.boolValue = EditorGUILayout.BeginToggleGroup("Sorting Order", sortOrderApplyProp.boolValue);

			sortOrderProp.intValue = EditorGUILayout.IntField(sortOrderProp.intValue);

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			if(serializedObject.ApplyModifiedProperties()) {
				var tgt = target as SpriteGroupControl;

				if(spriteRenderersAutoFillProp.boolValue)
					tgt.ApplyProperties(tgt.GetComponentsInChildren<SpriteRenderer>(true));
				else
					tgt.ApplyProperties();
			}
		}

		public string[] GetSortingLayerNames() {
			Type internalEditorUtilityType = typeof(InternalEditorUtility);
			PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
			return (string[])sortingLayersProperty.GetValue(null, new object[0]);
		}
	}
}
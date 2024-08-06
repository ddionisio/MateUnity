using DG.DemiEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace M8 {
	[CustomEditor(typeof(SpriteGeometryGrid))]
	public class SpriteGeometryGridInspector : Editor {

		public override void OnInspectorGUI() {
			var sprProp = serializedObject.FindProperty("_sprite");
			var matProp = serializedObject.FindProperty("_material");
			var colCountProp = serializedObject.FindProperty("_columnCount");
			var rowCountProp = serializedObject.FindProperty("_rowCount");
			var ppuOverrideProp = serializedObject.FindProperty("_overridePixelPerUnit");
			var ppuProp = serializedObject.FindProperty("_pixelPerUnit");
			var flipXProp = serializedObject.FindProperty("_flipX");
			var flipYProp = serializedObject.FindProperty("_flipY");
			var gradientModeProp = serializedObject.FindProperty("_gradientMode");
			var gradientOffsetProp = serializedObject.FindProperty("_gradientOffset");
			var gradientsProp = serializedObject.FindProperty("_gradients");
			var clrApplyProp = serializedObject.FindProperty("_colorApply");
			var clrProp = serializedObject.FindProperty("_color");
			var sortLayerProp = serializedObject.FindProperty("_sortingLayer");
			var sortOrderProp = serializedObject.FindProperty("_sortingOrder");

			bool refreshGeometry = false, refreshColor = false, refreshMesh = false, refreshMaterialProp = false;

			//sprite
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(sprProp);

			refreshGeometry = EditorGUI.EndChangeCheck();
			//

			//material
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(matProp);

			refreshMesh = EditorGUI.EndChangeCheck();
			//

			//geometry
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();

			EditorGUIUtility.labelWidth = 60f;

			EditorGUILayout.LabelField("Grid");

			EditorGUIUtility.labelWidth = 60f;

			var colCount = EditorGUILayout.IntField("Column", colCountProp.intValue);
			if(colCount < 1)
				colCount = 1;

			colCountProp.intValue = colCount;

			var rowCount = EditorGUILayout.IntField("Row", rowCountProp.intValue);
			if(rowCount < 1)
				rowCount = 1;

			rowCountProp.intValue = rowCount;

			EditorGUIUtility.labelWidth = 0f;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUIUtility.labelWidth = 60f;

			ppuOverrideProp.boolValue = EditorGUILayout.BeginToggleGroup("Pixel Per Unit", ppuOverrideProp.boolValue);

			EditorGUIUtility.labelWidth = 0f;

			ppuProp.floatValue = EditorGUILayout.FloatField(" ", ppuProp.floatValue);

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			refreshGeometry = refreshGeometry || EditorGUI.EndChangeCheck();
			//

			EditorExt.Utility.DrawSeparator();

			//colors
			EditorGUI.BeginChangeCheck();

			//EditorGUIUtility.labelWidth = 80f;

			var gradientMode = (SpriteGeometryGrid.ColorGradientMode)EditorGUILayout.EnumPopup("Gradient Mode", (SpriteGeometryGrid.ColorGradientMode)gradientModeProp.intValue);

			gradientModeProp.intValue = (int)gradientMode;

			SerializedProperty grad1Prop, grad2Prop;

			switch(gradientMode) {
				case SpriteGeometryGrid.ColorGradientMode.None:
					gradientsProp.arraySize = 0;
					break;

				case SpriteGeometryGrid.ColorGradientMode.Solid:
					gradientsProp.arraySize = 1;

					grad1Prop = gradientsProp.GetArrayElementAtIndex(0);

					var grad1 = grad1Prop.boxedValue as Gradient;

					var solidClr = grad1.Evaluate(0f);

					solidClr = EditorGUILayout.ColorField(" ", solidClr);

					grad1.mode = GradientMode.Fixed;
					grad1.SetKeys(new GradientColorKey[] { new GradientColorKey(solidClr, 0f) }, new GradientAlphaKey[] { new GradientAlphaKey(solidClr.a, 0f) });

					grad1Prop.boxedValue = grad1;
					break;

				case SpriteGeometryGrid.ColorGradientMode.Horizontal:
					gradientsProp.arraySize = 1;

					grad1Prop = gradientsProp.GetArrayElementAtIndex(0);

					EditorGUILayout.PropertyField(grad1Prop, new GUIContent(" "));
					break;

				case SpriteGeometryGrid.ColorGradientMode.Vertical:
					gradientsProp.arraySize = 1;

					grad1Prop = gradientsProp.GetArrayElementAtIndex(0);

					EditorGUILayout.PropertyField(grad1Prop, new GUIContent(" "));
					break;

				case SpriteGeometryGrid.ColorGradientMode.Both:
					gradientsProp.arraySize = 2;

					grad1Prop = gradientsProp.GetArrayElementAtIndex(0);
					grad2Prop = gradientsProp.GetArrayElementAtIndex(1);

					EditorGUILayout.PropertyField(grad1Prop, new GUIContent("Left"));
					EditorGUILayout.PropertyField(grad2Prop, new GUIContent("Right"));
					break;

				case SpriteGeometryGrid.ColorGradientMode.Circular:
					gradientsProp.arraySize = 1;

					grad1Prop = gradientsProp.GetArrayElementAtIndex(0);

					EditorGUILayout.PropertyField(grad1Prop, new GUIContent(" "));
					break;
			}

			if(!(gradientMode == SpriteGeometryGrid.ColorGradientMode.None || gradientMode == SpriteGeometryGrid.ColorGradientMode.Solid)) {
				var ofs = gradientOffsetProp.vector2Value;

				if(gradientMode == SpriteGeometryGrid.ColorGradientMode.Horizontal || gradientMode == SpriteGeometryGrid.ColorGradientMode.Circular || gradientMode == SpriteGeometryGrid.ColorGradientMode.Both) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Gradient Offset");

					ofs.x = EditorGUILayout.Slider(ofs.x, -1f, 1f);

					EditorGUILayout.EndHorizontal();
				}
								
				if(gradientMode == SpriteGeometryGrid.ColorGradientMode.Vertical || gradientMode == SpriteGeometryGrid.ColorGradientMode.Circular || gradientMode == SpriteGeometryGrid.ColorGradientMode.Both) {
					EditorGUILayout.BeginHorizontal();

					if(gradientMode == SpriteGeometryGrid.ColorGradientMode.Vertical)
						EditorGUILayout.LabelField("Gradient Offset");
					else
						EditorGUILayout.LabelField("");

					ofs.y = EditorGUILayout.Slider(ofs.y, -1f, 1f);

					EditorGUILayout.EndHorizontal();
				}

				gradientOffsetProp.vector2Value = ofs;
			}

			//EditorGUIUtility.labelWidth = 0f;

			refreshColor = EditorGUI.EndChangeCheck();
			//

			EditorExt.Utility.DrawSeparator();

			//material properties
			EditorGUI.BeginChangeCheck();

			//color
			EditorGUILayout.BeginHorizontal();

			EditorGUIUtility.labelWidth = 50f;

			clrApplyProp.boolValue = EditorGUILayout.BeginToggleGroup("Color", clrApplyProp.boolValue);
						
			clrProp.colorValue = EditorGUILayout.ColorField(" ", clrProp.colorValue);

			EditorGUIUtility.labelWidth = 0f;

			EditorGUILayout.EndToggleGroup();

			EditorGUILayout.EndHorizontal();

			//flip
			EditorGUILayout.BeginHorizontal();

			EditorGUIUtility.labelWidth = 50f;

			EditorGUILayout.LabelField("Flip");

			EditorGUIUtility.labelWidth = 20f;

			flipXProp.boolValue = EditorGUILayout.Toggle("X", flipXProp.boolValue, GUILayout.MaxWidth(60f));

			flipYProp.boolValue = EditorGUILayout.Toggle("Y", flipYProp.boolValue, GUILayout.MaxWidth(60f));

			EditorGUIUtility.labelWidth = 0f;

			EditorGUILayout.Space();

			EditorGUILayout.EndHorizontal();

			refreshMaterialProp = EditorGUI.EndChangeCheck();
			//

			EditorExt.Utility.DrawSeparator();

			//mesh properties
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.PropertyField(sortLayerProp);
			EditorGUILayout.PropertyField(sortOrderProp);

			refreshMesh = refreshMesh || EditorGUI.EndChangeCheck();
			//

			EditorExt.Utility.DrawSeparator();

			//edit
			if(GUILayout.Button("Refresh"))
				refreshGeometry = true;
			//

			//refresh
			serializedObject.ApplyModifiedProperties();

			var tgt = target as SpriteGeometryGrid;

			if(!tgt.isInit || refreshGeometry)
				tgt.Init();
			else {
				if(refreshColor)
					tgt.RefreshVertexColors();

				if(refreshMesh)
					tgt.RefreshMeshRenderer();

				if(refreshMaterialProp)
					tgt.RefreshMaterialProperty();
			}
		}
	}
}
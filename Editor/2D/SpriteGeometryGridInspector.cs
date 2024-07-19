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
			var clrsProp = serializedObject.FindProperty("_colors");
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

			SerializedProperty clr1, clr2, clr3, clr4;

			switch(gradientMode) {
				case SpriteGeometryGrid.ColorGradientMode.None:
					clrsProp.arraySize = 1;
					break;

				case SpriteGeometryGrid.ColorGradientMode.Solid:
					clrsProp.arraySize = 2;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");

					clr1 = clrsProp.GetArrayElementAtIndex(1);
					clr1.colorValue = EditorGUILayout.ColorField(clr1.colorValue);

					EditorGUILayout.EndHorizontal();
					break;

				case SpriteGeometryGrid.ColorGradientMode.Horizontal:
					clrsProp.arraySize = 3;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");

					clr1 = clrsProp.GetArrayElementAtIndex(1);
					clr1.colorValue = EditorGUILayout.ColorField(clr1.colorValue);

					clr2 = clrsProp.GetArrayElementAtIndex(2);
					clr2.colorValue = EditorGUILayout.ColorField(clr2.colorValue);

					EditorGUILayout.EndHorizontal();
					break;

				case SpriteGeometryGrid.ColorGradientMode.Vertical:
					clrsProp.arraySize = 3;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");

					clr1 = clrsProp.GetArrayElementAtIndex(1);
					clr1.colorValue = EditorGUILayout.ColorField(clr1.colorValue);

					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");

					clr2 = clrsProp.GetArrayElementAtIndex(2);
					clr2.colorValue = EditorGUILayout.ColorField(clr2.colorValue);

					EditorGUILayout.EndHorizontal();
					break;

				case SpriteGeometryGrid.ColorGradientMode.Both:
					clrsProp.arraySize = 5;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");

					clr1 = clrsProp.GetArrayElementAtIndex(1);
					clr1.colorValue = EditorGUILayout.ColorField(clr1.colorValue);

					clr3 = clrsProp.GetArrayElementAtIndex(3);
					clr3.colorValue = EditorGUILayout.ColorField(clr3.colorValue);

					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("");

					clr2 = clrsProp.GetArrayElementAtIndex(2);
					clr2.colorValue = EditorGUILayout.ColorField(clr2.colorValue);

					clr4 = clrsProp.GetArrayElementAtIndex(4);
					clr4.colorValue = EditorGUILayout.ColorField(clr4.colorValue);

					EditorGUILayout.EndHorizontal();
					break;
			}

			if(!(gradientMode == SpriteGeometryGrid.ColorGradientMode.None || gradientMode == SpriteGeometryGrid.ColorGradientMode.Solid)) {
				var ofs = gradientOffsetProp.vector2Value;

				if(gradientMode == SpriteGeometryGrid.ColorGradientMode.Horizontal || gradientMode == SpriteGeometryGrid.ColorGradientMode.Both) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Gradient Offset");

					ofs.x = EditorGUILayout.Slider(ofs.x, -1f, 1f);

					EditorGUILayout.EndHorizontal();
				}
								
				if(gradientMode == SpriteGeometryGrid.ColorGradientMode.Vertical || gradientMode == SpriteGeometryGrid.ColorGradientMode.Both) {
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
			var clrProp = clrsProp.GetArrayElementAtIndex(0);

			clrProp.colorValue = EditorGUILayout.ColorField("Color", clrProp.colorValue);

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
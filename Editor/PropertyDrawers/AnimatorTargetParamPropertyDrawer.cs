using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

namespace M8 {
	[CustomPropertyDrawer(typeof(AnimatorTargetParam), true)]
	public class AnimatorTargetParamPropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			//target
			var targetProp = property.FindPropertyRelative("_target");

			var targetPos = targetProp.objectReferenceValue ? new Rect(position.x, position.y, position.width * 0.5f, position.height) : position;

			targetProp.objectReferenceValue = EditorGUI.ObjectField(targetPos, targetProp.objectReferenceValue, typeof(Animator), true);

			//param
			var animator = targetProp.objectReferenceValue as Animator;
			if(animator) {
				var animCtrl = AnimatorUtil.GetAnimatorController(animator);
				if(animCtrl) {
					var paramPos = new Rect(position.x + position.width * 0.5f + 4f, position.y, position.width * 0.5f - 4f, position.height);

					var paramType = ((AnimatorTargetParam)property.boxedValue).paramType;

					var animParms = animCtrl.parameters;

					var parms = new List<AnimatorControllerParameter>(animParms.Length);

					for(int i = 0; i < animParms.Length; i++) {
						var animParm = animParms[i];
						if(animParm.type == paramType)
							parms.Add(animParm);
					}

					if(parms.Count > 0) {
						var nameIDProp = property.FindPropertyRelative("_nameID");

						var curInd = 0;

						var parmNames = new string[parms.Count];

						for(int i = 0; i < parms.Count; i++) {
							var parm = parms[i];

							parmNames[i] = parm.name;

							if(nameIDProp.intValue == parm.nameHash)
								curInd = i;
						}

						var selectInd = EditorGUI.Popup(paramPos, curInd, parmNames);

						nameIDProp.intValue = parms[selectInd].nameHash;
					}
					else
						EditorGUILayout.HelpBox(string.Format("No parameters of type {0} found.", paramType.ToString()), MessageType.Warning);
				}
				else
					EditorGUILayout.HelpBox("Animator Controller is not assigned.", MessageType.Warning);
			}
			
			EditorGUI.EndProperty();
		}
	}
}
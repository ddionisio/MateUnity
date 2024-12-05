#if !M8_UNITY_ANIMATOR_DISABLED
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CustomPropertyDrawer(typeof(AnimatorParam), true)]
	public class AnimatorParamPropertyDrawer : PropertyDrawer {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			var target = (MonoBehaviour)property.serializedObject.targetObject;
			var animator = target.GetComponent<UnityEngine.Animator>();

			if(animator) {
				var animCtrl = AnimatorEditorUtil.GetAnimatorController(animator);
				if(animCtrl) {
					var paramType = ((AnimatorParam)property.boxedValue).paramType;

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

						var selectInd = EditorGUI.Popup(position, curInd, parmNames);

						nameIDProp.intValue = parms[selectInd].nameHash;
					}
					else
						EditorGUILayout.HelpBox(string.Format("No parameters of type {0} found.", paramType.ToString()), MessageType.Warning);
				}
				else
					EditorGUILayout.HelpBox("Animator Controller is not assigned.", MessageType.Warning);
			}
			else
				EditorGUILayout.HelpBox("Animator is missing.", MessageType.Warning);

			EditorGUI.EndProperty();
		}
	}
}
#endif
#if !M8_UNITY_ANIMATOR_DISABLED
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
	[CustomEditor(typeof(AnimatorParamProxy))]
	public class AnimatorParamProxyInspector : Editor {
		public override void OnInspectorGUI() {
			var targetField = serializedObject.FindProperty("_target");
			
			EditorGUILayout.ObjectField(targetField, new GUIContent("Target"));

			var animator = targetField.objectReferenceValue as UnityEngine.Animator;
			if(animator) {
				var animCtrl = AnimatorEditorUtil.GetAnimatorController(animator);
				if(animCtrl) {
					var paramTypeField = serializedObject.FindProperty("_paramType");

					var paramType = (AnimatorControllerParameterType)EditorGUILayout.EnumPopup("Type", (AnimatorControllerParameterType)paramTypeField.intValue);

					paramTypeField.intValue = (int)paramType;

					var animParms = animCtrl.parameters;

					var parms = new List<AnimatorControllerParameter>(animParms.Length);

					for(int i = 0; i < animParms.Length; i++) {
						var animParm = animParms[i];
						if(animParm.type == paramType)
							parms.Add(animParm);
					}

					if(parms.Count > 0) {
						var paramField = serializedObject.FindProperty("_paramID");

						var curInd = 0;

						var parmNames = new string[parms.Count];

						for(int i = 0; i < parms.Count; i++) {
							var parm = parms[i];

							parmNames[i] = parm.name;

							if(paramField.intValue == parm.nameHash)
								curInd = i;
						}

						var selectInd = EditorGUILayout.Popup(new GUIContent("ID"), curInd, parmNames);

						paramField.intValue = parms[selectInd].nameHash;
					}
					else
						EditorGUILayout.HelpBox(string.Format("No parameters of type {0} found.", paramType.ToString()), MessageType.Warning);
				}
				else
					EditorGUILayout.HelpBox("Animator Controller is not assigned.", MessageType.Warning);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif
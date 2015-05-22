using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.UI {
    [CustomEditor(typeof(InputModule))]
    public class UIInputModuleInspector : Editor {
        public override void OnInspectorGUI() {
            GUI.changed = false;

            InputModule obj = target as InputModule;

            obj.playerIndex = EditorGUILayout.IntField("Player Index", obj.playerIndex);

            obj.horizontalAxis = EditorExt.InputBinder.GUISelectInputAction("Horizontal Axis", obj.horizontalAxis);
            obj.verticalAxis = EditorExt.InputBinder.GUISelectInputAction("Vertical Axis", obj.verticalAxis);
            obj.submitButton = EditorExt.InputBinder.GUISelectInputAction("Submit Button", obj.submitButton);
            obj.cancelButton = EditorExt.InputBinder.GUISelectInputAction("Cancel Button", obj.cancelButton);

            obj.inputActionsPerSecond = EditorGUILayout.FloatField("Input Actions Per Second", obj.inputActionsPerSecond);

            obj.allowActivationOnMobileDevice = EditorGUILayout.Toggle("Allow Activation On Mobile Device", obj.allowActivationOnMobileDevice);

            if(GUI.changed)
                EditorUtility.SetDirty(obj);
        }
    }
}
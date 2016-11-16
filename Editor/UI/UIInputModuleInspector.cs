using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8.UI {
    [CustomEditor(typeof(InputModule))]
    public class UIInputModuleInspector : Editor {
        public override void OnInspectorGUI() {
            InputModule obj = target as InputModule;

            EditorGUI.BeginChangeCheck();

            var playerIndex = EditorGUILayout.IntField("Player Index", obj.playerIndex);

            var horizontalAxis = EditorExt.InputBinder.GUISelectInputAction("Horizontal Axis", obj.horizontalAxis);
            var verticalAxis = EditorExt.InputBinder.GUISelectInputAction("Vertical Axis", obj.verticalAxis);
            var submitButton = EditorExt.InputBinder.GUISelectInputAction("Submit Button", obj.submitButton);
            var cancelButton = EditorExt.InputBinder.GUISelectInputAction("Cancel Button", obj.cancelButton);

            var inputActionsPerSecond = EditorGUILayout.FloatField("Input Actions Per Second", obj.inputActionsPerSecond);

            var allowActivationOnMobileDevice = EditorGUILayout.Toggle("Allow Activation On Mobile Device", obj.allowActivationOnMobileDevice);

            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Change Input Module");

                obj.playerIndex = playerIndex;

                obj.horizontalAxis = horizontalAxis;
                obj.verticalAxis = verticalAxis;
                obj.submitButton = submitButton;
                obj.cancelButton = cancelButton;

                obj.inputActionsPerSecond = inputActionsPerSecond;

                obj.allowActivationOnMobileDevice = allowActivationOnMobileDevice;
            }
        }
    }
}
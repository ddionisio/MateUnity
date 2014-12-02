using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(UIInputModule))]
    public class UIInputModuleInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            GUI.changed = false;

            UIInputModule obj = target as UIInputModule;

            obj.playerIndex = EditorGUILayout.IntField("Player Index", obj.playerIndex);

            obj.horizontalAxis = Editor.InputBinder.GUISelectInputAction("Horizontal Axis", obj.horizontalAxis);
            obj.verticalAxis = Editor.InputBinder.GUISelectInputAction("Vertical Axis", obj.verticalAxis);
            obj.submitButton = Editor.InputBinder.GUISelectInputAction("Submit Button", obj.submitButton);
            obj.cancelButton = Editor.InputBinder.GUISelectInputAction("Cancel Button", obj.cancelButton);

            obj.inputActionsPerSecond = EditorGUILayout.FloatField("Input Actions Per Second", obj.inputActionsPerSecond);

            obj.allowActivationOnMobileDevice = EditorGUILayout.Toggle("Allow Activation On Mobile Device", obj.allowActivationOnMobileDevice);

            if(GUI.changed)
                EditorUtility.SetDirty(obj);
        }
    }
}
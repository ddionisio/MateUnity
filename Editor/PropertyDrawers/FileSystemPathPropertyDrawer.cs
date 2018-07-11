using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CustomPropertyDrawer(typeof(FileSystemPathAttribute))]
    public class FileSystemPathPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var fileSystemPathAttr = this.attribute as FileSystemPathAttribute;

            const float dialogButtonWidth = 25f;
            const float space = 4f;

            var textFieldPos = position; textFieldPos.width -= dialogButtonWidth + space;
            property.stringValue = EditorGUI.TextField(textFieldPos, property.stringValue);

            var dialogButtonPos = new Rect(textFieldPos.max.x + space, textFieldPos.min.y, dialogButtonWidth, position.height);
            if(GUI.Button(dialogButtonPos, "...")) {
                var path = property.stringValue;
                if(string.IsNullOrEmpty(path))
                    path = Application.dataPath;

                string folder = System.IO.Path.GetDirectoryName(path);

                property.stringValue = EditorUtility.OpenFilePanel(fileSystemPathAttr.fileDialogTitle, folder, fileSystemPathAttr.fileExtension);
            }
            

            EditorGUI.EndProperty();
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace M8 {
    [CustomPropertyDrawer(typeof(SceneAssetPath))]
    public class SceneAssetPathPropertyDrawer : PropertyDrawer {
                
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var propName = property.FindPropertyRelative("name");
            var propPath = property.FindPropertyRelative("path");

            var scenes = EditorBuildSettings.scenes;
                        
            var names = new string[scenes.Length + 1];
            var paths = new string[scenes.Length + 1];

            names[0] = "None";
            paths[0] = "";

            for(int i = 0; i < scenes.Length; i++) {
                names[i+1] = SceneAssetPath.LoadableName(scenes[i].path);
                paths[i+1] = scenes[i].path;
            }

            var ind = System.Array.IndexOf(paths, propPath.stringValue);
            if(ind == -1)
                ind = 0;
            
            var newInd = EditorGUI.Popup(position, ind, names);

            if(newInd != ind) {
                propName.stringValue = newInd > 0 ? names[newInd] : "";
                propPath.stringValue = paths[newInd];
            }

            EditorGUI.EndProperty();
        }
    }
}
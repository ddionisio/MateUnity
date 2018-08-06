using UnityEditor;

namespace M8.UI.Widgets {
    [CustomEditor(typeof(Invisible))]
    public class InvisibleInspector : Editor {
        public override void OnInspectorGUI() {
            var dat = target as Invisible;

            var raycastTarget = EditorGUILayout.Toggle("Raycast Target", dat.raycastTarget);
            if(dat.raycastTarget != raycastTarget) {
                Undo.RecordObject(dat, "Change Raycast Target");
                dat.raycastTarget = raycastTarget;
            }
        }
    }
}
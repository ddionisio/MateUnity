using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(GOActiveBySceneStateFlag))]
    public class GOActiveBySceneStateFlagInspector : Editor {
        private string[] mMasks = null;

        public override void OnInspectorGUI() {
            if(mMasks == null)
                mMasks = M8.EditorExt.Utility.GenerateGenericMaskString();

            EditorGUI.BeginChangeCheck();

            GOActiveBySceneStateFlag input = target as GOActiveBySceneStateFlag;
            var newFlag = EditorGUILayout.TextField("Flag Name", input.flag);
            var newFlagMask = EditorGUILayout.MaskField("Flag Bits", (int)input.flagMask, mMasks);

            base.OnInspectorGUI();

            if(EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Change Flags");

                input.flag = newFlag;
                input.flagMask = (uint)newFlagMask;
            }
        }
    }
}
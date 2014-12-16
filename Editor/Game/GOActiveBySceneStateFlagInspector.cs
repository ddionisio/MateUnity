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

            GUI.changed = false;

            GOActiveBySceneStateFlag input = target as GOActiveBySceneStateFlag;
            input.flag = EditorGUILayout.TextField("Flag Name", input.flag);
            input.flagBit = EditorGUILayout.MaskField("Flag Bits", input.flagBit, mMasks);

            base.OnInspectorGUI();

            if(GUI.changed)
                EditorUtility.SetDirty(target);
        }
    }
}
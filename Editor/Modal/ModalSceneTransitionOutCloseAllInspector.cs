using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8 {
    [CustomEditor(typeof(ModalSceneTransitionOutCloseAll))]
    public class ModalSceneTransitionOutCloseAllInspector : Editor {
        public override void OnInspectorGUI() {
            var modalTrans = (ModalSceneTransitionOutCloseAll)target;

            var useMain = modalTrans.useMain;
            var modalManager = modalTrans.modalManager;

            useMain = EditorGUILayout.Toggle("Use Main", useMain);

            if(!useMain) {
                modalManager = (ModalManager)EditorGUILayout.ObjectField("Modal Manager", modalManager, typeof(ModalManager), true);
            }
            else
                modalManager = null;

            if(modalTrans.useMain != useMain || modalTrans.modalManager != modalManager) {
                Undo.RecordObject(target, "Change ModalSceneTransitionOutCloseAll");

                modalTrans.useMain = useMain;
                modalTrans.modalManager = modalManager;
            }
        }
    }
}
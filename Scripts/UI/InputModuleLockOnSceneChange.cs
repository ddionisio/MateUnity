using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8.UI {
    [AddComponentMenu("M8/UI/InputModuleLockOnSceneChange ")]
    public class InputModuleLockOnSceneChange : MonoBehaviour {
        private bool mIsLocked;

        void OnDestroy() {
            if(SceneManager.instance) {
                SceneManager.instance.sceneChangeStartCallback -= OnSceneChangeStart;
                SceneManager.instance.sceneChangeEndCallback -= OnSceneChangeEnd;
            }

            if(mIsLocked && InputModule.instance)
                InputModule.instance.lockInput = false;
        }

        void Awake() {
            SceneManager.instance.sceneChangeStartCallback += OnSceneChangeStart;
            SceneManager.instance.sceneChangeEndCallback += OnSceneChangeEnd;
        }

        void OnSceneChangeStart() {
            mIsLocked = true;
            InputModule.instance.lockInput = true;
        }

        void OnSceneChangeEnd() {
            mIsLocked = false;
            InputModule.instance.lockInput = false;
        }
    }
}
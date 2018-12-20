using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Close all modals in ModalManager.main during scene manager transition out
    /// </summary>
    [AddComponentMenu("M8/Modal/Helpers/Scene Transition Out Close All")]
    public class ModalSceneTransitionOutCloseAll : MonoBehaviour, SceneManager.ITransition {
        int SceneManager.ITransition.priority {
            get {
                return 1000; //make sure we close first before fullscreen transition
            }
        }

        void OnDestroy() {
            if(SceneManager.isInstantiated)
                SceneManager.instance.RemoveTransition(this);
        }

        void Awake() {
            SceneManager.instance.AddTransition(this);
        }

        IEnumerator SceneManager.ITransition.In() {
            yield return null;
        }

        IEnumerator SceneManager.ITransition.Out() {
            ModalManager.main.CloseAll();

            while(ModalManager.main.isBusy)
                yield return null;
        }
    }
}
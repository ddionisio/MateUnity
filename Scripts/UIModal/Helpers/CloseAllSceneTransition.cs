using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8.UI.Modal.Helpers {
    [AddComponentMenu("M8/UI Modal/Controller/CloseAllSceneTransition")]
    public class CloseAllSceneTransition : MonoBehaviour, SceneManager.ITransition {
        int SceneManager.ITransition.priority {
            get {
                return 1000; //make sure we close first before fullscreen transition
            }
        }

        void OnDestroy() {
            if(SceneManager.instance)
                SceneManager.instance.RemoveTransition(this);
        }

        void Awake() {
            SceneManager.instance.AddTransition(this);
        }

        IEnumerator SceneManager.ITransition.In() {
            UIModal.Manager.instance.ModalCloseAll();

            while(UIModal.Manager.instance.isBusy)
                yield return null;
        }

        IEnumerator SceneManager.ITransition.Out() {
            yield return null;
        }
    }
}
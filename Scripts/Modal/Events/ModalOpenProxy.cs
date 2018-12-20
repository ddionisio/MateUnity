using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Open a modal from ModalManager.main
    /// </summary>
    [AddComponentMenu("M8/Modal/Events/Open")]
    public class ModalOpenProxy : MonoBehaviour {
        public string modal;
        public bool closeIfOpened;

        public void Invoke() {
            if(!ModalManager.main)
                return;

            if(ModalManager.main.IsInStack(modal)) {
                if(closeIfOpened)
                    ModalManager.main.CloseUpTo(modal, true);
            }
            else
                ModalManager.main.Open(modal);
        }
    }
}
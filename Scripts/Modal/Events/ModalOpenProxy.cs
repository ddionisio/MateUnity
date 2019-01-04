using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Open a modal from ModalManager
    /// </summary>
    [AddComponentMenu("M8/Modal/Events/Open")]
    public class ModalOpenProxy : ModalManagerControlBase {
        public string modal;
        public bool closeIfOpened;

        public void Invoke() {
            var mgr = modalManager;

            if(!mgr)
                return;

            if(mgr.IsInStack(modal)) {
                if(closeIfOpened)
                    mgr.CloseUpTo(modal, true);
            }
            else
                mgr.Open(modal);
        }
    }
}
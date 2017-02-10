using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8.UI.Modal.Helpers {
    /// <summary>
    /// Simple behaviour to open a modal via Execute, useful for UI, or some timeline editor
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Helpers/OpenModal")]
    public class OpenModal : MonoBehaviour {
        public string modalRef;
        public bool closeIfOpened;

        public void Execute() {
            if(UIModal.Manager.instance.isBusy || SceneManager.instance.isLoading)
                return;

            if(UIModal.Manager.instance.ModalIsInStack(modalRef)) {
                if(closeIfOpened)
                    UIModal.Manager.instance.ModalCloseUpTo(modalRef, true);
            }
            else
                UIModal.Manager.instance.ModalOpen(modalRef);
        }
    }
}
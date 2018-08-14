using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8.UIModal.Helpers {
    /// <summary>
    /// Simple behaviour to open a modal via Execute, useful for UI, or some timeline editor
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Helpers/OpenModal")]
    public class OpenModal : MonoBehaviour {
        public string modalRef;
        public bool closeIfOpened;

        public void Execute() {
            if(Manager.instance.isBusy || SceneManager.instance.isLoading)
                return;

            if(Manager.instance.ModalIsInStack(modalRef)) {
                if(closeIfOpened)
                    Manager.instance.ModalCloseUpTo(modalRef, true);
            }
            else
                Manager.instance.ModalOpen(modalRef);
        }
    }
}
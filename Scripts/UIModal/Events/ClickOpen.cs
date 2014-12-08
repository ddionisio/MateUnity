using UnityEngine;
using System.Collections;

namespace M8.UIModal.Events {
    [AddComponentMenu("M8/UI Modal/Events/ClickOpen")]
    public class ClickOpen : MonoBehaviour {
        public string modal;

        void OnClick() {
            if(!Manager.instance.ModalIsInStack(modal))
                Manager.instance.ModalOpen(modal);
        }
    }
}
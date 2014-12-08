using UnityEngine;
using System.Collections;

namespace M8.UIModal.Events {
    [AddComponentMenu("M8/UI Modal/Events/ClickCloseTop")]
    public class ClickCloseTop : MonoBehaviour {
        void OnClick() {
            Manager.instance.ModalCloseTop();
        }
    }
}
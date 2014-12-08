using UnityEngine;
using System.Collections;

namespace M8.UIModal.Input {
    /// <summary>
    /// Pop modal stack if given input is pressed. Put this component on a modal dialog game object that has a UIController.
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Input/CloseTop")]
    public class CloseTop : MonoBehaviour {
        public int player = 0;
        public int escape = 0;

        private Controller mController;

        void OnDestroy() {
            if(mController != null) {
                mController.onActiveCallback -= UIActive;
            }
        }

        void Awake() {
            mController = GetComponent<Controller>();
            if(mController != null) {
                mController.onActiveCallback += UIActive;
            }
        }

        void OnInputEscape(InputManager.Info data) {
            if(data.state == InputManager.State.Pressed) {
                Manager.instance.ModalCloseTop();
            }
        }

        void UIActive(bool active) {
            InputManager input = InputManager.instance;

            if(active) {
                input.AddButtonCall(player, escape, OnInputEscape);
            }
            else {
                input.RemoveButtonCall(player, escape, OnInputEscape);
            }
        }
    }
}
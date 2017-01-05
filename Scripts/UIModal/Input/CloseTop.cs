using UnityEngine;
using System.Collections;
using M8.UIModal.Interface;
using System;

namespace M8.UIModal.Input {
    /// <summary>
    /// Pop modal stack if given input is pressed. Put this component on a modal dialog game object that has a UIController.
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Input/CloseTop")]
    [RequireComponent(typeof(Controller))]
    public class CloseTop : MonoBehaviour, IActive {
        public int player = 0;
        public int escape = 0;

        public void Execute() {
            Manager.instance.ModalCloseTop();
        }
        
        void OnInputEscape(InputManager.Info data) {
            if(data.state == InputManager.State.Pressed) {
                Execute();
            }
        }
        
        void IActive.SetActive(bool aActive) {
            InputManager input = InputManager.instance;

            if(aActive) {
                input.AddButtonCall(player, escape, OnInputEscape);
            }
            else {
                input.RemoveButtonCall(player, escape, OnInputEscape);
            }
        }
    }
}
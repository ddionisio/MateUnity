using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Clop top or all from ModalManager.main
    /// </summary>
    [AddComponentMenu("M8/Modal/Events/Close")]
    public class ModalCloseProxy : MonoBehaviour {
        public enum Mode {
            Top,
            All
        }

        public Mode mode = Mode.Top;

        public void Invoke() {
            if(!ModalManager.main)
                return;

            switch(mode) {
                case Mode.Top:
                    ModalManager.main.CloseTop();
                    break;
                case Mode.All:
                    ModalManager.main.CloseAll();
                    break;
            }
        }
    }
}
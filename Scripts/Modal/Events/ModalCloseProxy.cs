using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Clop top or all from ModalManager
    /// </summary>
    [AddComponentMenu("M8/Modal/Events/Close")]
    public class ModalCloseProxy : ModalManagerControlBase {
        public enum Mode {
            Top,
            All
        }

        public Mode mode = Mode.Top;

        public void Invoke() {
            var mgr = modalManager;

            if(!mgr)
                return;

            switch(mode) {
                case Mode.Top:
                    mgr.CloseTop();
                    break;
                case Mode.All:
                    mgr.CloseAll();
                    break;
            }
        }
    }
}
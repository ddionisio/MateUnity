using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [System.Serializable]
    public struct ModalManagerPath {
        public enum Mode {
            Main,
            Reference
        }

        public Mode mode;
        public ModalManager managerRef;

        public ModalManager manager {
            get {
                switch(mode) {
                    case Mode.Main:
                        return ModalManager.main;
                    case Mode.Reference:
                        return managerRef;
                    default:
                        return null;
                }
            }
        }
    }
}
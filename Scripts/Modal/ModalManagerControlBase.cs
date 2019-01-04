using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public abstract class ModalManagerControlBase : MonoBehaviour {
        [SerializeField]
        [HideInInspector]
        bool _modalMgrUseMain = false;

        [SerializeField]
        [HideInInspector]
        ModalManager _modalMgr = null;

        public ModalManager modalManager {
            get {
                return _modalMgrUseMain ? ModalManager.main : _modalMgr;
            }
        }
    }
}
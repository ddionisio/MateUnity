using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Open a modal from ModalManager
    /// </summary>
    [AddComponentMenu("M8/Modal/Events/Open")]
    public class ModalOpenProxy : MonoBehaviour {
        public ModalManagerPath modalManager;
        public string modal;
        [Tooltip("If true, only open if the stack is empty (no other modals opened)")]
        public bool onlyIfEmpty;
        public bool closeIfOpened;

        private Coroutine mRout;

        public void Invoke() {
            if(!gameObject.activeInHierarchy)
                return;

			var mgr = modalManager.manager;
            if(!mgr)
                return;

			if(onlyIfEmpty && (mgr.isBusy || mgr.activeCount > 0))
				return;

			if(mRout != null)
                return;

            mRout = StartCoroutine(DoOpen());
        }

		void OnDisable() {
			if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
			}
		}

		IEnumerator DoOpen() {
            var mgr = modalManager.manager;

			if(mgr.IsInStack(modal)) {
				if(closeIfOpened)
					mgr.CloseUpTo(modal, true);
			}
			else
				mgr.Open(modal);

            do {
                yield return null;
            } while(mgr.isBusy);

            mRout = null;
		}
    }
}
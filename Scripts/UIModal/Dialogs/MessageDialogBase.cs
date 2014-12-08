using UnityEngine;
using System.Collections;

namespace M8.UIModal.Dialogs {
    [AddComponentMenu("")]
    public abstract class MessageDialogBase : Controller {
        public delegate void OnClick();

        private OnClick mCallback = null;

        public static void Open(string modalName, string aTitle, string aText, OnClick aCallback) {
            Manager uiMgr = Manager.instance;

            if(uiMgr.ModalGetTop() == modalName)
                uiMgr.ModalCloseTop();

            Manager.UIData dat = uiMgr.ModalGetData(modalName);

            if(dat != null) {
                MessageDialogBase uiConfirm = dat.ui as MessageDialogBase;

                uiConfirm.OnSetInfo(aTitle, aText);
                uiConfirm.mCallback = aCallback;

                uiMgr.ModalOpen(modalName);
            }
        }

        protected abstract void OnSetInfo(string aTitle, string aText);

        /// <summary>
        /// Call this when closing the message
        /// </summary>
        protected void Click() {
            OnClick toCall = mCallback;

            Manager.instance.ModalCloseTop();

            if(toCall != null)
                toCall();
        }
    }
}
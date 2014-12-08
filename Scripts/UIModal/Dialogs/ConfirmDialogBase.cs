using UnityEngine;
using System.Collections;

namespace M8.UIModal.Dialogs {
    [AddComponentMenu("")]
    public abstract class ConfirmDialogBase : Controller {
        public delegate void OnConfirm(bool yes);

        private OnConfirm mCallback = null;

        public static void Open(string modalName, string aTitle, string aText, OnConfirm aCallback) {
            Manager uiMgr = Manager.instance;

            if(uiMgr.ModalGetTop() == modalName)
                uiMgr.ModalCloseTop();

            Manager.UIData dat = uiMgr.ModalGetData(modalName);

            if(dat != null) {
                ConfirmDialogBase uiConfirm = dat.ui as ConfirmDialogBase;
                uiConfirm.OnSetInfo(aTitle, aText);
                uiConfirm.mCallback = aCallback;

                uiMgr.ModalOpen(modalName);
            }
            else if(aCallback != null) //TODO: use default param?
                aCallback(true);
        }

        protected abstract void OnSetInfo(string aTitle, string aText);

        /// <summary>
        /// Call this when closing the message
        /// </summary>
        protected void Click(bool yes) {
            OnConfirm toCall = mCallback;

            Manager.instance.ModalCloseTop();

            if(toCall != null)
                toCall(yes);
        }
    }
}
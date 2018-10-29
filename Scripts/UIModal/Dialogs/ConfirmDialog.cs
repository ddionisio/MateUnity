using UnityEngine;
using UnityEngine.UI;

namespace M8.UIModal.Dialogs {
    [AddComponentMenu("M8/UI Modal/Confirm Dialog")]
    public class ConfirmDialog : Controller, Interface.IPush, Interface.IPop {
        public const string parmTitle = "confirmDialog_t";
        public const string parmText = "confirmDialog_txt"; 
        public const string parmCallback = "confirmDialog_cb";

        public delegate void OnConfirm(bool yes);

        [Header("Display")]
        public Text titleLabel;
        public Text descLabel;

        private OnConfirm mCallback = null;

        private static GenericParams mParms;

        public static void Open(string modalName, string aTitle, string aText, OnConfirm aCallback) {
            if(mParms == null)
                mParms = new GenericParams();

            mParms[parmTitle] = aTitle;
            mParms[parmText] = aText;
            mParms[parmCallback] = aCallback;

            Manager.instance.ModalOpen(modalName, mParms);
        }

        public void Invoke(bool confirm) {
            var toCall = mCallback;

            Close();

            if(toCall != null)
                toCall(confirm);
        }

        void Interface.IPush.Push(GenericParams parms) {
            if(parms != null) {
                if(titleLabel) {
                    object obj;
                    if(parms.TryGetValue(parmTitle, out obj))
                        titleLabel.text = obj.ToString();
                    else
                        titleLabel.text = "";
                }

                if(descLabel) {
                    object obj;
                    if(parms.TryGetValue(parmText, out obj))
                        descLabel.text = obj.ToString();
                    else
                        descLabel.text = "";
                }

                mCallback = parms.GetValue<OnConfirm>(parmCallback);
            }
        }

        void Interface.IPop.Pop() {
            mCallback = null;
        }
    }
}
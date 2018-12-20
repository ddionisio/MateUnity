using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace M8 {
    [AddComponentMenu("")]
    public abstract class ModalDialog : ModalController, IModalPush, IModalPop {
        //use these during Push to populate
        public const string paramTitle = "title";
        public const string paramDesc = "description";        
        public const string paramIconRef = "icon";
        public const string paramCallback = "callback";

        private System.Action<int> mChoiceCallback;

        /// <summary>
        /// Call this to open a dialog from ModalManager.main
        /// </summary>
        public static void Open(string modal, string title, string description, object icon, System.Action<int> callback) {
            var mgr = ModalManager.main;
            if(!mgr)
                return;

            var dlg = mgr.GetBehaviour<ModalDialog>(modal);
            if(dlg) {
                dlg.Setup(title, description, icon);
                dlg.mChoiceCallback = callback;
            }
            else {
                var parms = new GenericParams();
                parms[paramTitle] = title;
                parms[paramDesc] = description;
                parms[paramIconRef] = icon;
                parms[paramCallback] = callback;

                mgr.Open(modal, parms);
            }
        }
        
        /// <summary>
        /// Set choiceIndex = -1 if no choices
        /// </summary>
        public virtual void Action(int choiceIndex) {
            var cb = mChoiceCallback;
            mChoiceCallback = null;
            if(cb != null)
                cb(choiceIndex);
        }

        protected abstract void Setup(string title, string description, object icon);

        void IModalPush.Push(GenericParams parms) {
            string title = "";
            string description = "";
            object icon = null;

            if(parms != null) {
                title = parms.GetValue<string>(paramTitle);
                description = parms.GetValue<string>(paramDesc);
                icon = parms.GetValue<object>(paramIconRef);

                mChoiceCallback = parms.GetValue<System.Action<int>>(paramCallback);
            }

            Setup(title, description, icon);
        }

        void IModalPop.Pop() {
            mChoiceCallback = null;
        }
    }
}
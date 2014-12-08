using UnityEngine;
using System.Collections;

namespace M8.UIModal.Dialogs {
    [AddComponentMenu("")]
    public abstract class CharacterDialogBase : Controller {
        //index = -1 if there are no choices and we clicked next
        public delegate void OnAction(CharacterDialogBase dlg, int choiceIndex);
                
        public event OnAction actionCallback;
                
        /// <summary>
        /// Set the text and such for the dialog, call this before opening the dialog
        /// </summary>
        public abstract void Apply(bool isLocalized, string text, string aName = null, string portraitSpriteRef = null, string[] choices = null);

        /// <summary>
        /// Set choiceIndex = -1 if no choices
        /// </summary>
        protected void Action(int choiceIndex = -1) {
            if(actionCallback != null)
                actionCallback(this, choiceIndex);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8.UIModal.Dialogs {
    [AddComponentMenu("")]
    public abstract class CharacterDialogBase : Controller {
        //use these during Push to populate
        public const string paramText = "text";
        public const string paramName = "name";
        public const string paramSpriteRef = "spriteRef";
        public const string paramChoiceArray = "choices";

        //index = -1 if there are no choices and we clicked next
        public delegate void OnAction(CharacterDialogBase dlg, int choiceIndex);
                
        public event OnAction actionCallback;

        /// <summary>
        /// Set choiceIndex = -1 if no choices
        /// </summary>
        protected void Action(int choiceIndex = -1) {
            if(actionCallback != null)
                actionCallback(this, choiceIndex);
        }
    }
}
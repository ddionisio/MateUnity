using UnityEngine;
using System.Collections;

namespace M8.UIModal {
    using Dialogs;

    public struct DialogUtil {
        public const string characterDialogRef = "character";
        public const string confirmDialogRef = "confirm";
        public const string messageDialogRef = "message";

        public static CharacterDialogBase CharacterDialog(bool isLocalized, string text, string aName = null, string portraitSpriteRef = null, string[] choices = null) {
            Manager ui = Manager.instance;
            CharacterDialogBase dlg = ui.ModalGetController<CharacterDialogBase>(characterDialogRef);

            if(dlg != null) {
                dlg.Apply(isLocalized, text, aName, portraitSpriteRef, choices);

                if(!ui.ModalIsInStack(characterDialogRef)) {
                    ui.ModalOpen(characterDialogRef); //will show on the next update
                }
            }
            else {
                Debug.LogWarning("Failed to open dialog: " + characterDialogRef);
            }

            return dlg;
        }

        public static void Confirm(ConfirmDialogBase.OnConfirm aCallback) {
            ConfirmDialogBase.Open(confirmDialogRef, null, null, aCallback);
        }

        public static void Confirm(string aTitle, string aText, ConfirmDialogBase.OnConfirm aCallback) {
            ConfirmDialogBase.Open(confirmDialogRef, aTitle, aText, aCallback);
        }

        public static void Message(string aTitle, string aText, MessageDialogBase.OnClick aCallback) {
            MessageDialogBase.Open(messageDialogRef, aTitle, aText, aCallback);
        }
    }
}
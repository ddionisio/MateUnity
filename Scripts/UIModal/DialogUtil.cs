using UnityEngine;
using System.Collections;

namespace M8.UIModal {
    using Dialogs;

    public struct DialogUtil {
        public const string characterDialogRef = "character";
        public const string confirmDialogRef = "confirm";
        public const string messageDialogRef = "message";

        public static void CharacterDialog(string text, string aName = null, string portraitSpriteRef = null, string[] choices = null) {
            var parms = new GenericParams(
                new GenericParamArg(CharacterDialogBase.paramText, text),
                new GenericParamArg(CharacterDialogBase.paramName, aName),
                new GenericParamArg(CharacterDialogBase.paramSpriteRef, portraitSpriteRef),
                new GenericParamArg(CharacterDialogBase.paramChoiceArray, choices));

            Manager.instance.ModalOpen(characterDialogRef, parms);
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
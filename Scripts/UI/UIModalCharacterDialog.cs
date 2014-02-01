using UnityEngine;
using System.Collections;

[AddComponentMenu("")]
public abstract class UIModalCharacterDialog : UIController {
    //index = -1 if there are no choices and we clicked next
    public delegate void OnAction(UIModalCharacterDialog dlg, int choiceIndex);

    public const string defaultModalRef = "CharacterDialog";

    public event OnAction actionCallback;

    public static UIModalCharacterDialog Open(bool isLocalized, string modalRef, string text, string aName = null, string portraitSpriteRef = null, string[] choices = null) {
        UIModalManager ui = UIModalManager.instance;
        UIModalCharacterDialog dlg = ui.ModalGetController<UIModalCharacterDialog>(modalRef);

        if(dlg != null) {
            dlg.Apply(isLocalized, text, aName, portraitSpriteRef, choices);

            if(!ui.ModalIsInStack(modalRef)) {
                ui.ModalOpen(modalRef); //will show on the next update
            }
        }
        else {
            Debug.LogWarning("Failed to open dialog: " + modalRef);
        }

        return dlg;
    }
    
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

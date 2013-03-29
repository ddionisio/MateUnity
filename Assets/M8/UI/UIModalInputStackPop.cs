using UnityEngine;
using System.Collections;

/// <summary>
/// Pop modal stack if given input is pressed. Put this component on a modal dialog game object that has a UIController.
/// </summary>
[RequireComponent(typeof(UIController))]
[AddComponentMenu("M8/UI/ModalInputStackPop")]
public class UIModalInputStackPop : MonoBehaviour {

    public InputAction escape;

    private UIController mController;

    void OnDestroy() {
        if(mController != null) {
            mController.onActiveCallback -= UIActive;
        }
    }

    void Awake() {
        mController = GetComponent<UIController>();
        if(mController != null) {
            mController.onActiveCallback += UIActive;
        }
    }

    void OnInputEscape(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed) {
            UIModalManager.instance.ModalCloseTop();
        }
    }

    void UIActive(bool active) {
        InputManager input = Main.instance.input;

        if(active) {
            input.AddButtonCall(escape, OnInputEscape);
        }
        else {
            input.RemoveButtonCall(escape, OnInputEscape);
        }
    }
}
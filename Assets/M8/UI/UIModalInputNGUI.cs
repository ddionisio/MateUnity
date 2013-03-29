using UnityEngine;
using System.Collections;

/// <summary>
/// Allow InputManager to handle input for NGUI.  Put this component along with ui modal manager. Use in conjuction with UIButtonKeys for each item.
/// </summary>
[AddComponentMenu("M8/UI/ModalInputNGUI")]
public class UIModalInputNGUI : MonoBehaviour {

    public InputAction axisX;
    public InputAction axisY;

    public float axisDelay = 0.25f;
    public float axisThreshold = 0.75f;

    public InputAction enter;
    public InputAction cancel;

    private UICamera.MouseOrTouch mController = new UICamera.MouseOrTouch();
    private bool mInputActive = false;

    void OnEnable() {
        if(mInputActive) {
            StartCoroutine(AxisCheck());
        }
    }

    void OnDestroy() {
        OnUIModalInactive();
    }

    void OnInputEnter(InputManager.Info data) {
        bool pressed = data.state == InputManager.State.Pressed;
        bool released = data.state == InputManager.State.Released;

        if(pressed || released) {
            UICamera.currentTouchID = -666;
            UICamera.currentTouch = mController;
            UICamera.currentTouch.current = UICamera.selectedObject;
            UICamera.eventHandler.ProcessTouch(pressed, released);
            UICamera.currentTouch.current = null;

            UICamera.currentTouch = null;
        }

        //UICamera.current
        //mController
       /* if(data.state == InputManager.State.Pressed && UICamera.selectedObject != null) {
            UICamera.Notify(UICamera.selectedObject, "OnClick", null);
        }*/
    }

    void OnInputCancel(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed && UICamera.selectedObject != null) {
            UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.Escape);
        }
    }

    IEnumerator AxisCheck() {
        float nextTime = 0.0f;

        while(mInputActive) {
            if(UICamera.selectedObject != null) {
                float time = Time.realtimeSinceStartup;
                if(nextTime < time) {
                    InputManager input = Main.instance.input;

                    float x = input.GetAxis(axisX);
                    if(x < -axisThreshold) {
                        nextTime = time + axisDelay;
                        UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.LeftArrow);
                    }
                    else if(x > axisThreshold) {
                        nextTime = time + axisDelay;
                        UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.RightArrow);
                    }

                    float y = input.GetAxis(axisY);
                    if(y < -axisThreshold) {
                        nextTime = time + axisDelay;
                        UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.DownArrow);
                    }
                    else if(y > axisThreshold) {
                        nextTime = time + axisDelay;
                        UICamera.Notify(UICamera.selectedObject, "OnKey", KeyCode.UpArrow);
                    }
                }
            }

            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    void OnUIModalActive() {
        if(!mInputActive) {
            //bind callbacks
            InputManager input = Main.instance.input;

            if(enter != InputAction.NumAction)
                input.AddButtonCall(enter, OnInputEnter);

            if(cancel != InputAction.NumAction)
                input.AddButtonCall(cancel, OnInputCancel);

            mInputActive = true;

            if(gameObject.activeInHierarchy) {
                StartCoroutine(AxisCheck());
            }
        }
    }

    void OnUIModalInactive() {
        if(mInputActive) {
            //unbind callbacks
            InputManager input = Main.instance != null ? Main.instance.input : null;

            if(input != null) {
                if(enter != InputAction.NumAction)
                    input.RemoveButtonCall(enter, OnInputEnter);

                if(cancel != InputAction.NumAction)
                    input.RemoveButtonCall(cancel, OnInputCancel);
            }

            mInputActive = false;
        }
    }
}
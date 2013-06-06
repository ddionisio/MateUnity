using UnityEngine;
using System.Collections;

/// <summary>
/// Use this to simulate a click with an ngui widget using input manager.
/// </summary>
[AddComponentMenu("M8/NGUI/InputClick")]
public class NGUIInputClick : MonoBehaviour {
    public int player = 0;
    public int action = 0;
    public int alternate = InputManager.ActionInvalid;
    public float axisCheck;
    public float axisDelay = 0.5f;

    /// <summary>
    /// Check to see if this object is selected via NGUI for input to process.
    /// </summary>
    public bool checkSelected;

    private bool mStarted;

    private bool mIsAxisAction = false;
    private bool mIsAxisActionAlt = false;
    private float mAxisLastTime = 0.0f;
    private bool mLastClick = false;

    void OnEnable() {
        if(mStarted)
            DoBind();
    }

    void OnDisable() {
        if(mStarted && Main.instance != null && Main.instance.input != null) {
            Main.instance.input.RemoveButtonCall(player, action, OnInputEnter);

            if(alternate != InputManager.ActionInvalid)
                Main.instance.input.RemoveButtonCall(player, alternate, OnInputEnter);
        }
    }

    // Use this for initialization
    void Start() {
        mStarted = true;
        DoBind();
    }

    void Update() {
        if(mIsAxisAction || mIsAxisActionAlt) {
            bool doClick = false;
            if(mIsAxisAction) {
                float axis = Main.instance.input.GetAxis(player, action);
                if(axisCheck < 0.0f) {
                    doClick = axis <= axisCheck;
                }
                else if(axisCheck > 0.0f) {
                    doClick = axis >= axisCheck;
                }
            }

            if(mIsAxisActionAlt && !doClick) {
                float axis = Main.instance.input.GetAxis(player, alternate);
                if(axisCheck < 0.0f) {
                    doClick = axis <= axisCheck;
                }
                else if(axisCheck > 0.0f) {
                    doClick = axis >= axisCheck;
                }
            }

            if(doClick) {
                if(!mLastClick) {
                    mLastClick = true;
                    mAxisLastTime = Time.time;
                }
                else if(Time.time - mAxisLastTime < axisDelay)
                    doClick = false;
                else {
                    mAxisLastTime = Time.time;
                }
            }
            else {
                if(mLastClick) {
                    mLastClick = false;
                    mAxisLastTime = 0.0f;
                }
            }

            if(doClick && (!checkSelected || UICamera.selectedObject == gameObject)) {
                UICamera.Notify(gameObject, "OnClick", null);
            }
        }
    }

    void DoBind() {
        mIsAxisAction = false;
        mIsAxisActionAlt = false;

        if(Main.instance != null && Main.instance.input != null) {
            if(Main.instance.input.GetControlType(action) == InputManager.Control.Button)
                Main.instance.input.AddButtonCall(player, action, OnInputEnter);
            else
                mIsAxisAction = true;

            if(alternate != InputManager.ActionInvalid) {
                if(Main.instance.input.GetControlType(action) == InputManager.Control.Button)
                    Main.instance.input.AddButtonCall(player, alternate, OnInputEnter);
                else
                    mIsAxisActionAlt = true;
            }
        }
    }

    void OnInputEnter(InputManager.Info data) {
        if(data.state == InputManager.State.Pressed && (!checkSelected || UICamera.selectedObject == gameObject)) {
            UICamera.Notify(gameObject, "OnClick", null);
        }
    }
}

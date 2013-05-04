using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Core/InputManagerOuya")]
public class InputManagerOuya : InputManager {
#if OUYA

    //TODO: handle players
    private const OuyaSDK.OuyaPlayer player = OuyaSDK.OuyaPlayer.player1;

    private bool[] mKeysDown = new bool[(int)InputKeyMap.NumKeys];

    private Dictionary<InputKeyMap, List<BindData>> mBindMap;

    protected override float ProcessAxis(Key key, float deadZone) {
        float ret = 0.0f;

        switch(key.map) {
            case InputKeyMap.LX:
                ret = OuyaInputManager.GetAxis("LX", player);
                if(Mathf.Abs(ret) <= deadZone)
                    ret = 0.0f;
                break;

            case InputKeyMap.LY:
                ret = OuyaInputManager.GetAxis("LY", player);
                if(Mathf.Abs(ret) <= deadZone)
                    ret = 0.0f;
                break;

            case InputKeyMap.RX:
                ret = OuyaInputManager.GetAxis("RX", player);
                if(Mathf.Abs(ret) <= deadZone)
                    ret = 0.0f;
                break;

            case InputKeyMap.RY:
                ret = OuyaInputManager.GetAxis("RY", player);
                if(Mathf.Abs(ret) <= deadZone)
                    ret = 0.0f;
                break;

            case InputKeyMap.DPADX:
                if(mKeysDown[(int)InputKeyMap.LEFT])
                    ret = -1.0f;
                else if(mKeysDown[(int)InputKeyMap.RIGHT])
                    ret = 1.0f;
                break;

            case InputKeyMap.DPADY:
                if(mKeysDown[(int)InputKeyMap.UP])
                    ret = 1.0f;
                else if(mKeysDown[(int)InputKeyMap.DOWN])
                    ret = -1.0f;
                break;
        }

        return ret;
    }

    protected override bool ProcessButtonDown(Key key) {
        if(key.map == InputKeyMap.None)
            return base.ProcessButtonDown(key);

        return mKeysDown[(int)key.map];
    }

    //internal

    protected override void OnDestroy() {
        base.OnDestroy();

        OuyaInputManager.OuyaButtonEvent.removeButtonEventListener(HandleButtonEvent);
        OuyaInputManager.initKeyStates();
    }

    protected override void Awake() {
        OuyaInputManager.OuyaButtonEvent.addButtonEventListener(HandleButtonEvent);

        base.Awake();

        //map out keys
        mBindMap = new Dictionary<InputKeyMap, List<BindData>>(mBinds.Length);

        foreach(BindData bind in mBinds) {
            foreach(Key key in bind.keys) {
                List<BindData> bindList;

                if(!mBindMap.TryGetValue(key.map, out bindList)) {
                    bindList = new List<BindData>();
                    mBindMap.Add(key.map, bindList);
                }

                bindList.Add(bind);
            }
        }
    }

    protected override void Update() {
        foreach(BindData bindData in mBinds) {
            if(bindData != null && bindData.keys != null) {
                switch(bindData.control) {
                    case Control.Axis:
                        bindData.info.axis = 0.0f;

                        foreach(Key key in bindData.keys) {
                            float axis = ProcessAxis(key, bindData.deadZone);
                            if(axis != 0.0f) {
                                bindData.info.axis = axis;
                                break;
                            }
                        }
                        break;
                }
            }
        }
    }

    private void KeyPressed(InputKeyMap keyMap) {
        int keyInd = (int)keyMap;

        if(!mKeysDown[keyInd]) {
            mKeysDown[keyInd] = true;

            //callback
            List<BindData> binds;
            if(mBindMap.TryGetValue(keyMap, out binds)) {
                foreach(BindData bind in binds) {
                    bind.info.axis = 1.0f; //hm
                    bind.info.state = State.Pressed;
                    bind.info.index = 0; //hm

                    bind.Call();
                }
            }
        }
    }

    private void KeyReleased(InputKeyMap keyMap) {
        int keyInd = (int)keyMap;

        if(mKeysDown[keyInd]) {
            mKeysDown[keyInd] = false;

            //callback
            List<BindData> binds;
            if(mBindMap.TryGetValue(keyMap, out binds)) {
                foreach(BindData bind in binds) {
                    bind.info.axis = 0.0f;
                    bind.info.state = State.Released;
                    bind.info.index = 0; //hm

                    bind.Call();
                }
            }
        }
    }

    private void HandleButtonEvent(OuyaSDK.OuyaPlayer p, OuyaSDK.KeyEnum b, OuyaSDK.InputAction bs) {
        if(p != player) {
            return;
        }

        switch(bs) {
            case OuyaSDK.InputAction.KeyDown:
                switch(b) {
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        KeyPressed(InputKeyMap.UP);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        KeyPressed(InputKeyMap.DOWN);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        KeyPressed(InputKeyMap.LEFT);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        KeyPressed(InputKeyMap.RIGHT);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_O:
                        KeyPressed(InputKeyMap.O);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_U:
                        KeyPressed(InputKeyMap.U);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        KeyPressed(InputKeyMap.Y);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_A:
                        KeyPressed(InputKeyMap.A);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                        KeyPressed(InputKeyMap.SYSTEM);
                        break;
                }
                break;

            case OuyaSDK.InputAction.KeyUp:
                switch(b) {
                    case OuyaSDK.KeyEnum.BUTTON_DPAD_UP:
                        KeyReleased(InputKeyMap.UP);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_DPAD_DOWN:
                        KeyReleased(InputKeyMap.DOWN);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_DPAD_LEFT:
                        KeyReleased(InputKeyMap.LEFT);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_DPAD_RIGHT:
                        KeyReleased(InputKeyMap.RIGHT);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_O:
                        KeyReleased(InputKeyMap.O);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_U:
                        KeyReleased(InputKeyMap.U);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_Y:
                        KeyReleased(InputKeyMap.Y);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_A:
                        KeyReleased(InputKeyMap.A);
                        break;

                    case OuyaSDK.KeyEnum.BUTTON_SYSTEM:
                        KeyReleased(InputKeyMap.SYSTEM);
                        break;
                }
                break;
        }
    }

#endif
}

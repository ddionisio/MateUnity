using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this on actions that need to be displayed on text via localization params.
    /// This should be placed in the core GameObject (core.prefab)
    /// </summary>
    [AddComponentMenu("M8/Input/Action Localize Param Binds")]
    public class InputActionLocalizeParamBinds : MonoBehaviour {
        public InputAction[] actions;
        public bool showOneBind; //only show the first valid bind

        void Awake() {
            var localizer = Localize.instance; //Localize.instance.RegisterParam(name, OnTextParam);

            if(showOneBind) {
                for(int i = 0; i < actions.Length; i++) {
                    localizer.RegisterParam(actions[i].name, delegate (string key) { return OnTextParam(actions[i], key); });
                }
            }
            else {
                for(int i = 0; i < actions.Length; i++) {
                    localizer.RegisterParam(actions[i].name, delegate (string key) { return OnTextParamAllBinds(actions[i], key); });
                }
            }
        }

        string OnTextParam(InputAction action, string key) {
            var _binds = action.binds;

            if(_binds == null)
                return "";

            for(int i = 0; i < _binds.Length; i++) {
                string keyString = action.GetKeyString(_binds[i]);
                if(!string.IsNullOrEmpty(keyString)) {
                    return keyString;
                }
            }

            return "";
        }

        string OnTextParamAllBinds(InputAction action, string key) {
            var _binds = action.binds;

            if(_binds == null)
                return "";

            var sb = new System.Text.StringBuilder();

            for(int i = 0; i < _binds.Length; i++) {
                string keyString = action.GetKeyString(_binds[i]);
                if(!string.IsNullOrEmpty(keyString)) {
                    sb.Append(keyString);

                    if(_binds.Length > 1 && i < _binds.Length - 1) {
                        sb.Append(", ");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
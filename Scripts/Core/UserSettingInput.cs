using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Use this to save input settings.  Use InputManager.GetKey to modify bindings,
    /// then call UserSettingInput.Apply
    /// See InputBindDialogBase for example use
    /// </summary>
    [PrefabCore]
    [AddComponentMenu("M8/Core/UserSettingInput")]
    public class UserSettingInput : UserSetting<UserSettingInput> {
        [System.Serializable]
        public struct ActionData {
            public string name;

            public InputAction action;

            public int bindIndexOffset;
            public int bindIndexCount;
            
            private string[] mKeyBindInputs;
            private string[] mKeyBindCodes;

            public void Init(string header) {
                if(string.IsNullOrEmpty(name))
                    name = action.name;

                string headerKey = GetKey(header, name);
                
                mKeyBindInputs = new string[bindIndexCount];
                mKeyBindCodes = new string[bindIndexCount];

                for(int i = 0; i < bindIndexCount; i++) {
                    var bindHeader = GetKey(headerKey, i.ToString());

                    mKeyBindInputs[i] = GetKey(bindHeader, "input");
                    mKeyBindCodes[i] = GetKey(bindHeader, "code");
                }
            }
            
            public void Load(UserData userData) {
                //make sure Init is called first
                                
                var _binds = action.binds;
                for(int i = 0; i < bindIndexCount; i++) {
                    action.ResetBind(i + bindIndexOffset);

                    var input = userData.GetString(mKeyBindInputs[i], "");
                    var code = userData.GetInt(mKeyBindCodes[i], InputAction.keyCodeNone);

                    if(!string.IsNullOrEmpty(input))
                        _binds[i + bindIndexOffset].SetAsInput(input);
                    else if(code != InputAction.keyCodeNone)
                        _binds[i + bindIndexOffset].SetAsKey(code);
                }
            }

            public void Apply(UserData userData) {
                //make sure Init is called first

                var _binds = action.binds;
                for(int i = 0; i < bindIndexCount; i++) {
                    var bind = _binds[i + bindIndexOffset];

                    if(!string.IsNullOrEmpty(bind.input))
                        userData.SetString(mKeyBindInputs[i], bind.input);
                    else if(bind.code != InputAction.keyCodeNone)
                        userData.SetInt(mKeyBindInputs[i], bind.code);
                }
            }

            public void ResetToDefault(UserData userData) {
                //make sure Init is called first
                //this will delete entries in userData

                var _binds = action.binds;
                for(int i = 0; i < bindIndexCount; i++) {
                    action.ResetBind(i + bindIndexOffset);

                    userData.Delete(mKeyBindInputs[i]);
                    userData.Delete(mKeyBindCodes[i]);
                }
            }

            private string GetKey(string header, params string[] items) {
                var sb = new System.Text.StringBuilder();

                if(!string.IsNullOrEmpty(header))
                    sb.Append(header);

                for(int i = 0; i < items.Length; i++) {
                    if(sb.Length > 0)
                        sb.Append('/');

                    sb.Append(items[i]);
                }

                return sb.ToString();
            }
        }

        public ActionData[] actions;
        public string header = "input";

        public override void Load() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].Load(userData);
        }

        public void RevertToDefault() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].ResetToDefault(userData);
        }

        public override void Save() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].Apply(userData);

            base.Save();
        }

        protected override void OnInstanceInit() {
            for(int i = 0; i < actions.Length; i++)
                actions[i].Init(header);

            base.OnInstanceInit();
        }
    }
}
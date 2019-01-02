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
        public class ActionData {
            public string name;

            public InputAction action;

            public int bindIndexOffset;
            public int bindIndexCount;
            
            private string[] mKeyBinds;

            public void Init(string header) {
                if(string.IsNullOrEmpty(name))
                    name = action.name;

                string headerKey = GetKey(header, name);
                
                mKeyBinds = new string[bindIndexCount];

                for(int i = 0; i < bindIndexCount; i++) {
                    var bindHeader = GetKey(headerKey, i.ToString());

                    mKeyBinds[i] = GetKey(bindHeader, "k");
                }
            }
            
            public void Load(UserData userData) {
                //make sure Init is called first

                for(int i = 0; i < bindIndexCount; i++)
                    Load(userData, i);
            }

            public void Load(UserData userData, int index) {
                //make sure Init is called first

                action.ResetBind(index + bindIndexOffset);

                var input = userData.GetString(mKeyBinds[index], "");
                var code = userData.GetInt(mKeyBinds[index], InputAction.keyCodeNone);

                if(!string.IsNullOrEmpty(input))
                    action.binds[index + bindIndexOffset].SetAsInput(input);
                else if(code != InputAction.keyCodeNone)
                    action.binds[index + bindIndexOffset].SetAsKey(code);
            }

            public void Apply(UserData userData) {
                //make sure Init is called first

                for(int i = 0; i < bindIndexCount; i++)
                    Apply(userData, i);
            }

            public void Apply(UserData userData, int index) {
                //make sure Init is called first

                var bind = action.binds[index + bindIndexOffset];

                if(!string.IsNullOrEmpty(bind.input))
                    userData.SetString(mKeyBinds[index], bind.input);
                else if(bind.code != InputAction.keyCodeNone)
                    userData.SetInt(mKeyBinds[index], bind.code);
                else
                    userData.Remove(mKeyBinds[index]);
            }

            public void SetAsKey(UserData userData, int index, int code) {
                //make sure Init is called first

                action.binds[index + bindIndexOffset].SetAsKey(code);
            }

            public void SetAsInput(UserData userData, int index, string input) {
                //make sure Init is called first

                action.binds[index + bindIndexOffset].SetAsInput(input);
            }

            public void ResetToDefault(UserData userData) {
                //make sure Init is called first
                //this will delete entries in userData

                var _binds = action.binds;
                for(int i = 0; i < bindIndexCount; i++) {
                    action.ResetBind(i + bindIndexOffset);

                    userData.Remove(mKeyBinds[i]);
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

        [SerializeField]
        string _header = "input";
        [SerializeField]
        ActionData[] _actions = new ActionData[0];

        /// <summary>
        /// Grab action data index based on given name (note: this is the ActionData.name, not InputAction)
        /// </summary>
        public int GetActionDataIndex(string actionDataName) {
            for(int i = 0; i < _actions.Length; i++) {
                if(_actions[i].name == actionDataName)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Apply Bind (code) to action, keyIndex is relative to ActionData.bindIndexOffset
        /// </summary>
        public void SetBind(int actionDataIndex, int keyIndex, int code) {
            _actions[actionDataIndex].SetAsKey(userData, keyIndex, code);
        }

        /// <summary>
        /// Apply Bind (input) to action, keyIndex is relative to ActionData.bindIndexOffset
        /// </summary>
        public void SetBind(int actionDataIndex, int keyIndex, string input) {
            _actions[actionDataIndex].SetAsInput(userData, keyIndex, input);
        }

        /// <summary>
        /// Revert all binds in ActionData.InputAction (within bindIndexOffset, bindIndexCount)
        /// </summary>
        public void RevertBind(int actionDataIndex) {
            _actions[actionDataIndex].Load(userData);
        }

        /// <summary>
        /// Revert binds to InputAction in ActionData, keyIndex is relative to ActionData.bindIndexOffset
        /// </summary>
        public void RevertBind(int actionDataIndex, int keyIndex) {
            _actions[actionDataIndex].Load(userData, keyIndex);
        }

        public override void Load() {
            for(int i = 0; i < _actions.Length; i++)
                _actions[i].Load(userData);
        }

        public void RevertToDefault() {
            for(int i = 0; i < _actions.Length; i++)
                _actions[i].ResetToDefault(userData);
        }

        public override void Save() {
            for(int i = 0; i < _actions.Length; i++)
                _actions[i].Apply(userData);

            base.Save();
        }

        protected override void OnInstanceInit() {
            for(int i = 0; i < _actions.Length; i++)
                _actions[i].Init(_header);

            base.OnInstanceInit();
        }
    }
}
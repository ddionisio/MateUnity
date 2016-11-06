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
        [System.Flags]
        public enum Flags : ushort {
            Binds = 0x1,
            Sensitivity = 0x2,

            All = 0xffff
        }

        /// <summary>
        /// Revert to input's config (if deleteSettings = true), otherwise reload from current settings
        /// </summary>
        public void Revert(Flags flags, bool deleteSettings) {
            InputManager input = InputManager.instance;

            bool isBinds = (flags & Flags.Binds) != 0;
            
            if(isBinds)
                input.RevertBinds();

            if(deleteSettings) {
                int actCount = input.actionCount;
                for(int act = 0; act < actCount; act++) {
                    InputManager.BindData bindDat = input.GetBindData(act);
                    if(bindDat == null)
                        continue;

                    for(int player = 0; player < bindDat.players.Length; player++) {
                        InputManager.PlayerData pd = bindDat.players[player];
                        if(pd == null)
                            continue;

                        //binds
                        if(isBinds) {
                            for(int index = 0; index < pd.keys.Length; index++) {
                                string usdKey = _BaseKey(act, player, index);
                                _DeleteBindPlayerPrefs(usdKey);
                            }
                        }

                        //other settings
                        var bindKey = _BaseKey(act, player);

                        //sensitivity
                        if((flags & Flags.Sensitivity) != 0)
                            userData.Delete(bindKey + "_s");
                    }
                }
            }
            else { //reload from settings
                Load(flags);
            }
        }

        /// <summary>
        /// Call this once you are done modifying key binds, note: this will not necessarily save to persistent data,
        /// call Save() to do actual saving
        /// </summary>
        public void Apply(Flags flags) {
            InputManager input = InputManager.instance;

            int actCount = input.actionCount;
            for(int act = 0; act < actCount; act++) {
                InputManager.BindData bindDat = input.GetBindData(act);
                if(bindDat == null)
                    continue;

                for(int player = 0; player < bindDat.players.Length; player++) {
                    InputManager.PlayerData pd = bindDat.players[player];
                    if(pd == null)
                        continue;

                    //binds
                    if((flags & Flags.Binds) != 0) {
                        for(int index = 0; index < pd.keys.Length; index++) {
                            string usdKey = _BaseKey(act, player, index);

                            InputManager.Key key = pd.keys[index];
                            if(key.isValid) {
                                if(key.IsDirty()) {
                                    //for previous bind if type is changed
                                    _DeleteBindPlayerPrefs(usdKey);

                                    if(!string.IsNullOrEmpty(key.input)) {
                                        userData.SetString(usdKey + "_k", key.input);
                                    }
                                    else {
                                        //pack data
                                        if(key.code != KeyCode.None)
                                            userData.SetInt(usdKey + "_k", key._CreateKeyCodeDataPak());
                                        else if(key.map != InputKeyMap.None)
                                            userData.SetInt(usdKey + "_m", key._CreateMapDataPak());
                                    }

                                    key.SetDirty(false);
                                }
                            }
                            else {
                                _DeleteBindPlayerPrefs(usdKey);
                                userData.SetString(usdKey + "_d", "-");
                            }
                        }
                    }

                    //other settings
                    var bindKey = _BaseKey(act, player);

                    //sensitivity
                    if((flags & Flags.Sensitivity) != 0)
                        userData.SetFloat(bindKey + "_s", pd.sensitivity);
                }
            }
        }

        private void Load(Flags flags) {
            InputManager input = InputManager.instance;

            int actCount = input.actionCount;
            for(int act = 0; act < actCount; act++) {
                InputManager.BindData bindDat = input.GetBindData(act);
                if(bindDat == null)
                    continue;

                //load keys
                for(int player = 0; player < bindDat.players.Length; player++) {
                    InputManager.PlayerData pd = bindDat.players[player];
                    if(pd == null)
                        continue;

                    //binds
                    if((flags & Flags.Binds) != 0) {
                        for(int index = 0; index < pd.keys.Length; index++) {
                            string usdKey = _BaseKey(act, player, index);

                            if(userData.HasKey(usdKey + "_i")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new InputManager.Key();

                                pd.keys[index].SetAsInput(userData.GetString(usdKey + "_i"));
                            }
                            else if(userData.HasKey(usdKey + "_k")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new InputManager.Key();

                                pd.keys[index]._SetAsKey((uint)userData.GetInt(usdKey + "_k"));
                            }
                            else if(userData.HasKey(usdKey + "_m")) {
                                if(pd.keys[index] == null)
                                    pd.keys[index] = new InputManager.Key();

                                pd.keys[index]._SetAsMap((uint)userData.GetInt(usdKey + "_m"));
                            }
                            else if(userData.HasKey(usdKey + "_d"))
                                pd.keys[index].ResetKeys();
                        }
                    }

                    //load other settings
                    string bindKey = _BaseKey(act, player);

                    //sensitivity
                    if((flags & Flags.Sensitivity) != 0 && userData.HasKey(bindKey + "_s"))
                        pd.sensitivity = userData.GetFloat(bindKey + "_s", 1.0f);
                }
            }
        }
        
        string _BaseKey(int action, int player) {
            return string.Format("bind_{0}_{1}", action, player);
        }

        string _BaseKey(int action, int player, int index) {
            return string.Format("bind_{0}_{1}_{2}", action, player, index);
        }

        void _DeleteBindPlayerPrefs(string baseKey) {
            userData.Delete(baseKey + "_i");
            userData.Delete(baseKey + "_k");
            userData.Delete(baseKey + "_m");
            userData.Delete(baseKey + "_d");
        }

        protected override void OnInstanceInit() {
            base.OnInstanceInit();

            //load user config binds
            Load(Flags.All);
        }
    }
}
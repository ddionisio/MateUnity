using UnityEngine;
using System.Collections;

/// <summary>
/// Use this to save input settings.  Use InputManager.GetKey to modify bindings,
/// then call UserSettingInput.Apply
/// See InputBindDialogBase for example use
/// </summary>
[AddComponentMenu("M8/Core/UserSettingInput")]
public class UserSettingInput : UserSetting {

    private static UserSettingInput mInstance;

    public static UserSettingInput instance { get { return mInstance; } }

    /// <summary>
    /// Revert to input's config (if deleteSettings = true), otherwise reload from current settings
    /// </summary>
    public void Revert(bool deleteSettings) {
        InputManager input = InputManager.instance;

        input.RevertBinds();

        if(deleteSettings) {
            int actCount = input.actionCount;
            for(int act = 0; act < actCount; act++) {
                InputManager.BindData bindDat = input.GetBindData(act);
                if(bindDat != null) {
                    for(int player = 0; player < bindDat.players.Length; player++) {
                        InputManager.PlayerData pd = bindDat.players[player];
                        if(pd != null) {
                            for(int index = 0; index < pd.keys.Length; index++) {
                                string usdKey = _BaseKey(act, player, index);
                                _DeletePlayerPrefs(usdKey);
                            }
                        }
                    }
                }
            }
        }
        else { //reload from settings
            LoadBinds();
        }
    }

    /// <summary>
    /// Call this once you are done modifying key binds, note: this will not necessarily save to persistent data,
    /// call Save() to do actual saving
    /// </summary>
    public void Apply() {
        InputManager input = InputManager.instance;

        int actCount = input.actionCount;
        for(int act = 0; act < actCount; act++) {
            InputManager.BindData bindDat = input.GetBindData(act);
            if(bindDat != null) {
                for(int player = 0; player < bindDat.players.Length; player++) {
                    InputManager.PlayerData pd = bindDat.players[player];
                    if(pd != null) {
                        for(int index = 0; index < pd.keys.Length; index++) {
                            string usdKey = _BaseKey(act, player, index);

                            InputManager.Key key = pd.keys[index];
                            if(key.isValid) {
                                if(key.IsDirty()) {
                                    //for previous bind if type is changed
                                    _DeletePlayerPrefs(usdKey);

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
                                _DeletePlayerPrefs(usdKey);
                                userData.SetString(usdKey + "_d", "-");
                            }
                        }
                    }
                }
            }
        }
    }

    private void LoadBinds() {
        InputManager input = InputManager.instance;

        int actCount = input.actionCount;
        for(int act = 0; act < actCount; act++) {
            InputManager.BindData bindDat = input.GetBindData(act);
            if(bindDat != null) {
                for(int player = 0; player < bindDat.players.Length; player++) {
                    InputManager.PlayerData pd = bindDat.players[player];
                    if(pd != null) {
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
                }
            }
        }
    }

    string _BaseKey(int action, int player, int key) {
        return string.Format("bind_{0}_{1}_{2}", action, player, key);
    }

    void _DeletePlayerPrefs(string baseKey) {
        userData.Delete(baseKey + "_i");
        userData.Delete(baseKey + "_k");
        userData.Delete(baseKey + "_m");
        userData.Delete(baseKey + "_d");
    }

    void OnDestroy() {
        mInstance = null;
    }

    protected override void Awake() {
        mInstance = this;

        base.Awake();

        //load user config binds
        LoadBinds();
    }
}

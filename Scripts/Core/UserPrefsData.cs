using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Core/UserPrefsData")]
public class UserPrefsData : UserData {
    [SerializeField]
    string _prefKey = "ud";

    public string prefKey {
        get { return _prefKey; }
        set {
            if(_prefKey != value) {
                Delete();
                _prefKey = value;
                Load();
            }
        }
    }

    protected override byte[] LoadRawData() {
        return System.Convert.FromBase64String(PlayerPrefs.GetString(_prefKey, ""));
    }

    protected override void SaveRawData(byte[] dat) {
        PlayerPrefs.SetString(_prefKey, System.Convert.ToBase64String(dat));
    }

    protected override void DeleteRawData() {
        PlayerPrefs.DeleteKey(_prefKey);
    }
}

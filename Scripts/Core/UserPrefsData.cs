using UnityEngine;
using System.Collections;

namespace M8 {
    [CreateAssetMenu(fileName = "userDataPref", menuName = "M8/UserData/From Prefs")]
    public class UserPrefsData : UserData {
        [SerializeField]
        string _prefKey = "ud";

        public string prefKey {
            get { return _prefKey; }
            set {
                if(_prefKey != value) {
                    Unload();
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
            PlayerPrefs.Save();
        }

        protected override void DeleteRawData() {
            PlayerPrefs.DeleteKey(_prefKey);
            PlayerPrefs.Save();
        }
    }
}
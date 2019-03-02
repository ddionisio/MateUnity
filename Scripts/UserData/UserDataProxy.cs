using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Add this to allow calls to UserData via events
    /// </summary>
    [AddComponentMenu("M8/Core/UserData Proxy")]
    public class UserDataProxy : MonoBehaviour {
        [SerializeField]
        UserData _userData = null;

        public void Load() {
            _userData.Load();
        }

        public void Save() {
            _userData.Save();
        }
    }
}
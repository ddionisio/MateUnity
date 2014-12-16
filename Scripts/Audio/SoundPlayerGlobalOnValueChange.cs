using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Audio/SoundPlayerGlobalOnValueChange")]
    public class SoundPlayerGlobalOnValueChange : MonoBehaviour {
        public string id;

        void OnValueChange(float val) {
            SoundPlayerGlobal.instance.Play(id);
        }
    }
}
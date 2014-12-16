using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Audio/SoundPlayerOnSelect")]
    public class SoundPlayerOnSelect : SoundPlayer {
        public bool onDeselect;

        void OnSelect(bool yes) {
            if((!onDeselect && yes) || (onDeselect && !yes)) {
                Play();
            }
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Audio/SoundPlayerOnClick")]
    public class SoundPlayerOnClick : SoundPlayer {
        void OnClick() {
            Play();
        }
    }
}
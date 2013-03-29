using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Audio/SoundPlayerOnClick")]
public class SoundPlayerOnClick : SoundPlayer {
    void OnClick() {
        Play();
    }
}

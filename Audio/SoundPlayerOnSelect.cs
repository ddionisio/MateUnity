using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Audio/SoundPlayerOnSelect")]
public class SoundPlayerOnSelect : SoundPlayer {

    void OnSelect(bool yes) {
        if(yes)
            Play();
    }
}

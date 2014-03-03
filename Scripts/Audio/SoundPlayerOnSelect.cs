using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Audio/SoundPlayerOnSelect")]
public class SoundPlayerOnSelect : SoundPlayer {
    public bool onDeselect;

    void OnSelect(bool yes) {
        if((!onDeselect && yes) || (onDeselect && !yes)) {
            Play();
        }
    }
}

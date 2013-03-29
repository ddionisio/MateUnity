using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Audio/SoundPlayerGlobalOnValueChange")]
public class SoundPlayerGlobalOnValueChange : MonoBehaviour {
    public string id;

    void OnValueChange(float val) {
        SoundPlayerGlobal.instance.Play(id);
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Audio/SoundPlayerGlobalOnClick")]
public class SoundPlayerGlobalOnClick : MonoBehaviour {
    public string id;

    void OnClick() {
        SoundPlayerGlobal.instance.Play(id);
    }
}

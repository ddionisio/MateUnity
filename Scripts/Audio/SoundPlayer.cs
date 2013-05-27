using UnityEngine;
using System.Collections;

/// <summary>
/// Base class for playing sounds, need to inherit from this in order to allow global sound settings to affect.
/// Put this alongside an audio source
/// </summary>
[AddComponentMenu("M8/Audio/SoundPlayer")]
public class SoundPlayer : MonoBehaviour {
    public const float refRate = 44100.0f;

    /// <summary>
    /// Play the sound whenever it is enabled
    /// </summary>
    public bool playOnActive = false;

    public float playDelay = 0.0f;

    private bool mStarted = false;
    private float mDefaultVolume = 1.0f;

    public float defaultVolume { get { return mDefaultVolume; } set { mDefaultVolume = value; } }

    public virtual void Play() {
        UserSettings us = Main.instance.userSettings;
        audio.volume = mDefaultVolume * us.soundVolume;

        if(playDelay > 0.0f)
            audio.PlayDelayed(refRate * playDelay);
        else
            audio.Play();
    }

    protected virtual void OnEnable() {
        if(mStarted)
            Play();
    }

    protected virtual void Awake() {
        audio.playOnAwake = playOnActive;

        mDefaultVolume = audio.volume;
    }

    // Use this for initialization
    protected virtual void Start() {
        mStarted = true;
    }

    void UserSettingsChanged(UserSettings us) {
        if(audio.isPlaying)
            audio.volume = mDefaultVolume * us.soundVolume;
    }
}

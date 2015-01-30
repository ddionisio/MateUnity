using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Base class for playing sounds, need to inherit from this in order to allow global sound settings to affect.
    /// Put this alongside an audio source
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
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

        public bool isPlaying { get { return audio.isPlaying; } }
        public float defaultVolume { get { return mDefaultVolume; } set { mDefaultVolume = value; } }

        public virtual void Play() {
            audio.volume = mDefaultVolume * UserSettingAudio.instance.soundVolume;

            if(playDelay > 0.0f)
                audio.PlayDelayed(playDelay);
            else
                audio.Play();
        }

        public virtual void Stop() {
            audio.Stop();
        }

        protected virtual void OnEnable() {
            if(mStarted && playOnActive)
                Play();
        }

        protected virtual void OnDestroy() {
            if(UserSettingAudio.instance)
                UserSettingAudio.instance.changeCallback -= UserSettingsChanged;
        }

        protected virtual void Awake() {
            audio.playOnAwake = false;

            mDefaultVolume = audio.volume;

            audio.volume = mDefaultVolume * UserSettingAudio.instance.soundVolume;

            UserSettingAudio.instance.changeCallback += UserSettingsChanged;
        }

        // Use this for initialization
        protected virtual void Start() {
            mStarted = true;

            if(playOnActive)
                Play();
        }

        void UserSettingsChanged(UserSettingAudio us) {
            //if(audio.isPlaying)
            audio.volume = mDefaultVolume * us.soundVolume;
        }
    }
}
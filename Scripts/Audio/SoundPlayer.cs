using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Base class for playing sounds, need to inherit from this in order to allow global sound settings to affect.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("M8/Audio/SoundPlayer")]
    public class SoundPlayer : MonoBehaviour {
        [SerializeField]
        AudioSource _target;

        /// <summary>
        /// Play the sound whenever it is enabled
        /// </summary>
        public bool playOnActive = false;

        public float playDelay = 0.0f;

        private bool mStarted = false;
        private float mDefaultVolume = 1.0f;

        public AudioSource target {
            get { return _target; }
            set {
                if(_target != value) {
                    _target = value;
                    if(_target)
                        InitTarget();
                }
            }
        }

        public bool isPlaying { get { return _target.isPlaying; } }
        public float defaultVolume { get { return mDefaultVolume; } set { mDefaultVolume = value; } }

        public virtual void Play() {
            _target.volume = mDefaultVolume * UserSettingAudio.instance.soundVolume;

            if(playDelay > 0.0f)
                _target.PlayDelayed(playDelay);
            else
                _target.Play();
        }

        public virtual void Stop() {
            _target.Stop();
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
            if(_target)
                InitTarget();
            else
                target = GetComponent<AudioSource>();

            UserSettingAudio.instance.changeCallback += UserSettingsChanged;
        }

        // Use this for initialization
        protected virtual void Start() {
            mStarted = true;

            if(playOnActive)
                Play();
        }

        void InitTarget() {
            _target.playOnAwake = false;
            mDefaultVolume = _target.volume;
            _target.volume = mDefaultVolume * UserSettingAudio.instance.soundVolume;
        }

        void UserSettingsChanged(UserSettingAudio us) {
            //if(audio.isPlaying)
            _target.volume = mDefaultVolume * us.soundVolume;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/UserSettingAudio")]
    public class UserSettingAudio : UserSetting<UserSettingAudio> {
        public const string volumeKey = "volumeMaster";
        public const string soundKey = "volumeSfx";
        public const string musicKey = "volumeMusic";

        [SerializeField]
        float _masterVolumeDefault = 1.0f;
        [SerializeField]
        float _soundVolumeDefault = 1.0f;
        [SerializeField]
        float _musicVolumeDefault = 1.0f;
                
        public float soundVolume {
            get { return mSoundVolume; }

            set {
                if(mSoundVolume != value) {
                    mSoundVolume = value;
                    userData.SetFloat(soundKey, mSoundVolume);

                    RelaySettingsChanged();
                }
            }
        }

        public float musicVolume {
            get { return mMusicVolume; }

            set {
                if(mMusicVolume != value) {
                    mMusicVolume = value;
                    userData.SetFloat(musicKey, mMusicVolume);

                    RelaySettingsChanged();
                }
            }
        }

        public float volume {
            get { return mVolume; }

            set {
                if(mVolume != value) {
                    mVolume = value;
                    userData.SetFloat(volumeKey, mVolume);

                    AudioListener.volume = mVolume;
                }
            }
        }

        public float volumeDefault { get { return _masterVolumeDefault; } }
        public float soundVolumeDefault { get { return _soundVolumeDefault; } }
        public float musicVolumeDefault { get { return _musicVolumeDefault; } }

        private float mVolume;
        private float mSoundVolume;
        private float mMusicVolume;

        public override void Load() {
            mVolume = userData.GetFloat(volumeKey, _masterVolumeDefault);

            mSoundVolume = userData.GetFloat(soundKey, _soundVolumeDefault);

            mMusicVolume = userData.GetFloat(musicKey, _musicVolumeDefault);

            AudioListener.volume = mVolume;
        }
    }
}
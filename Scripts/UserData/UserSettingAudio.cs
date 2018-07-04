using UnityEngine;
using System.Collections;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Core/UserSettingAudio")]
    public class UserSettingAudio : UserSetting<UserSettingAudio> {
        public const string volumeKey = "volumeMaster";
        public const string soundKey = "volumeSfx";
        public const string musicKey = "volumeMusic";

        //need to debug while listening to music
#if UNITY_EDITOR
        private const float volumeDefault = 1.0f;
#else
	private const float volumeDefault = 1.0f;
#endif

        private float mVolume;
        private float mSoundVolume;
        private float mMusicVolume;

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

        public override void Load() {
            mVolume = userData.GetFloat(volumeKey, 1.0f);

            mSoundVolume = userData.GetFloat(soundKey, volumeDefault);

            mMusicVolume = userData.GetFloat(musicKey, volumeDefault);

            AudioListener.volume = mVolume;
        }
    }
}
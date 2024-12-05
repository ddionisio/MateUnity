using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Audio/Sound Playlist Proxy")]
    public class SoundPlaylistProxy : MonoBehaviour {
        public enum Mode {
            TwoD,
            ThreeD,
            ThreeDFollow
        }

        [SoundPlaylist]
        public string sound;
        public Mode mode = Mode.TwoD;
        public bool checkPlaying;

        private AudioSourceProxy mLastPlayed;

        public void Play() {
            if(string.IsNullOrEmpty(sound))
                return;

            if(checkPlaying && mLastPlayed && mLastPlayed.isPlaying)
                return;

            switch(mode) {
                case Mode.TwoD:
					mLastPlayed = SoundPlaylist.instance.Play(sound, false);
                    break;
                case Mode.ThreeD:
					mLastPlayed = SoundPlaylist.instance.Play(sound, transform.position, false);
                    break;
                case Mode.ThreeDFollow:
					mLastPlayed = SoundPlaylist.instance.Play(sound, transform, false);
                    break;
            }
        }

		void OnDisable() {
			mLastPlayed = null;
		}
	}
}
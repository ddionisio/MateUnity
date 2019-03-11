using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Audio/Music Playlist Proxy")]
    public class MusicPlaylistProxy : MonoBehaviour {
        [MusicPlaylist]
        public string music;
        public bool loop;
        public bool immediate;

        public void Play(bool checkPrevious) {
            var ctrl = MusicPlaylist.instance;

            //don't play if it's already playing
            if(checkPrevious) {
                if(ctrl.lastPlayName == music && ctrl.isPlaying)
                    return;
            }

            ctrl.Play(music, loop, immediate);
        }

        public void Stop(bool aImmediate) {
            MusicPlaylist.instance.Stop(aImmediate);
        }
    }
}
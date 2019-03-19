using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Audio/Music Playlist Play On Enable")]
    public class MusicPlaylistPlayOnEnable : MonoBehaviour {
        [MusicPlaylist]
        public string music;
        public bool loop;
        public bool immediate;
        public bool checkPrevious = true; //if true, don't play if it's already playing

        void OnEnable() {
            var ctrl = MusicPlaylist.instance;

            //don't play if it's already playing
            if(checkPrevious) {
                if(ctrl.lastPlayName == music && ctrl.isPlaying)
                    return;
            }

            ctrl.Play(music, loop, immediate);
        }
    }
}
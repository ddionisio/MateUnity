using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Note: Create musicPlaylist.asset in the Resources folder with this script.
    /// </summary>
    [CreateAssetMenu(fileName = "musicPlaylist", menuName = "M8/Music Playlist")]
    public class MusicPlaylist : SingletonScriptableObject<MusicPlaylist> {
        [System.Serializable]
        public class MusicData {
            public string name;
            public AudioClip source;
        }

        public delegate void OnMusicFinish(string curMusicName);

        public MusicData[] music;

        [Header("Config")]
        public AudioMixerGroup audioMixer;
        public bool muteOnFocusLost = true;
        public float fadeInDelay = 0.3f;
        public float fadeOutDelay = 0.3f;

        public bool isPlaying { get { return mSourceProxy && mSourceProxy.isPlaying; } }

        public string lastPlayName { get; private set; }

        public AudioSourceProxy sourceProxy {
            get {
                if(!mSourceProxy)
                    SetupSourceProxy(null);

                return mSourceProxy;
            }
        }
        
        private AudioSourceProxy mSourceProxy;

        private Dictionary<string, MusicData> mMusic;

        public void SetupSourceProxy(GameObject sourceGO) {
            if(!sourceGO) {
                sourceGO = new GameObject("musicPlaylistSource");
                DontDestroyOnLoad(sourceGO);
            }

            var audioSrc = Util.GetOrAddComponent<AudioSource>(sourceGO);
            audioSrc.outputAudioMixerGroup = audioMixer;
            audioSrc.playOnAwake = false;

            mSourceProxy = Util.GetOrAddComponent<AudioSourceProxy>(sourceGO);
            mSourceProxy.settingsSource = AudioSourceProxy.VolumeSourceType.Music;
            mSourceProxy.audioSource = audioSrc;
            mSourceProxy.muteOnFocusLost = muteOnFocusLost;
        }

        public bool Exists(string name) {
            if(mMusic != null)
                return mMusic.ContainsKey(name);
            else if(music != null) {
                for(int i = 0; i < music.Length; i++) {
                    if(music[i].name == name)
                        return true;
                }
            }

            return false;
        }

        public void Play(string name, bool loop, bool immediate) {
            var proxy = sourceProxy;
            if(!proxy)
                return;

            MusicData dat;
            if(!mMusic.TryGetValue(name, out dat)) {
                Debug.LogWarning("Music does not exist: " + name);
                return;
            }

            lastPlayName = name;

            float _fadeInDelay, _fadeOutDelay;
            if(immediate) {
                _fadeInDelay = 0f;
                _fadeOutDelay = 0f;
            }
            else {
                _fadeInDelay = fadeInDelay;
                _fadeOutDelay = fadeOutDelay;
            }

            proxy.Play2DFade(dat.source, loop, _fadeOutDelay, _fadeInDelay);
        }

        public void Stop(bool immediate) {
            var proxy = sourceProxy;
            if(!proxy)
                return;

            if(immediate)
                proxy.StopFade(0f);
            else
                proxy.StopFade(fadeOutDelay);
        }
        
        protected override void OnInstanceInit() {
            lastPlayName = null;

            mMusic = new Dictionary<string, MusicData>(music.Length);
            for(int i = 0; i < music.Length; i++)
                mMusic.Add(music[i].name, music[i]);
        }
    }
}
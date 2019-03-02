using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [CreateAssetMenu(fileName = "soundPlaylist", menuName = "M8/Sound Playlist")]
    public class SoundPlaylist : SingletonScriptableObject<SoundPlaylist> {
        [System.Serializable]
        public class SoundData {
            public string name;
            public AudioClip clip;
            public float spatialBlend = 1f; //use for playing 3D
        }

        public SoundData[] sounds;

        public int max = 32;

        [Header("Config")]
        public AudioMixerGroup audioMixer;
        public bool muteOnFocusLost = true;

        private Dictionary<string, SoundData> mSfx;

        private GameObject mSourceRootGO;

        private CacheList<AudioSourceProxy> mSourceCache;

        public bool Exists(string name) {
            if(mSfx != null)
                return mSfx.ContainsKey(name);
            else if(sounds != null) {
                for(int i = 0; i < sounds.Length; i++) {
                    if(sounds[i].name == name)
                        return true;
                }
            }

            return false;
        }
        
        public AudioSourceProxy Play(string name, bool loop) {
            if(loop) {
                SoundData dat;
                if(!mSfx.TryGetValue(name, out dat)) {
                    Debug.LogWarning("Sound does not exist: " + name);
                    return null;
                }

                var src = GetAvailable();
                if(src)
                    src.Play2D(dat.clip, true);

                return src;
            }
            else
                return Play(name, null, null);
        }

        public AudioSourceProxy Play(string name, System.Action<GenericParams> callback, GenericParams parms) {
            SoundData dat;
            if(!mSfx.TryGetValue(name, out dat)) {
                Debug.LogWarning("Sound does not exist: " + name);
                return null;
            }

            var src = GetAvailable();
            if(src) {
                src.Play2D(dat.clip, delegate (GenericParams _parms) {
                    Cache(src); //add back

                    if(callback != null)
                        callback(_parms);
                }, parms);
            }

            return src;
        }

        public AudioSourceProxy Play(string name, Transform follow, bool loop) {
            if(loop) {
                SoundData dat;
                if(!mSfx.TryGetValue(name, out dat)) {
                    Debug.LogWarning("Sound does not exist: " + name);
                    return null;
                }

                var src = GetAvailable();
                if(src)
                    src.Play(dat.clip, dat.spatialBlend, follow, true);

                return src;
            }
            else
                return Play(name, follow, null, null);
        }

        public AudioSourceProxy Play(string name, Transform follow, System.Action<GenericParams> callback, GenericParams parms) {
            SoundData dat;
            if(!mSfx.TryGetValue(name, out dat)) {
                Debug.LogWarning("Sound does not exist: " + name);
                return null;
            }

            var src = GetAvailable();
            if(src) {
                src.Play(dat.clip, dat.spatialBlend, follow, delegate (GenericParams _parms) {
                    Cache(src); //add back

                    if(callback != null)
                        callback(_parms);
                }, parms);
            }

            return src;
        }

        public AudioSourceProxy Play(string name, Vector3 position, bool loop) {
            if(loop) {
                SoundData dat;
                if(!mSfx.TryGetValue(name, out dat)) {
                    Debug.LogWarning("Sound does not exist: " + name);
                    return null;
                }

                var src = GetAvailable();
                if(src)
                    src.Play(dat.clip, dat.spatialBlend, position, true);

                return src;
            }
            else
                return Play(name, position, null, null);
        }

        public AudioSourceProxy Play(string name, Vector3 position, System.Action<GenericParams> callback, GenericParams parms) {
            SoundData dat;
            if(!mSfx.TryGetValue(name, out dat)) {
                Debug.LogWarning("Sound does not exist: " + name);
                return null;
            }

            var src = GetAvailable();
            if(src) {
                src.Play(dat.clip, dat.spatialBlend, position, delegate (GenericParams _parms) {
                    Cache(src); //add back

                    if(callback != null)
                        callback(_parms);
                }, parms);
            }

            return src;
        }

        /// <summary>
        /// Call this with the source returned by Play, normally use this for looping clips
        /// </summary>
        public void Stop(AudioSourceProxy src) {
            src.Stop();
            Cache(src);
        }
        
        protected override void OnInstanceInit() {
            //generate lookups
            mSfx = new Dictionary<string, SoundData>();
            for(int i = 0; i < sounds.Length; i++)
                mSfx.Add(sounds[i].name, sounds[i]);

            //generate sources
            if(max <= 0)
                max = sounds.Length;

            mSourceRootGO = new GameObject("soundPlaylist");
            DontDestroyOnLoad(mSourceRootGO);

            mSourceCache = new CacheList<AudioSourceProxy>(max);
            for(int i = 0; i < max; i++) {
                var go = new GameObject("source " + i);
                go.transform.SetParent(mSourceRootGO.transform);

                var audioSrc = go.AddComponent<AudioSource>();
                audioSrc.outputAudioMixerGroup = audioMixer;
                audioSrc.playOnAwake = false;

                var srcProxy = go.AddComponent<AudioSourceProxy>();
                srcProxy.settingsSource = AudioSourceProxy.VolumeSourceType.Sound;
                srcProxy.audioSource = audioSrc;
                srcProxy.muteOnFocusLost = muteOnFocusLost;

                mSourceCache.Add(srcProxy);
            }
        }

        private AudioSourceProxy GetAvailable() {
            if(mSourceCache == null)
                return null;

            if(!mSourceCache.IsFull) {
                return mSourceCache.RemoveLast();
            }
            else {
                Debug.LogWarning("No available source.");
                return null;
            }
        }

        private void Cache(AudioSourceProxy src) {
            if(mSourceCache == null)
                return;

            mSourceCache.Add(src);
        }
    }
}
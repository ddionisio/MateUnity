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
            public AudioClip[] clipVariants;
            public float spatialBlend = 1f; //use for playing 3D

            public AudioClip GetClip() {
                if(clipVariants.Length > 0) {
                    if(clip) {
                        int ind = Random.Range(0, clipVariants.Length + 1);
                        return ind == 0 ? clip : clipVariants[ind - 1];
                    }
                    else
                        return clipVariants[Random.Range(0, clipVariants.Length)];
                }
                else
                    return clip;
            }
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
                    src.Play2D(dat.GetClip(), true);

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

            var clip = dat.GetClip();

            if(clip == null) {
                Debug.LogWarning(string.Format("Sound '{0}' does not have a clip.", name));
                return null;
            }

            var src = GetAvailable();
            if(src) {
                src.Play2D(clip, delegate (GenericParams _parms) {
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
                    src.Play(dat.GetClip(), dat.spatialBlend, follow, true);

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
                src.Play(dat.GetClip(), dat.spatialBlend, follow, delegate (GenericParams _parms) {
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
                    src.Play(dat.GetClip(), dat.spatialBlend, position, true);

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
                src.Play(dat.GetClip(), dat.spatialBlend, position, delegate (GenericParams _parms) {
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

        public void SetupSourceRoot(GameObject sourceGO) {
            if(sourceGO)
                mSourceRootGO = sourceGO;
            else {
                mSourceRootGO = new GameObject("soundPlaylist");
                DontDestroyOnLoad(mSourceRootGO);
            }

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

                go.SetActive(false);

                mSourceCache.Add(srcProxy);
            }
        }
        
        protected override void OnInstanceInit() {
            //generate lookups
            mSfx = new Dictionary<string, SoundData>();
            for(int i = 0; i < sounds.Length; i++)
                mSfx.Add(sounds[i].name, sounds[i]);

            if(max <= 0)
                max = sounds.Length;
        }

        private AudioSourceProxy GetAvailable() {
            if(mSourceCache == null)
                SetupSourceRoot(null);

            if(mSourceCache.Count > 0) {
                var src = mSourceCache.RemoveLast();
                src.gameObject.SetActive(true);
                return src;
            }
            else {
                Debug.LogWarning("No available source.");
                return null;
            }
        }

        private void Cache(AudioSourceProxy src) {
            if(mSourceCache == null)
                SetupSourceRoot(null);

            src.gameObject.SetActive(false);

            mSourceCache.Add(src);
        }
    }
}
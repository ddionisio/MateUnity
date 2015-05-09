using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [PrefabCore]
    [AddComponentMenu("M8/Audio/SoundPlayerGlobal")]
    public class SoundPlayerGlobal : SingletonBehaviour<SoundPlayerGlobal> {
        public delegate void OnSoundEnd(object param);

        [System.Serializable]
        public class SoundData {
            public string name;
            public AudioClip clip;
            public float delay;
            public float volume = 1.0f;
            public bool loop;
            public bool realtime;
        }

        public SoundData[] sfx;

        public int max = 10;

        private Dictionary<string, SoundData> mSfx;

        private CacheList<SoundPlayer> mAvailable;

        public SoundPlayer Play(string name, OnSoundEnd onEndCallback = null, params object[] onEndParam) {
            return Play(name, Vector3.zero, onEndCallback, onEndParam);
        }

        public SoundPlayer Play(string name, Vector3 position, OnSoundEnd onEndCallback = null, params object[] onEndParam) {
            SoundData dat;

            SoundPlayer ret = null;

            if(mSfx.TryGetValue(name, out dat)) {
                ret = GetAvailable();
                if(ret != null) {
                    ret.transform.position = position;

                    ret.target.clip = dat.clip;
                    ret.target.volume = dat.volume;
                    ret.target.loop = dat.loop;

                    ret.defaultVolume = dat.volume;
                    ret.playDelay = dat.delay;

                    if(ret.target.loop)
                        ret.Play();
                    else
                        ret.StartCoroutine(DoSoundPlay(ret, onEndCallback, onEndParam));

                }
                /*else {
                    Debug.LogWarning("Ran out of available sound player for: " + name);
                }*/
            }
            else {
                Debug.LogWarning("sound player not found: " + name);
            }

            return ret;
        }

        SoundPlayer GetAvailable() {
            SoundPlayer ret = mAvailable.Remove();

            if(ret)
                ret.gameObject.SetActive(true);

            return ret;
        }

        /// <summary>
        /// Call this with SoundPlayer returned by Play, normally use this for looping clip
        /// </summary>
        public void Stop(SoundPlayer sp) {
            //in case parent is set elsewhere
            sp.transform.SetParent(transform, false);

            sp.gameObject.SetActive(false);

            mAvailable.Add(sp);
        }

        IEnumerator DoSoundPlay(SoundPlayer sp, OnSoundEnd endCallback, params object[] endParam) {
            sp.Play();

            while(sp.target.isPlaying)
                yield return null;

            Stop(sp);

            if(endCallback != null)
                endCallback(endParam);
        }

        protected override void OnInstanceInit() {
            if(max <= 0)
                max = sfx.Length;

            mSfx = new Dictionary<string, SoundData>(sfx.Length);
            foreach(SoundData sd in sfx)
                mSfx.Add(sd.name, sd);

            //generate pool
            mAvailable = new CacheList<SoundPlayer>(max);
            for(int i = 0; i < max; i++)
                mAvailable.Add(CreateSource(i));
        }

        private SoundPlayer CreateSource(int ind) {
            GameObject go = new GameObject(ind.ToString(), typeof(AudioSource), typeof(SoundPlayer));
            go.transform.SetParent(transform, false);

            SoundPlayer sp = go.GetComponent<SoundPlayer>();

            go.SetActive(false);

            return sp;
        }
    }
}
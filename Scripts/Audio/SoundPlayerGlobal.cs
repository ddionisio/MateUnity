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

        private struct SoundPlaying {
            public SoundData data;
            public SoundPlayer player;
            public float startTime;
            public OnSoundEnd onEndCallback;
            public object onEndParam;
        }

        private Dictionary<string, SoundData> mSfx;

        private List<SoundPlaying> mSfxPlaying;

        private int mNextId = 0;

        public GameObject Play(string name, OnSoundEnd onEndCallback = null, object onEndParam = null) {
            SoundData dat;

            GameObject ret = null;

            if(mSfx.TryGetValue(name, out dat)) {
                ret = GetAvailable();
                if(ret != null) {
                    ret.audio.clip = dat.clip;
                    ret.audio.volume = dat.volume;
                    ret.audio.loop = dat.loop;

                    SoundPlayer sp = ret.GetComponent<SoundPlayer>();
                    sp.playOnActive = false;
                    sp.defaultVolume = dat.volume;
                    sp.playDelay = dat.delay;

                    sp.Play();

                    SoundPlaying newPlay = new SoundPlaying() {
                        data = dat,
                        player = sp,
                        startTime = dat.realtime ? Time.realtimeSinceStartup : Time.time,
                        onEndCallback = onEndCallback,
                        onEndParam = onEndParam
                    };

                    mSfxPlaying.Add(newPlay);

                    //   sp.StartCoroutine(OnSoundPlayFinish(dat.clip.length + dat.delay, ret, onEndCallback, onEndParam));

                    ret.SetActive(true);
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

        GameObject GetAvailable() {
            GameObject ret = null;

            Transform thisT = transform;

            for(int i = 0; i < max; i++) {
                Transform t = thisT.GetChild(mNextId);
                GameObject go = t.gameObject;

                if(!go.activeSelf) {
                    ret = go;
                    ret.SetActive(true);

                    mNextId++; if(mNextId == max) mNextId = 0;
                    break;
                }
                else {
                    mNextId++; if(mNextId == max) mNextId = 0;
                }
            }

            return ret;
        }

        /// <summary>
        /// Call this with GameObject returned by Play, normally use this for looping clip
        /// </summary>
        public void Stop(GameObject go) {
            //in case parent is set elsewhere
            if(go.transform.parent != transform)
                go.transform.parent = transform;

            go.SetActive(false);
        }

        IEnumerator OnSoundPlayFinish(float delay, GameObject go, OnSoundEnd endCallback, object endParam) {
            yield return new WaitForSeconds(delay);

            Stop(go);

            if(endCallback != null)
                endCallback(endParam);
        }

        protected override void Awake() {
            base.Awake();

            if(max <= 0)
                max = sfx.Length;

            mSfx = new Dictionary<string, SoundData>(sfx.Length);
            foreach(SoundData sd in sfx)
                mSfx.Add(sd.name, sd);

            //generate pool
            for(int i = 0; i < max; i++) {
                CreateSource(i);
            }

            mSfxPlaying = new List<SoundPlaying>(max);

            mNextId = 0;
        }

        void Update() {
            for(int i = 0, max = mSfxPlaying.Count; i < max; i++) {
                SoundPlaying playing = mSfxPlaying[i];

                bool isDone = false;

                if(!playing.player.gameObject.activeSelf) {
                    isDone = true;
                }
                else if(!playing.data.loop) {
                    float duration = playing.data.clip.length + playing.data.delay;

                    if(playing.data.realtime) {
                        isDone = Time.realtimeSinceStartup - playing.startTime >= duration;
                    }
                    else {
                        isDone = Time.time - playing.startTime >= duration;
                    }
                }

                if(isDone) {
                    mSfxPlaying.RemoveAt(i);
                    max = mSfxPlaying.Count;
                    i--;

                    Stop(playing.player.gameObject);

                    if(playing.onEndCallback != null)
                        playing.onEndCallback(playing.onEndParam);
                }
            }
        }

        private GameObject CreateSource(int ind) {
            GameObject go = new GameObject(ind.ToString(), typeof(AudioSource), typeof(SoundPlayer));
            go.transform.parent = transform;

            go.SetActive(false);

            return go;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Play a sound based on index id, pair this with an entity component.
    /// </summary>
    [AddComponentMenu("M8/Audio/SoundPlayerMulti")]
    public class SoundPlayerMulti : MonoBehaviour {
        public const float refRate = 44100.0f;

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
        public int max = 4;

        private class SoundPlaying {
            public SoundData data;
            public AudioSource src;
            public float startTime;
            public OnSoundEnd onEndCallback;
            public object onEndParam;
        }

        private Dictionary<string, SoundData> mDataLookup;
        private SoundPlaying[] mSfxPlaying;
        private int mSfxPlayingCount;
        private Transform mHolder;
        private bool mSoundActive;

        public AudioSource Play(int index, OnSoundEnd onEndCallback = null, object onEndParam = null) {
            return Play(sfx[index], onEndCallback, onEndParam);
        }

        public AudioSource Play(string name, OnSoundEnd onEndCallback = null, object onEndParam = null) {
            SoundData dat;
            if(mDataLookup.TryGetValue(name, out dat)) {
                return Play(dat, onEndCallback, onEndParam);
            }
            else
                Debug.LogWarning("Unknown sound: "+name);

            return null;
        }

        AudioSource Play(SoundData dat, OnSoundEnd onEndCallback, object onEndParam) {
            AudioSource ret = null;

            if(mSfxPlayingCount < sfx.Length) {
                SoundPlaying plyr = mSfxPlaying[mSfxPlayingCount];
                mSfxPlayingCount++;

                plyr.data = dat;
                plyr.src.clip = dat.clip;
                plyr.startTime = dat.realtime ? Time.realtimeSinceStartup : Time.time;
                plyr.onEndCallback = onEndCallback;
                plyr.onEndParam = onEndParam;
                plyr.src.volume = dat.volume * UserSettingAudio.instance.soundVolume;
                plyr.src.loop = dat.loop;

                if(dat.delay > 0.0f)
                    audio.PlayDelayed(dat.delay);
                else
                    audio.Play();

                if(!mSoundActive)
                    StartCoroutine(DoSounds());
            }
            else {
                Debug.LogWarning("Ran out of available player for: "+dat.name);
            }

            return ret;
        }

        /// <summary>
        /// Call this with GameObject returned by Play, normally use this for looping clip
        /// </summary>
        public void Stop(AudioSource a) {
            a.Stop();

            //in case parent is set elsewhere
            if(a.transform.parent != mHolder)
                a.transform.parent = mHolder;

            int ind = -1;
            for(int i = 0; i < mSfxPlayingCount; i++) {
                if(mSfxPlaying[i].src == a) {
                    ind = i;
                    break;
                }
            }

            if(ind != -1) {
                SoundPlaying prev = mSfxPlaying[ind];

                a.gameObject.SetActive(false);

                if(mSfxPlayingCount > 1) {
                    mSfxPlaying[ind] = mSfxPlaying[mSfxPlayingCount-1];
                    mSfxPlaying[mSfxPlayingCount-1] = prev;
                }
                else {
                    mSfxPlayingCount = 0;
                }

                if(prev.onEndCallback != null)
                    prev.onEndCallback(prev.onEndParam);
            }
        }

        void OnDisable() {
            for(int i = 0; i < mSfxPlayingCount; i++) {
                SoundPlaying player = mSfxPlaying[i];

                //in case parent is set elsewhere
                if(player.src.transform.parent != mHolder)
                    player.src.transform.parent = mHolder;

                player.src.Stop();
                player.src.gameObject.SetActive(false);
            }

            mSfxPlayingCount = 0;

            StopAllCoroutines();
        }

        void OnDestroy() {
            if(UserSettingAudio.instance)
                UserSettingAudio.instance.changeCallback -= UserSettingsChanged;
        }

        void Awake() {
            if(max <= 0)
                max = sfx.Length;

            mDataLookup = new Dictionary<string, SoundData>(sfx.Length);
            mSfxPlaying = new SoundPlaying[max];

            GameObject holderGO = new GameObject("_sfx");
            mHolder = holderGO.transform;
            mHolder.parent = transform;

            //look-up
            for(int i = 0; i < sfx.Length; i++)
                mDataLookup.Add(sfx[i].name, sfx[i]);

            //generate pool
            for(int i = 0; i < max; i++) {
                GameObject go = new GameObject(i.ToString());
                go.transform.parent = mHolder;

                mSfxPlaying[i] = new SoundPlaying();
                mSfxPlaying[i].src = go.AddComponent(typeof(AudioSource)) as AudioSource;

                go.SetActive(false);
            }

            UserSettingAudio.instance.changeCallback += UserSettingsChanged;
        }

        IEnumerator DoSounds() {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            mSoundActive = true;

            while(mSfxPlayingCount > 0) {
                for(int i = 0; i < mSfxPlayingCount; i++) {
                    SoundPlaying playing = mSfxPlaying[i];

                    bool isDone = false;

                    if(!playing.src.gameObject.activeSelf) {
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
                        playing.src.gameObject.SetActive(false);

                        if(mSfxPlayingCount > 1) {
                            mSfxPlaying[i] = mSfxPlaying[mSfxPlayingCount-1];
                            mSfxPlaying[mSfxPlayingCount-1] = playing;
                            i--;
                        }
                        else {
                            mSfxPlayingCount = 0;
                        }

                        if(playing.onEndCallback != null)
                            playing.onEndCallback(playing.onEndParam);
                    }
                }

                yield return wait;
            }

            mSoundActive = false;
        }

        void UserSettingsChanged(UserSettingAudio us) {
            //if(audio.isPlaying)
            for(int i = 0; i < mSfxPlayingCount; i++) {
                SoundPlaying player = mSfxPlaying[i];
                player.src.volume = player.data.volume * us.soundVolume;
            }
        }
    }
}
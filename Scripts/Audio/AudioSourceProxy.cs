using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("")]
    public class AudioSourceProxy : MonoBehaviour {
        public enum VolumeSourceType {
            Sound,
            Music
        }
        
        public VolumeSourceType settingsSource;

        public float volumeScale = 1f;
        public AudioSource audioSource;
        public bool muteOnFocusLost;

        public float volume {
            get {
                if(!muteOnFocusLost || mIsFocused) {
                    if(UserSettingAudio.isInstantiated) {
                        switch(settingsSource) {
                            case VolumeSourceType.Sound:
                                return volumeScale * UserSettingAudio.instance.soundVolume;
                            case VolumeSourceType.Music:
                                return volumeScale * UserSettingAudio.instance.musicVolume;
                            default:
                                return volumeScale;
                        }
                    }
                    else
                        return volumeScale;
                }
                else
                    return 0f;
            }
        }

        public bool isPlaying {
            get { return audioSource && audioSource.isPlaying; }
        }

        private Coroutine mRout;
        private bool mIsFocused;

        public void Play(AudioClip clip, float spatialBlend, Transform follow, bool loop) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.loop = loop;

            mRout = StartCoroutine(DoPlayFollow(follow));
        }

        public void Play(AudioClip clip, float spatialBlend, Transform follow, System.Action<GenericParams> callback, GenericParams parms) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.loop = false;

            mRout = StartCoroutine(DoPlayFollowCallback(follow, callback, parms));
        }

        public void Play(AudioClip clip, float spatialBlend, Vector3 position, System.Action<GenericParams> callback, GenericParams parms) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.loop = false;

            transform.position = position;

            mRout = StartCoroutine(DoPlayCallback(callback, parms));
        }

        public void Play(AudioClip clip, float spatialBlend, Vector3 position, bool loop) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = spatialBlend;
            audioSource.loop = loop;

            transform.position = position;

            audioSource.Play();
        }

        public void Play2D(AudioClip clip, System.Action<GenericParams> callback, GenericParams parms) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            audioSource.Stop();

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.spatialBlend = 0f;
            audioSource.loop = false;

            mRout = StartCoroutine(DoPlayCallback(callback, parms));
        }

        public void Play2D(AudioClip clip, bool loop) {
            Play2DFade(clip, loop, 0f, 0f);
        }
        
        public void Play2DFade(AudioClip clip, bool loop, float fadeOutDelay, float fadeInDelay) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            if(fadeOutDelay <= 0f && fadeInDelay <= 0f) {
                audioSource.Stop();

                audioSource.clip = clip;
                audioSource.volume = volume;
                audioSource.spatialBlend = 0f;
                audioSource.loop = loop;

                audioSource.Play();
            }
            else
                mRout = StartCoroutine(DoPlayFadeInOut(clip, loop, fadeOutDelay, fadeInDelay));
        }

        public void Stop() {
            StopFade(0f);
        }

        public void StopFade(float fadeOutDelay) {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            if(!audioSource)
                return;

            if(fadeOutDelay <= 0f)
                audioSource.Stop();
            else
                mRout = StartCoroutine(DoStopFadeOut(fadeOutDelay));
        }

        void OnApplicationFocus(bool focus) {
            if(!muteOnFocusLost || !audioSource)
                return;

            mIsFocused = focus;

            audioSource.volume = volume;
        }

        void OnDisable() {
            if(UserSettingAudio.instance)
                UserSettingAudio.instance.changeCallback -= UserSettingsChanged;

            if(audioSource)
                audioSource.Stop();

            mRout = null;
        }

        void OnEnable() {
            mIsFocused = Application.isFocused;

            if(UserSettingAudio.instance)
                UserSettingAudio.instance.changeCallback += UserSettingsChanged;
        }
        
        void UserSettingsChanged(UserSettingAudio us) {
            if(audioSource)
                audioSource.volume = volume;
        }

        IEnumerator DoPlayFollow(Transform follow) {
            audioSource.Play();

            var t = transform;

            while(audioSource.isPlaying) {
                if(follow && follow.gameObject.activeInHierarchy)
                    t.position = follow.position;

                yield return null;
            }
        }

        IEnumerator DoPlayFollowCallback(Transform follow, System.Action<GenericParams> callback, GenericParams parms) {
            audioSource.Play();

            var t = transform;

            while(audioSource.isPlaying) {
                if(follow && follow.gameObject.activeInHierarchy)
                    t.position = follow.position;

                yield return null;
            }

            if(callback != null)
                callback(parms);
        }

        IEnumerator DoPlayCallback(System.Action<GenericParams> callback, GenericParams parms) {
            audioSource.Play();

            while(audioSource.isPlaying)
                yield return null;

            if(callback != null)
                callback(parms);
        }

        IEnumerator DoPlayFadeInOut(AudioClip nextClip, bool loop, float fadeOutDelay, float fadeInDelay) {
            float curTime;

            if(audioSource.isPlaying) {
                curTime = 0f;
                while(curTime < fadeOutDelay) {
                    yield return null;

                    curTime += Time.deltaTime;

                    var t = Mathf.Clamp01(curTime / fadeOutDelay);

                    audioSource.volume = volume * (1.0f - t);
                }

                audioSource.Stop();
            }

            if(nextClip) {
                audioSource.clip = nextClip;
                audioSource.loop = loop;
                
                audioSource.Play();

                if(fadeInDelay > 0f) {
                    audioSource.volume = 0f;

                    curTime = 0f;
                    while(curTime < fadeInDelay) {
                        yield return null;

                        curTime += Time.deltaTime;

                        var t = Mathf.Clamp01(curTime / fadeInDelay);

                        audioSource.volume = volume * t;
                    }
                }
                else
                    audioSource.volume = volume;
            }

            mRout = null;
        }

        IEnumerator DoStopFadeOut(float fadeOutDelay) {

            var curTime = 0f;
            while(curTime < fadeOutDelay) {
                yield return null;

                curTime += Time.deltaTime;

                var t = Mathf.Clamp01(curTime / fadeOutDelay);

                audioSource.volume = volume * (1.0f - t);
            }

            audioSource.Stop();

            mRout = null;
        }
    }
}
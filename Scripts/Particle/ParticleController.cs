using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Particle/Controller")]
    public class ParticleController : MonoBehaviour {
        public bool playOnEnable;
        public float playOnEnableDelay = 0.1f;

        public bool stopOnDisable;
        public bool clearOnStop;

        private bool mStarted;

        private ParticleSystem mPS;

        public void Play(bool withChildren) {
            if(!mPS.isPlaying)
                mPS.Play(withChildren);
        }

        public void PlayLoop(bool withChildren) {
            SetLoop(true);

            if(!mPS.isPlaying)
                mPS.Play(withChildren);
        }

        public void Stop() {
            mPS.Stop();
        }

        public void Pause() {
            mPS.Pause();
        }

        public void SetLoop(bool loop) {
            var main = mPS.main;
            main.loop = loop;
        }

        void OnEnable() {
            if(mStarted && playOnEnable && !mPS.isPlaying) {
                mPS.Clear();

                if(playOnEnableDelay > 0.0f)
                    Invoke("DoPlay", playOnEnableDelay);
                else
                    DoPlay();
            }

        }

        void DoPlay() {
            mPS.Play();
        }

        void OnDisable() {
            CancelInvoke();

            if(mStarted && stopOnDisable) {
                mPS.Stop();

                if(clearOnStop)
                    mPS.Clear();
            }
        }

        void Awake() {
            mPS = GetComponent<ParticleSystem>();
        }

        void Start() {
            mStarted = true;
            OnEnable();
        }
    }
}
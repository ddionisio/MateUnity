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

        public void Play(bool withChildren) {
            particleSystem.Play(withChildren);
        }

        public void Stop() {
            particleSystem.Stop();
        }

        public void Pause() {
            particleSystem.Pause();
        }

        public void SetLoop(bool loop) {
            particleSystem.loop = loop;
        }

        void OnEnable() {
            if(mStarted && playOnEnable && !particleSystem.isPlaying) {
                particleSystem.Clear();

                if(playOnEnableDelay > 0.0f)
                    Invoke("DoPlay", playOnEnableDelay);
                else
                    DoPlay();
            }

        }

        void DoPlay() {
            particleSystem.Play();
        }

        void OnDisable() {
            CancelInvoke();

            if(mStarted && stopOnDisable) {
                particleSystem.Stop();

                if(clearOnStop)
                    particleSystem.Clear();
            }
        }

        void Start() {
            mStarted = true;
            OnEnable();
        }
    }
}
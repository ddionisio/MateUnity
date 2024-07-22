using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Particle/Controller")]
    public class ParticleController : MonoBehaviour {
        public ParticleSystem target;

        [SerializeField]
        bool _playOnEnable;
		[SerializeField]
		bool _stopOnDisable;
        [SerializeField]
        bool _disableOnEnd;

        [Header("Controls")]
        [SerializeField]
        bool _loop;

        public bool loop {
            get { return _loop; }
            set {
                if(_loop != value) {
                    _loop = value;

                    var main = target.main;
                    main.loop = _loop;
                }
            }
        }

        public void Play(bool withChildren) {
            target.Play(withChildren);
        }

        public void PlayLoop(bool withChildren) {
            target.Play(withChildren);

            loop = true;
        }

        public void Stop() {
			target.Stop();
        }

        public void Pause() {
            target.Pause();
        }

        public void Refresh() {
			var main = target.main;

			main.loop = _loop;
		}

		void OnDidApplyAnimationProperties() {
            Refresh();
		}

		void OnEnable() {
            Refresh();

			if(_playOnEnable)
                target.Play();
        }
                
        void OnDisable() {
            if(target) {
                if(_stopOnDisable)
					target.Stop();
            }
        }

		void Update() {
			if(_disableOnEnd) {
                if(!target.isPlaying)
                    enabled = false;
            }
		}

		void Awake() {
			if(!target) target = GetComponent<ParticleSystem>();
		}
	}
}
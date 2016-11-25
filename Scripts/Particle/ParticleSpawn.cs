using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Use this with PoolController
    /// </summary>
    [AddComponentMenu("M8/Particle/Spawn")]
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleSpawn : MonoBehaviour, IPoolSpawn, IPoolDespawn {
        public float playDelay = 0.1f;

        private ParticleSystem mParticles;
        private bool mActive = false;

        void IPoolSpawn.OnSpawned(GenericParams parms) {
            mActive = false;

            if(playDelay > 0)
                Invoke("DoPlay", playDelay);
            else
                DoPlay();
        }

        void IPoolDespawn.OnDespawned() {
            mActive = false;

            CancelInvoke();
            mParticles.Clear();
        }

        // Update is called once per frame
        void LateUpdate() {
            if(mActive && !mParticles.IsAlive())
                PoolController.ReleaseAuto(transform);
        }

        void Awake() {
            mParticles = GetComponent<ParticleSystem>();
        }

        void DoPlay() {
            mParticles.Play();
            mActive = true;
        }
    }
}
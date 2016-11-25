using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game/PoolDespawnDelay")]
    public class PoolDespawnDelay : MonoBehaviour, IPoolSpawn {
        public delegate void DespawnCall(GameObject go);

        public float delay = 1.0f;

        public event DespawnCall despawnCallback;
        
        void OnDestroy() {
            despawnCallback = null;
        }

        void IPoolSpawn.OnSpawned(GenericParams parms) {
            Invoke("DoDespawn", delay);
        }

        void DoDespawn() {
            if(despawnCallback != null)
                despawnCallback(gameObject);

            PoolController.ReleaseAuto(transform);
        }
    }
}
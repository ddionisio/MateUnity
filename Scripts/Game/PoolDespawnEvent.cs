using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    [AddComponentMenu("M8/Game/Pool Despawn Event")]
    public class PoolDespawnEvent : MonoBehaviour, IPoolDespawn {
        public UnityEvent despawnCallback;

        void IPoolDespawn.OnDespawned() {
            despawnCallback.Invoke();
        }
    }
}
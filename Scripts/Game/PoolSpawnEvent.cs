using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    [AddComponentMenu("M8/Game/PoolSpawnEvent")]
    public class PoolSpawnEvent : MonoBehaviour, IPoolSpawn {
        public UnityEventGenericParams spawnCallback;

        void IPoolSpawn.OnSpawned(GenericParams parms) {
            spawnCallback.Invoke(parms);
        }
    }
}
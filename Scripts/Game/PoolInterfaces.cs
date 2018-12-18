using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// This is called during Spawn via Pool. Generally used for initialization.
    /// </summary>
    public interface IPoolSpawn {
        void OnSpawned(GenericParams parms);
    }

    /// <summary>
    /// This is called just after OnSpawned. Generally used to start up anything initialized during OnSpawned
    /// </summary>
    public interface IPoolSpawnComplete {
        void OnSpawnComplete();
    }

    public interface IPoolDespawn {
        void OnDespawned();
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    public interface IPoolSpawn {
        void OnSpawned(GenericParams parms);
    }

    public interface IPoolDespawn {
        void OnDespawned();
    }
}
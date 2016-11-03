using UnityEngine;
using System.Collections;

namespace M8 {
    public interface IPoolSpawn {
        void OnSpawned();
    }

    public interface IPoolDespawn {
        void OnDespawned();
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Use this for items that you want to persist after they are spawned. So when you return to the scene, this item will be spawned.
    /// Make sure SceneSerializerSpawnManager exists. This object must be created via PoolController, when this object is despawned,
    /// its saved data will be removed and will no longer be spawned.
    /// Note: Id is generated upon call to OnSpawned
    /// </summary>
    [AddComponentMenu("M8/Serializer/Object Spawn")]
    public class SceneSerializerSpawnItem : SceneSerializer, IPoolSpawn, IPoolDespawn {
        void IPoolSpawn.OnSpawned(GenericParams parms) {
            SceneSerializerSpawnManager.instance.RegisterSpawn(this);
        }

        void IPoolDespawn.OnDespawned() {
            MarkRemove();
        }

        public override void MarkRemove() {
            if(id != invalidID) {
                //remove info from manager
                SceneSerializerSpawnManager.instance.UnRegisterSpawn(this);

                RemoveAllValues();

                //possible that we will be spawned again as a fresh new item
                __SetID(invalidID);
            }
        }

        protected override void Init() { }
    }
}
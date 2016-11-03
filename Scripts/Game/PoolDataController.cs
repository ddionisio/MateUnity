using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class PoolDataController : MonoBehaviour {
        [System.NonSerialized]
        public string group;

        [System.NonSerialized]
        public string factoryKey;

        /// <summary>
        /// only entity manager ought to set this.
        /// </summary>
        [System.NonSerialized]
        public bool claimed;
                
        private bool mIsInterfacesInit;
        private IPoolSpawn[] mISpawns;
        private IPoolDespawn[] mIDespawns;

        public void Spawn() {
            InitInterfaces();

            for(int i = 0; i < mISpawns.Length; i++)
                mISpawns[i].OnSpawned();
        }

        public void Despawn() {
            InitInterfaces();

            for(int i = 0; i < mIDespawns.Length; i++)
                mIDespawns[i].OnDespawned();
        }

        private void InitInterfaces() {
            if(mIsInterfacesInit)
                return;

            var comps = GetComponentsInChildren<MonoBehaviour>(true);

            var ISpawns = new List<IPoolSpawn>();
            var IDespawns = new List<IPoolDespawn>();

            for(int i = 0; i < comps.Length; i++) {
                var comp = comps[i];

                var spawn = comp as IPoolSpawn;
                if(comp != null) ISpawns.Add(spawn);

                var despawn = comp as IPoolDespawn;
                if(comp != null) IDespawns.Add(despawn);
            }

            mISpawns = ISpawns.ToArray();
            mIDespawns = IDespawns.ToArray();

            mIsInterfacesInit = true;
        }
    }
}
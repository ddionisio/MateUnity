using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class PoolDataController : MonoBehaviour {
        public string group { get; private set; }
        
        public string factoryKey { get; private set; }
        
        public bool isSpawned { get { return mIsSpawned; } }
        public bool claimed { get { return !mIsSpawned; } }

        public event System.Action<PoolDataController> spawnCallback;
        public event System.Action<PoolDataController> despawnCallback;

        private bool mIsSpawned = false;
        private bool mIsInterfacesInit;
        private IPoolSpawn[] mISpawns;
        private IPoolSpawnComplete[] mISpawnCompletes;
        private IPoolDespawn[] mIDespawns;

        public static PoolDataController Generate(string factoryKey, GameObject template, string group, Transform parent) {
            var go = Instantiate(template);
            go.name = template.name;

            go.SetActive(false);

            go.transform.SetParent(parent, false);

            PoolDataController pdc = go.GetComponent<PoolDataController>();
            if(pdc == null) {
                pdc = go.AddComponent<PoolDataController>();
            }

            pdc.group = group;
            pdc.factoryKey = !string.IsNullOrEmpty(factoryKey) ? factoryKey : template.name;
            pdc.mIsSpawned = false;

            return pdc;
        }
        
        public void Spawn(GenericParams parms) {
            mIsSpawned = true;

            InitInterfaces();
                        
            for(int i = 0; i < mISpawns.Length; i++)
                mISpawns[i].OnSpawned(parms);

            for(int i = 0; i < mISpawnCompletes.Length; i++)
                mISpawnCompletes[i].OnSpawnComplete();

            if(spawnCallback != null)
                spawnCallback(this);
        }

        public void Despawn() {
            InitInterfaces();

            if(despawnCallback != null)
                despawnCallback(this);

            for(int i = 0; i < mIDespawns.Length; i++)
                mIDespawns[i].OnDespawned();

            mIsSpawned = false;
        }

        public void Release() {
            PoolController.ReleaseAuto(this);
        }

        private void InitInterfaces() {
            if(mIsInterfacesInit)
                return;

            var comps = GetComponentsInChildren<MonoBehaviour>(true);

            var ISpawns = new List<IPoolSpawn>();
            var ISpawnCompletes = new List<IPoolSpawnComplete>();
            var IDespawns = new List<IPoolDespawn>();

            for(int i = 0; i < comps.Length; i++) {
                var comp = comps[i];

                var spawn = comp as IPoolSpawn;
                if(spawn != null) ISpawns.Add(spawn);

                var spawnComplete = comp as IPoolSpawnComplete;
                if(spawnComplete != null) ISpawnCompletes.Add(spawnComplete);

                var despawn = comp as IPoolDespawn;
                if(despawn != null) IDespawns.Add(despawn);
            }

            mISpawns = ISpawns.ToArray();
            mISpawnCompletes = ISpawnCompletes.ToArray();
            mIDespawns = IDespawns.ToArray();

            mIsInterfacesInit = true;
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class PoolDataController : MonoBehaviour {
        public string group { get; private set; }
        
        public string factoryKey { get; private set; }
        
        public bool claimed { get; private set; }
                
        private bool mIsInterfacesInit;
        private IPoolSpawn[] mISpawns;
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
            pdc.claimed = true;

            return pdc;
        }
        
        public void Spawn(GenericParams parms) {
            claimed = false;

            InitInterfaces();
                        
            for(int i = 0; i < mISpawns.Length; i++)
                mISpawns[i].OnSpawned(parms);
        }

        public void Despawn() {
            InitInterfaces();

            for(int i = 0; i < mIDespawns.Length; i++)
                mIDespawns[i].OnDespawned();

            claimed = true;
        }

        public void Release() {
            PoolController.ReleaseAuto(this);
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
                if(spawn != null) ISpawns.Add(spawn);

                var despawn = comp as IPoolDespawn;
                if(despawn != null) IDespawns.Add(despawn);
            }

            mISpawns = ISpawns.ToArray();
            mIDespawns = IDespawns.ToArray();

            mIsInterfacesInit = true;
        }
    }
}
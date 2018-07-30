using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Game/PoolController")]
    public class PoolController : MonoBehaviour {
        [System.Serializable]
        public class FactoryData {
            public string name;
            public GameObject template = null;

            public int startCapacity = 0;
            public int maxCapacity = 0;

            public Transform defaultParent = null;

            private CacheList<PoolDataController> mActives;
            private CacheList<PoolDataController> mAvailable;

            private Transform mInactiveHolder;

            private int mNameHolder = 0;
            private int mCapacity = 0;

            public int availableCount { get { return mAvailable.Count; } }

            public int capacityCount { get { return mCapacity; } }

            public int activeCount { get { return mActives.Count; } }

            public CacheList<PoolDataController> actives { get { return mActives; } }

            public void Init(string group, Transform inactiveHolder) {
                if(string.IsNullOrEmpty(name) && template) {
                    name = template.name;
                }

                this.mInactiveHolder = inactiveHolder;

                mNameHolder = 0;

                mActives = new CacheList<PoolDataController>(maxCapacity);
                mAvailable = new CacheList<PoolDataController>(maxCapacity);
                Expand(group, startCapacity);
            }

            public void Expand(string group, int num) {
                mCapacity += num;

                if(mCapacity > maxCapacity) {
                    int diff = mCapacity - maxCapacity;

                    maxCapacity = mCapacity;

                    mActives.Expand(diff);
                    mAvailable.Expand(diff);
                }

                for(int i = 0; i < num; i++) {
                    var pdc = PoolDataController.Generate(name, template, group, mInactiveHolder);
                    mAvailable.Add(pdc);
                }
            }

            public void Release(PoolDataController pdc) {
                pdc.gameObject.SetActive(false);

                Transform t = pdc.transform;

                t.SetParent(mInactiveHolder, false);
                                
                mAvailable.Add(pdc);

                mActives.Remove(pdc);
            }

            /// <summary>
            /// Only ReleaseAll and ReleaseAllByType should call this
            /// </summary>
            public void ReleaseIgnoreActiveList(PoolDataController pdc) {
                pdc.gameObject.SetActive(false);

                Transform t = pdc.transform;

                t.SetParent(mInactiveHolder, false);

                mAvailable.Add(pdc);
            }

            public PoolDataController Allocate(string group, string name, Transform parent) {
                if(mAvailable.Count == 0) {
                    if(mActives.IsFull) {
                        Debug.LogWarning(template.name + " is expanding beyond max capacity: " + maxCapacity);

                        Expand(group, maxCapacity);
                    }
                    else {
                        Expand(group, 1);
                    }
                }

                PoolDataController pdc = mAvailable[mAvailable.Count - 1];

                mAvailable.RemoveAt(mAvailable.Count - 1);
                
                Transform t = pdc.transform;
                t.name = string.IsNullOrEmpty(name) ? template.name + (mNameHolder++) : name;

                t.SetParent(parent == null ? defaultParent : parent, false);

                t.gameObject.SetActive(true);

                mActives.Add(pdc);

                return pdc;
            }

            public void DeInit() {
                for(int i = 0; i < mActives.Count; i++)
                    Destroy(mActives[i].gameObject);
                for(int i = 0; i < mAvailable.Count; i++)
                    Destroy(mAvailable[i].gameObject);

                mActives.Clear();
                mAvailable.Clear();

                mInactiveHolder = null;
            }
        }

        public string group = "";
        
        [SerializeField]
        FactoryData[] factory;

        [SerializeField]
        Transform poolHolder;

        public event System.Action<PoolDataController> spawnCallback;
        public event System.Action<PoolDataController> despawnCallback;

        private static Dictionary<string, PoolController> mControllers;

        private Dictionary<string, FactoryData> mFactory;
                
        /// <summary>
        /// Create a new pool. If given group already exists, then it will return that.
        /// Remember to add new types into this pool.
        /// </summary>
        public static PoolController CreatePool(string group) {            
            return CreatePool(group, null);
        }

        /// <summary>
        /// Create a new pool. If given group already exists, then it will return that.
        /// Remember to add new types into this pool.
        /// </summary>
        public static PoolController CreatePool(string group, Transform parent) {
            PoolController pc;
            if(mControllers == null || !mControllers.TryGetValue(group, out pc)) {
                GameObject go = new GameObject(group);
                pc = go.AddComponent<PoolController>();

                if(parent)
                    go.transform.SetParent(parent);
                //Debug.Log ( "Creating group " + group);
            }

            return pc;
        }

        /// <summary>
        /// Group name is based on the name of the game object with the PoolController component
        /// </summary>
        public static PoolController GetPool(string group) {
            PoolController ctrl = null;

            if(mControllers != null) {
                mControllers.TryGetValue(group, out ctrl);
            }

            return ctrl;
        }

        /// <summary>
        /// type is based on the name of the prefab
        /// </summary>
        public static PoolDataController SpawnFromGroup(string group, string type, string name, Transform toParent, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null)
                return pc._Spawn(type, name, toParent, null, null, parms);

            return null;
        }

        public static PoolDataController SpawnFromGroup(string group, string type, string name, Transform toParent, Vector3 position, Quaternion rotation, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null)
                return pc._Spawn(type, name, toParent, position, rotation, parms);

            return null;
        }

        public static PoolDataController SpawnFromGroup(string group, string type, string name, Transform toParent, Vector3 position, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null)
                return pc._Spawn(type, name, toParent, position, null, parms);

            return null;
        }

        public static PoolDataController SpawnFromGroup(string group, string type, string name, Transform toParent, Vector2 position, Quaternion rotation, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null)
                return pc._Spawn(type, name, toParent, new Vector3(position.x, position.y, 0f), rotation, parms);

            return null;
        }

        public static PoolDataController SpawnFromGroup(string group, string type, string name, Transform toParent, Vector2 position, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null)
                return pc._Spawn(type, name, toParent, new Vector3(position.x, position.y, 0f), null, parms);

            return null;
        }

        public static T SpawnFromGroup<T>(string group, string type, string name, Transform toParent, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                var spawned = pc._Spawn(type, name, toParent, null, null, parms);
                if(spawned)
                    return spawned.GetComponent<T>();
            }

            return default(T);
        }

        public static T SpawnFromGroup<T>(string group, string type, string name, Transform toParent, Vector3 position, Quaternion rotation, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                var spawned = pc._Spawn(type, name, toParent, position, rotation, parms);
                if(spawned)
                    return spawned.GetComponent<T>();
            }

            return default(T);
        }

        public static T SpawnFromGroup<T>(string group, string type, string name, Transform toParent, Vector3 position, GenericParams parms) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                var spawned = pc._Spawn(type, name, toParent, position, null, parms);
                if(spawned)
                    return spawned.GetComponent<T>();
            }

            return default(T);
        }
        
        public static void ReleaseAuto(Transform entity) {
            //NOTE: don't really need to destroy
            PoolDataController pdc = entity.GetComponent<PoolDataController>();
            if(pdc != null)
                ReleaseAuto(pdc);
            else
                entity.gameObject.SetActive(false);
            //Object.Destroy(entity.gameObject);
        }

        public static void ReleaseAuto(GameObject entity) {
            //NOTE: don't really need to destroy
            PoolDataController pdc = entity.GetComponent<PoolDataController>();
            if(pdc != null)
                ReleaseAuto(pdc);
            else
                entity.SetActive(false);
        }

        public static void ReleaseAuto(PoolDataController poolCtrl) {
            PoolController pc = GetPool(poolCtrl.group);
            if(pc != null)
                pc.Release(poolCtrl);
            else
                poolCtrl.gameObject.SetActive(false);
        }

        public static void ReleaseFromGroup(string group, Transform entity) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                pc.Release(entity);
            }
        }

        public static void ReleaseFromGroup(string group, PoolDataController pdc) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                pc.Release(pdc);
            }
        }

        public static void ExpandGroup(string group, string type, int amount) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                pc.Expand(type, amount);
            }
        }

        public static void DestroyAllGroup() {
            if(mControllers != null) {
                foreach(var pair in mControllers) {
                    pair.Value.ReleaseAll();
                    Object.Destroy(pair.Value.gameObject);
                }

                mControllers.Clear();
            }
        }

        public static void ReleaseAllGroup() {
            if(mControllers != null) {
                foreach(var pair in mControllers)
                    pair.Value.ReleaseAll();
            }
        }

        //////////////////////////////////////////////////////////
        // Methods


        /// <summary>
        /// Add a new type into this pool.  The type name is based on template's name.  If template
        /// already exists (via name), then type will not be added.  Returns true if successfully added.
        /// </summary>
        public bool AddType(GameObject template, int startCapacity, int maxCapacity, Transform defaultParent = null) {            
            return AddType(template.name, template, startCapacity, maxCapacity, defaultParent);
        }

        /// <summary>
        /// Add a new type into this pool.  If template already exists (via name), then type will not be added.  
        /// Returns true if successfully added.
        /// </summary>
        public bool AddType(string typeName, GameObject template, int startCapacity, int maxCapacity, Transform defaultParent = null) {
            if(mFactory.ContainsKey(typeName))
                return false;

            if(poolHolder == null) //default holder to self
                poolHolder = transform;

            FactoryData newData = new FactoryData();
            newData.name = typeName;
            newData.template = template;
            newData.startCapacity = startCapacity;
            newData.maxCapacity = maxCapacity;
            newData.defaultParent = defaultParent;

            newData.Init(group, poolHolder);

            mFactory.Add(typeName, newData);

            return true;
        }

        public void RemoveType(string type) {
            FactoryData factory;
            if(mFactory.TryGetValue(type, out factory)) {
                factory.DeInit();
                mFactory.Remove(type);
            }
        }

        /// <summary>
        /// Get the factory type of given GameObject.  If not found, returns empty string.
        /// </summary>
        public string GetFactoryType(GameObject go) {
            PoolDataController pdc = go.GetComponent<PoolDataController>();
            return pdc && pdc.group == group && mFactory.ContainsKey(pdc.factoryKey) ? pdc.factoryKey : "";
        }

        public bool IsFactoryTypeExists(string factoryKey) {
            return mFactory.ContainsKey(factoryKey);
        }

        public Transform GetDefaultParent(string type) {
            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                return dat.defaultParent;
            }
            else {
                return null;
            }
        }

        public PoolDataController Spawn(string type, string name, Transform toParent, Vector3 position, GenericParams parms) {
            return _Spawn(type, name, toParent, position, null, parms);
        }

        public PoolDataController Spawn(string type, string name, Transform toParent, Vector3 position, Quaternion rot, GenericParams parms) {
            return _Spawn(type, name, toParent, position, rot, parms);
        }

        public PoolDataController Spawn(string type, string name, Transform toParent, GenericParams parms) {
            return _Spawn(type, name, toParent, null, null, parms);
        }

        public T Spawn<T>(string type, string name, Transform toParent, Vector3 position, GenericParams parms) {
            var spawned = _Spawn(type, name, toParent, position, null, parms);
            return spawned ? spawned.GetComponent<T>() : default(T);
        }

        public T Spawn<T>(string type, string name, Transform toParent, Vector3 position, Quaternion rot, GenericParams parms) {
            var spawned = _Spawn(type, name, toParent, position, rot, parms);
            return spawned ? spawned.GetComponent<T>() : default(T);
        }

        public T Spawn<T>(string type, string name, Transform toParent, GenericParams parms) {
            var spawned = _Spawn(type, name, toParent, null, null, parms);
            return spawned ? spawned.GetComponent<T>() : default(T);
        }

        public PoolDataController Spawn(string name, Transform toParent, Vector3 position, GenericParams parms) {
            return _Spawn(null, name, toParent, position, null, parms);
        }

        public PoolDataController Spawn(string name, Transform toParent, Vector3 position, Quaternion rot, GenericParams parms) {
            return _Spawn(null, name, toParent, position, rot, parms);
        }

        public PoolDataController Spawn(string name, Transform toParent, GenericParams parms) {
            return _Spawn(null, name, toParent, null, null, parms);
        }

        public T Spawn<T>(string name, Transform toParent, Vector3 position, GenericParams parms) {
            var spawned = _Spawn(null, name, toParent, position, null, parms);
            return spawned ? spawned.GetComponent<T>() : default(T);
        }

        public T Spawn<T>(string name, Transform toParent, Vector3 position, Quaternion rot, GenericParams parms) {
            var spawned = _Spawn(null, name, toParent, position, rot, parms);
            return spawned ? spawned.GetComponent<T>() : default(T);
        }

        public T Spawn<T>(string name, Transform toParent, GenericParams parms) {
            var spawned = _Spawn(null, name, toParent, null, null, parms);
            return spawned ? spawned.GetComponent<T>() : default(T);
        }

        PoolDataController _Spawn(string type, string name, Transform toParent, Vector3? position, Quaternion? rot, GenericParams parms) {
            PoolDataController entityRet = null;

            FactoryData dat = null;

            if(string.IsNullOrEmpty(type)) {
                //this should only be used if there's just one type
                if(factory != null && factory.Length > 0)
                    dat = mFactory[factory[0].template.name];
                else if(mFactory.Count > 0) {
                    foreach(var pair in mFactory) {
                        dat = pair.Value;
                        break;
                    }
                }
            }
            else
                mFactory.TryGetValue(type, out dat);

            if(dat != null) {
                var pdc = dat.Allocate(group, name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);

                if(pdc != null) {
                    entityRet = pdc;

                    var t = entityRet.transform;

                    if(position.HasValue) t.position = position.Value;
                    if(rot.HasValue) t.rotation = rot.Value;

                    pdc.Spawn(parms);

                    if(spawnCallback != null)
                        spawnCallback(pdc);
                }
                else {
                    Debug.LogWarning("Failed to allocate type: " + type + " for: " + name);
                }
            }
            else {
                Debug.LogWarning("No such type: " + type + " attempt to allocate: " + name);
            }

            return entityRet;
        }

        public void Release(GameObject entity) {
            PoolDataController pdc = entity.GetComponent<PoolDataController>();
            if(pdc)
                Release(pdc);
            else //not in the pool, just kill it
                Object.Destroy(entity);
        }

        public void Release(Transform entity) {
            PoolDataController pdc = entity.GetComponent<PoolDataController>();
            if(pdc)
                Release(pdc);
            else //not in the pool, just kill it
                Object.Destroy(entity.gameObject);
        }

        public void Release(PoolDataController pdc) {
            if(pdc.claimed) //already claimed
                return;

            FactoryData dat;
            if(mFactory.TryGetValue(pdc.factoryKey, out dat)) {
                if(despawnCallback != null)
                    despawnCallback(pdc);

                pdc.Despawn();

                dat.Release(pdc);
            }
            else
                Debug.LogWarning("Unable to find type: "+pdc.factoryKey+" in "+group+", failed to release.");
        }

        /// <summary>
        /// Releases all currently spawned objects of all types.
        /// </summary>
        public void ReleaseAll() {
            foreach(KeyValuePair<string, FactoryData> itm in mFactory) {
                FactoryData factory = itm.Value;

                for(int i = 0; i < factory.actives.Count; i++) {
                    PoolDataController pdc = factory.actives[i];

                    if(despawnCallback != null)
                        despawnCallback(pdc);

                    pdc.Despawn();

                    factory.ReleaseIgnoreActiveList(pdc);
                }

                factory.actives.Clear();
            }
        }

        /// <summary>
        /// Release all currently spawned objects based on given type.
        /// </summary>
        public void ReleaseAllByType(string type) {
            FactoryData factory;
            if(mFactory.TryGetValue(type, out factory)) {
                for(int i = 0; i < factory.actives.Count; i++) {
                    PoolDataController pdc = factory.actives[i];

                    if(despawnCallback != null)
                        despawnCallback(pdc);

                    pdc.Despawn();

                    factory.ReleaseIgnoreActiveList(pdc);
                }

                factory.actives.Clear();
            }
            else
                Debug.LogWarning("Unable to find type: "+type);
        }

        public int GetActiveCount(string type) {
            FactoryData factory;
            if(mFactory.TryGetValue(type, out factory)) {
                return factory.activeCount;
            }

            return 0;
        }

        /// <summary>
        /// Note: please treat this list as a read-only
        /// </summary>
        public CacheList<PoolDataController> GetActiveList(string type) {
            FactoryData factory;
            if(mFactory.TryGetValue(type, out factory)) {
                return factory.actives;
            }

            return null;
        }

        public int CapacityCount(string type) {
            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                return dat.capacityCount;
            }

            return 0;
        }

        public int AvailableCount(string type) {
            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                return dat.availableCount;
            }

            return 0;
        }

        public void Expand(string type, int amount) {
            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                dat.Expand(group, amount);
            }
        }

        void OnDestroy() {
            if(mControllers != null) {
                mControllers.Remove(group);

                if(mControllers.Count == 0)
                    mControllers = null;
            }

            //Debug.Log ( "Destroying group " + group );
        }

        void Awake() {
            if(string.IsNullOrEmpty(group))
                group = name;

            if(poolHolder == null) {//create default holder
                var holderGO = new GameObject("holder");
                holderGO.SetActive(false);
                poolHolder = holderGO.transform;
                poolHolder.SetParent(transform);
            }

            if(mControllers == null) {
                mControllers = new Dictionary<string, PoolController>();
            }

            if(!mControllers.ContainsKey(group)) {
                mControllers.Add(group, this);
                                
                //generate cache and such
                if(factory != null) {
                    mFactory = new Dictionary<string, FactoryData>(factory.Length);
                    foreach(FactoryData factoryData in factory) {
                        factoryData.Init(group, poolHolder);

                        mFactory.Add(factoryData.name, factoryData);
                    }
                }
                else
                    mFactory = new Dictionary<string, FactoryData>();
            }
            else {
                Debug.LogWarning("PoolController for: " + group + " already exists!");
                DestroyImmediate(gameObject);
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Game/PoolController")]
    public class PoolController : MonoBehaviour {
        public const string OnSpawnMessage = "OnSpawned";
        public const string OnDespawnMessage = "OnDespawned";

        [System.Serializable]
        public class FactoryData {
            public Transform template = null;

            public int startCapacity = 0;
            public int maxCapacity = 0;

            public Transform defaultParent = null;

            private List<PoolDataController> mActives;
            private List<PoolDataController> mAvailable;

            private Transform mInactiveHolder;

            private int mNameHolder = 0;
            private int mCapacity = 0;

            public int availableCount { get { return mAvailable.Count; } }

            public int capacityCount { get { return mCapacity; } }

            public int activeCount { get { return mActives.Count; } }

            public List<PoolDataController> actives { get { return mActives; } }

            public void Init(string group, Transform inactiveHolder) {
                this.mInactiveHolder = inactiveHolder;

                mNameHolder = 0;

                mActives = new List<PoolDataController>(maxCapacity);
                mAvailable = new List<PoolDataController>(maxCapacity);
                Expand(group, startCapacity);
            }

            public void Expand(string group, int num) {
                mCapacity += num;

                if(mCapacity > maxCapacity)
                    maxCapacity = mCapacity;

                for(int i = 0; i < num; i++) {
                    //PoolDataController
                    Transform t = (Transform)Object.Instantiate(template);

                    t.parent = mInactiveHolder;
                    t.localPosition = Vector3.zero;

                    PoolDataController pdc = t.GetComponent<PoolDataController>();
                    if(pdc == null) {
                        pdc = t.gameObject.AddComponent<PoolDataController>();
                    }

                    pdc.group = group;
                    pdc.factoryKey = template.name;

                    mAvailable.Add(pdc);
                }
            }

            /// <summary>
            /// Only ReleaseAll and ReleaseAllByType should set removeFromActive to false
            /// </summary>
            public void Release(PoolDataController pdc, bool removeFromActive = true) {
                pdc.claimed = true;

                Transform t = pdc.transform;
                t.parent = mInactiveHolder;
                t.localPosition = Vector3.zero;

                if(removeFromActive)
                    mActives.Remove(pdc);

                mAvailable.Add(pdc);
            }

            public Transform Allocate(string group, string name, Transform parent) {
                if(mAvailable.Count == 0) {
                    if(mActives.Count + 1 > maxCapacity) {
                        Debug.LogWarning(template.name + " is expanding beyond max capacity: " + maxCapacity);

                        Expand(group, maxCapacity);
                    }
                    else {
                        Expand(group, 1);
                    }
                }

                PoolDataController pdc = mAvailable[mAvailable.Count - 1];

                mAvailable.RemoveAt(mAvailable.Count - 1);

                pdc.claimed = false;

                Transform t = pdc.transform;
                t.name = string.IsNullOrEmpty(name) ? template.name + (mNameHolder++) : name;
                t.parent = parent == null ? defaultParent : parent;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;

                t.gameObject.SetActive(true);

                mActives.Add(pdc);

                return t;
            }

            public void DeInit() {
                foreach(PoolDataController pdc in mActives)
                    Destroy(pdc.gameObject);
                foreach(PoolDataController pdc in mAvailable)
                    Destroy(pdc.gameObject);

                mInactiveHolder = null;
            }
        }

        public string group = "";

        [SerializeField]
        bool _persistent = false;

        [SerializeField]
        FactoryData[] factory;

        [SerializeField]
        Transform poolHolder;

        private static Dictionary<string, PoolController> mControllers;

        private Dictionary<string, FactoryData> mFactory;

        /// <summary>
        /// Create a new pool. If given group already exists, then it will return that.
        /// Remember to add new types into this pool.
        /// </summary>
        public static PoolController CreatePool(string group) {
            PoolController pc;
            if(mControllers == null || !mControllers.TryGetValue(group, out pc)) {
                GameObject go = new GameObject(group);
                pc = go.AddComponent<PoolController>();
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
        public static Transform Spawn(string group, string type, string name, Transform toParent) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                return pc.Spawn(type, name, toParent);
            }
            else {
                return null;
            }
        }

        public static Transform Spawn(string group, string type, string name, Transform toParent, Vector3 position, Quaternion rotation) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                return pc.Spawn(type, name, toParent, position, rotation);
            }
            else {
                return null;
            }
        }

        public static Transform Spawn(string group, string type, string name, Transform toParent, Vector3 position) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                return pc.Spawn(type, name, toParent, position);
            }
            else {
                return null;
            }
        }

        public static Transform Spawn(string group, string type, string name, Transform toParent, Vector2 position, Quaternion rotation) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                Transform t = pc.Spawn(type, name, toParent, position, rotation);
                Vector3 p = t.localPosition;
                p.z = 0.0f;
                t.localPosition = p;
                return t;
            }
            else {
                return null;
            }
        }

        public static Transform Spawn(string group, string type, string name, Transform toParent, Vector2 position) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                Transform t = pc.Spawn(type, name, toParent, position);
                Vector3 p = t.localPosition;
                p.z = 0.0f;
                t.localPosition = p;
                return t;
            }
            else {
                return null;
            }
        }

        public static void ReleaseAuto(Transform entity) {
            //NOTE: don't really need to destroy
            PoolDataController pdc = entity.GetComponent<PoolDataController>();
            if(pdc != null) {
                PoolController pc = GetPool(pdc.group);
                if(pc != null) {
                    pc.Release(entity);
                }
                else
                    entity.gameObject.SetActive(false);
                //Object.Destroy(entity.gameObject);
            }
            else
                entity.gameObject.SetActive(false);
            //Object.Destroy(entity.gameObject);
        }

        public static void ReleaseByGroup(string group, Transform entity) {
            PoolController pc = GetPool(group);
            if(pc != null) {
                pc.Release(entity);
            }
        }

        public static void Expand(string group, string type, int amount) {
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
        public bool AddType(Transform template, int startCapacity, int maxCapacity, Transform defaultParent = null) {
            if(mFactory.ContainsKey(template.name))
                return false;

            FactoryData newData = new FactoryData();
            newData.template = template;
            newData.startCapacity = startCapacity;
            newData.maxCapacity = maxCapacity;
            newData.defaultParent = defaultParent;

            newData.Init(group, poolHolder);

            mFactory.Add(template.name, newData);

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

        public Transform GetDefaultParent(string type) {
            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                return dat.defaultParent;
            }
            else {
                return null;
            }
        }

        public Transform Spawn(string type, string name, Transform toParent, Vector3 position) {
            Transform entityRet = null;

            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                entityRet = dat.Allocate(group, name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);

                if(entityRet != null) {
                    entityRet.transform.position = position;

                    entityRet.SendMessage(OnSpawnMessage, null, SendMessageOptions.DontRequireReceiver);
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

        public Transform Spawn(string type, string name, Transform toParent, Vector3 position, Quaternion rot) {
            Transform entityRet = null;

            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                entityRet = dat.Allocate(group, name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);

                if(entityRet != null) {
                    entityRet.transform.position = position;
                    entityRet.transform.rotation = rot;

                    entityRet.SendMessage(OnSpawnMessage, null, SendMessageOptions.DontRequireReceiver);
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

        public Transform Spawn(string type, string name, Transform toParent) {
            Transform entityRet = null;

            FactoryData dat;
            if(mFactory.TryGetValue(type, out dat)) {
                entityRet = dat.Allocate(group, name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);

                if(entityRet != null) {
                    entityRet.SendMessage(OnSpawnMessage, null, SendMessageOptions.DontRequireReceiver);
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

        public void Release(Transform entity) {
            entity.SendMessage(OnDespawnMessage, null, SendMessageOptions.DontRequireReceiver);

            PoolDataController pdc = entity.GetComponent<PoolDataController>();
            if(pdc) {
                FactoryData dat;
                if(mFactory.TryGetValue(pdc.factoryKey, out dat))
                    dat.Release(pdc);
                else
                    Debug.LogWarning("Unable to find type: "+pdc.factoryKey+" in "+group+", failed to release.");
            }
            else //not in the pool, just kill it
                Object.Destroy(entity.gameObject);
        }

        /// <summary>
        /// Releases all currently spawned objects of all types.
        /// </summary>
        public void ReleaseAll() {
            foreach(KeyValuePair<string, FactoryData> itm in mFactory) {
                FactoryData factory = itm.Value;

                for(int i = 0; i < factory.actives.Count; i++) {
                    PoolDataController pdc = factory.actives[i];

                    pdc.SendMessage(OnDespawnMessage, null, SendMessageOptions.DontRequireReceiver);

                    factory.Release(pdc, false);
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

                    pdc.SendMessage(OnDespawnMessage, null, SendMessageOptions.DontRequireReceiver);

                    factory.Release(pdc, false);
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
        }

        void Awake() {
            if(string.IsNullOrEmpty(group))
                group = name;

            if(mControllers == null) {
                mControllers = new Dictionary<string, PoolController>();
            }

            if(!mControllers.ContainsKey(group)) {
                mControllers.Add(group, this);

                if(_persistent)
                    DontDestroyOnLoad(gameObject);

                if(poolHolder == null) {
                    GameObject holderGO = new GameObject("holder");
                    poolHolder = holderGO.transform;
                    poolHolder.parent = transform;
                }

                poolHolder.gameObject.SetActive(false);

                //generate cache and such
                if(factory != null) {
                    mFactory = new Dictionary<string, FactoryData>(factory.Length);
                    foreach(FactoryData factoryData in factory) {
                        factoryData.Init(group, poolHolder);

                        mFactory.Add(factoryData.template.name, factoryData);
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
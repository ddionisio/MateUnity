using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Game/PoolController")]
public class PoolController : MonoBehaviour {
    [System.Serializable]
    public class FactoryData {
        public Transform template = null;

        public int startCapacity = 0;
        public int maxCapacity = 0;

        public Transform defaultParent = null;

        private List<Transform> available;

        private int allocateCounter = 0;

        private Transform poolHolder;

        private int nameCounter = 0;
        private int capacity = 0;

        public int availableCount { get { return available.Count; } }

        public int capacityCount { get { return capacity; } }

        public void Init(string group, Transform poolHolder) {
            this.poolHolder = poolHolder;

            nameCounter = 0;

            available = new List<Transform>(maxCapacity);
            Expand(group, startCapacity);
        }

        public void Expand(string group, int num) {
            capacity += num;

            if(capacity > maxCapacity)
                maxCapacity = capacity;

            for(int i = 0; i < num; i++) {
                //PoolDataController
                Transform t = (Transform)Object.Instantiate(template);

                t.parent = poolHolder;
                t.localPosition = Vector3.zero;

                PoolDataController pdc = t.GetComponent<PoolDataController>();
                if(pdc == null) {
                    pdc = t.gameObject.AddComponent<PoolDataController>();
                }

                pdc.group = group;
                pdc.factoryKey = template.name;

                available.Add(t);
            }
        }

        public void Release(Transform t) {
            t.parent = poolHolder;
            t.localPosition = Vector3.zero;

            available.Add(t);
            allocateCounter--;
        }

        public Transform Allocate(string group, string name, Transform parent) {
            if(available.Count == 0) {
                if(allocateCounter + 1 > maxCapacity) {
                    Debug.LogWarning(template.name + " is expanding beyond max capacity: " + maxCapacity);

                    Expand(group, maxCapacity);
                }
                else {
                    Expand(group, 1);
                }
            }

            Transform t = available[available.Count - 1];

            available.RemoveAt(available.Count - 1);

            t.GetComponent<PoolDataController>().claimed = false;

            t.name = string.IsNullOrEmpty(name) ? template.name + (nameCounter++) : name;
            t.parent = parent == null ? defaultParent : parent;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            t.gameObject.SetActive(true);

            allocateCounter++;

            return t;
        }

        public void DeInit() {
            available.Clear();

            poolHolder = null;

            allocateCounter = 0;
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
    public static Transform Spawn(string group, string type, string name, Transform toParent, string waypoint) {
        PoolController pc = GetPool(group);
        if(pc != null) {
            return pc.Spawn(type, name, toParent, waypoint);
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

    public FactoryData[] types {
        get { return factory; }
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

    /// <summary>
    /// type is based on the name of the prefab, if toParent is null, then set parent to us or factory's default
    /// </summary>
    public Transform Spawn(string type, string name, Transform toParent, string waypoint) {
        Vector3 position = Vector3.zero;
        Quaternion rot = Quaternion.identity;

        if(!string.IsNullOrEmpty(waypoint)) {
            if(WaypointManager.instance != null) {
                Transform wp = WaypointManager.instance.GetWaypoint(waypoint);
                if(wp != null) {
                    position = wp.position;
                    rot = wp.rotation;
                }
            }
            else {
                Debug.LogWarning("Waypoint Manager is not present, trying to hook-up with: " + waypoint);
            }
        }

        return Spawn(type, name, toParent, position, rot);
    }

    public Transform Spawn(string type, string name, Transform toParent, Vector3 position, Quaternion rot) {
        Transform entityRet = null;

        FactoryData dat;
        if(mFactory.TryGetValue(type, out dat)) {
            entityRet = dat.Allocate(group, name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);

            if(entityRet != null) {
                entityRet.transform.position = position;
                entityRet.transform.rotation = rot;

                entityRet.SendMessage("OnSpawned", null, SendMessageOptions.DontRequireReceiver);
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
        PoolDataController pdc = entity.GetComponent<PoolDataController>();
        if(pdc != null) {
            FactoryData dat;
            if(mFactory.TryGetValue(pdc.factoryKey, out dat)) {
                pdc.claimed = true;

                entity.SendMessage("OnDespawned", null, SendMessageOptions.DontRequireReceiver);

                dat.Release(entity);
            }
        }
        else { //not in the pool, just kill it
            Object.Destroy(entity.gameObject);
            //StartCoroutine(DestroyEntityDelay(entity.gameObject));
        }
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

    /*IEnumerator DestroyEntityDelay(GameObject go) {
        yield return new WaitForFixedUpdate();

        Object.Destroy(go);

        yield break;
    }*/

    void OnDestroy() {
        if(mControllers != null) {
            foreach(FactoryData dat in mFactory.Values) {
                dat.DeInit();
            }

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

            poolHolder.gameObject.SetActive(false);

            //generate cache and such
            mFactory = new Dictionary<string, FactoryData>(factory.Length);
            foreach(FactoryData factoryData in factory) {
                factoryData.Init(group, poolHolder);

                mFactory.Add(factoryData.template.name, factoryData);
            }
        }
        else {
            Debug.LogWarning("PoolController for: " + group + " already exists!");
            DestroyImmediate(gameObject);
        }
    }
}

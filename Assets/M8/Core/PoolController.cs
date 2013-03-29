using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("M8/Game/PoolController")]
class PoolController : MonoBehaviour {
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

        public void Init(Transform poolHolder) {
            this.poolHolder = poolHolder;

            nameCounter = 0;

            available = new List<Transform>(maxCapacity);
            Expand(startCapacity);
        }

        public void Expand(int num) {
            for(int i = 0; i < num; i++) {
                //PoolDataController
                Transform t = (Transform)Object.Instantiate(template);

                t.parent = poolHolder;
                t.localPosition = Vector3.zero;

                PoolDataController pdc = t.GetComponent<PoolDataController>();
                if(pdc == null) {
                    pdc = t.gameObject.AddComponent<PoolDataController>();
                }

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

        public Transform Allocate(string name, Transform parent) {
            if(available.Count == 0) {
                if(allocateCounter + 1 > maxCapacity) {
                    Debug.LogWarning(template.name + " is expanding beyond max capacity: " + maxCapacity);

                    Expand(maxCapacity);
                }
                else {
                    Expand(1);
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

    public static void Release(string group, Transform entity) {
        PoolController pc = GetPool(group);
        if(pc != null) {
            pc.Release(entity);
        }
    }

    /// <summary>
    /// type is based on the name of the prefab, if toParent is null, then set parent to us or factory's default
    /// </summary>
    public Transform Spawn(string type, string name, Transform toParent, string waypoint) {
        Transform entityRet = null;

        FactoryData dat;
        if(mFactory.TryGetValue(type, out dat)) {
            entityRet = dat.Allocate(name, toParent == null ? dat.defaultParent == null ? transform : null : toParent);

            if(entityRet != null) {
                if(!string.IsNullOrEmpty(waypoint)) {
                    if(WaypointManager.instance != null) {
                        Transform wp = WaypointManager.instance.GetWaypoint(waypoint);
                        if(wp != null) {
                            entityRet.transform.position = wp.position;
                        }
                    }
                    else {
                        Debug.LogWarning("Waypoint Manager is not present, trying to hook-up with: " + waypoint);
                    }
                }

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
                dat.Release(entity);
            }

            entity.SendMessage("OnDespawned", null, SendMessageOptions.DontRequireReceiver);
        }
        else { //not in the pool, just kill it
            Object.Destroy(entity);
            //StartCoroutine(DestroyEntityDelay(entity.gameObject));
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

            mControllers.Remove(name);

            if(mControllers.Count == 0)
                mControllers = null;
        }
    }

    void Awake() {
        if(mControllers == null) {
            mControllers = new Dictionary<string, PoolController>();
        }

        if(!mControllers.ContainsKey(name)) {
            mControllers.Add(name, this);

            if(_persistent)
                DontDestroyOnLoad(gameObject);

            poolHolder.gameObject.SetActive(false);

            //generate cache and such
            mFactory = new Dictionary<string, FactoryData>(factory.Length);
            foreach(FactoryData factoryData in factory) {
                factoryData.Init(poolHolder);

                mFactory.Add(factoryData.template.name, factoryData);
            }
        }
        else {
            Debug.LogWarning("PoolController for: " + name + " already exists!");
            DestroyImmediate(gameObject);
        }
    }
}

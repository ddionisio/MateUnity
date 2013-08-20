using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Use this to save spawned items.  The game object must be spawned via PoolController and must have the component SceneSerializerSpawnItem
/// </summary>
[AddComponentMenu("M8/Core/SceneSerializerSpawnManager")]
public class SceneSerializerSpawnManager : MonoBehaviour {
    private static SceneSerializerSpawnManager mInstance = null;

    [System.Serializable]
    private class SpawnInfo {
        public int id;
        public string grp;
        public string type;
        public Vector3 pos;
        public Quaternion rot;

        public SpawnInfo(SceneSerializerSpawnItem item) {
            PoolDataController pdc = item.GetComponent<PoolDataController>();

            id = item.id;
            grp = pdc.group;
            type = pdc.factoryKey;
            pos = item.transform.position;
            rot = item.transform.rotation;
        }

        public void Spawn() {
            Transform t = PoolController.Spawn(grp, type, type + id, null, pos, rot);

            SceneSerializer ss = t.GetComponent<SceneSerializer>();
            ss.__EditorSetID(id);
        }
    }

    //[spawn id, SpawnInfo]
    private Dictionary<int, SpawnInfo> mSpawns;
    private int mNextId = -1;

    private bool mStarted = false;

    public static SceneSerializerSpawnManager instance { get { return mInstance; } }

    /// <summary>
    /// only SceneSerializerSpawnItem calls this
    /// </summary>
    public void RegisterSpawn(SceneSerializerSpawnItem item) {
        if(item.id == SceneSerializer.invalidID) {
            item.__EditorSetID(mNextId);
            mNextId--;

            mSpawns.Add(item.id, new SpawnInfo(item));
        }
        //if id is valid, then we already spawned it
    }

    /// <summary>
    /// only SceneSerializerSpawnItem calls this
    /// </summary>
    public void UnRegisterSpawn(SceneSerializerSpawnItem item) {
        mSpawns.Remove(item.id);
    }

    void OnUserDataAction(UserData ud, UserData.Action act) {
        switch(act) {
            case UserData.Action.Enable:
                if(mStarted)
                    StartCoroutine(DoSpawnWait());
                break;

            case UserData.Action.Disable:
                DoSave();
                mSpawns.Clear();
                break;
        }
    }

    void OnDestroy() {
        if(mInstance == this)
            mInstance = null;
    }

    void Awake() {
        if(mInstance == null) {
            mInstance = this;

            UserData.instance.actCallback += OnUserDataAction;

            mSpawns = new Dictionary<int, SpawnInfo>();
        }
    }

    // Use this for initialization
    void Start() {
        mStarted = true;
        DoSpawns();
    }

    void DoSave() {
        if(mSpawns != null && mSpawns.Count > 0) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, new List<SpawnInfo>(mSpawns.Values));
            UserData.instance.SetString("sssm_" + Application.loadedLevelName, System.Convert.ToBase64String(ms.GetBuffer()));
        }
        else {
            UserData.instance.Delete("sssm_" + Application.loadedLevelName);
        }
    }

    void DoSpawns() {
        //load
        string dat = UserData.instance.GetString("sssm_" + Application.loadedLevelName);

        if(!string.IsNullOrEmpty(dat)) {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(dat));

            List<SpawnInfo> spawnList = (List<SpawnInfo>)bf.Deserialize(ms);

            //start spawning stuff
            foreach(SpawnInfo spawn in spawnList) {
                if(spawn.id <= mNextId)
                    mNextId = spawn.id - 1;

                mSpawns.Add(spawn.id, spawn);
                spawn.Spawn();
            }
        }
    }

    IEnumerator DoSpawnWait() {
        yield return new WaitForFixedUpdate();

        //start spawning stuff
        DoSpawns();
    }
}

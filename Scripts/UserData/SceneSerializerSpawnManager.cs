using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace M8 {
    /// <summary>
    /// Use this to save spawned items.  The game object must be spawned via PoolController and must have the component SceneSerializerSpawnItem
    /// </summary>
    [AddComponentMenu("M8/Serializer/Spawn Manager")]
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
                var spawned = PoolController.SpawnFromGroup(grp, type, type + id, null, pos, rot, null);

                SceneSerializer ss = spawned.GetComponent<SceneSerializer>();
                ss.__SetID(id);
            }
        }

        [SerializeField]
        UserData _userData = null;

        //[spawn id, SpawnInfo]
        private Dictionary<int, SpawnInfo> mSpawns;
        private int mNextId = -1;

        public static SceneSerializerSpawnManager instance { get { return mInstance; } }

        /// <summary>
        /// only SceneSerializerSpawnItem calls this
        /// </summary>
        public void RegisterSpawn(SceneSerializerSpawnItem item) {
            if(item.id == SceneSerializer.invalidID) {
                item.__SetID(mNextId);
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

        void OnDestroy() {
            if(mInstance == this) {
                DoSave();

                mInstance = null;
            }
        }

        void Awake() {
            if(mInstance == null) {
                mInstance = this;

                mSpawns = new Dictionary<int, SpawnInfo>();
            }
        }

        // Use this for initialization
        void Start() {
            DoSpawns();
        }

        void DoSave() {
            string curScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if(mSpawns != null && mSpawns.Count > 0) {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, new List<SpawnInfo>(mSpawns.Values));
                _userData.SetString("sssm_" + curScene, System.Convert.ToBase64String(ms.GetBuffer()));
            }
            else {
                _userData.Remove("sssm_" + curScene);
            }
        }

        void DoSpawns() {
            //load
            string curScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            string dat = _userData.GetString("sssm_" + curScene);

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
    }
}
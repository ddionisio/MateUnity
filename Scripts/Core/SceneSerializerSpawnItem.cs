using UnityEngine;
using System.Collections;

/// <summary>
/// Use this for items that you want to persist after they are spawned. So when you return to the scene, this item will be spawned.
/// Make sure SceneSerializerSpawnManager exists. This object must be created via PoolController, when this object is despawned,
/// its saved data will be removed and will no longer be spawned.
/// </summary>
[AddComponentMenu("M8/Core/SceneSerializerSpawnItem")]
public class SceneSerializerSpawnItem : SceneSerializer {
    void OnSpawned() {
        StartCoroutine(DoSpawn());
    }

    void OnDespawned() {
        MarkRemove();
    }

    void OnDisable() {
        StopAllCoroutines();
    }

    public override void MarkRemove() {
        if(id != invalidID) {
            //remove info from manager
            SceneSerializerSpawnManager.instance.UnRegisterSpawn(this);

            DeleteAllValues();

            //possible that we will be spawned again as a fresh new item
            __EditorSetID(invalidID);
        }
    }

    protected override void Awake() {
    }

    IEnumerator DoSpawn() {
        yield return new WaitForFixedUpdate();

        //if our id is already valid, then it shouldn't be registered
        SceneSerializerSpawnManager.instance.RegisterSpawn(this);
    }
}

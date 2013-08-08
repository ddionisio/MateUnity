using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game/GamePlatformActivator")]
public class GamePlatformActivator : MonoBehaviour {

    [System.Serializable]
    public class DataPlatform {
        public GamePlatform platform;
        public GameObject go;
    }

    public DataPlatform[] platforms;

    void OnEnable() {
        GamePlatform platform = Main.instance.platform;

        foreach(DataPlatform dp in platforms) {
            dp.go.SetActive(dp.platform == platform);
        }
    }

    void Awake() {
        DoIt();
    }

    private void DoIt() {
        GamePlatform platform = Main.instance.platform;

        foreach(DataPlatform dp in platforms) {
            dp.go.SetActive(dp.platform == platform);
        }
    }
}

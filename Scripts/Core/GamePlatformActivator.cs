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
        //for debug purpose, wait till main is initialized, otherwise it should be guaranteed
        if(Main.instance != null)
            DoIt();
        else
            StartCoroutine(LateDoIt());
    }

    IEnumerator LateDoIt() {
        yield return new WaitForFixedUpdate();

        DoIt();
    }
    
    private void DoIt() {
        GamePlatform platform = Main.instance.platform;

        foreach(DataPlatform dp in platforms) {
            dp.go.SetActive(dp.platform == platform);
        }
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Core/OUYAController")]
#if OUYA
public class OUYAController : MonoBehaviour, OuyaSDK.IPauseListener, OuyaSDK.IResumeListener {

    void OnDestroy() {
        OuyaSDK.unregisterPauseListener(this);
        OuyaSDK.unregisterResumeListener(this);
    }

    void Awake() {
        OuyaSDK.registerPauseListener(this);
        OuyaSDK.registerResumeListener(this);
    }

    void Start() {
    }

    public void OuyaOnPause() {
        if(Main.instance != null) {
            SceneManager sm = Main.instance.sceneManager;
            if(sm != null) {
                sm.Pause();
            }
        }
    }

    public void OuyaOnResume() {
        if(Main.instance != null) {
            SceneManager sm = Main.instance.sceneManager;
            if(sm != null) {
                sm.Resume();
            }
        }
    }
}
#else
public class OUYAController : MonoBehaviour {
}
#endif

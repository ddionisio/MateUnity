using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Game/GOActiveBySceneStateFlag")]
public class GOActiveBySceneStateFlag : MonoBehaviour {
    [HideInInspector]
    public string flag;

    [HideInInspector]
    public int flagBit;

    public bool global;

    public bool setActive;

    public GameObject target;

    private bool mStarted;

    void OnEnable() {
        if(mStarted) {
            bool val = global ? SceneState.instance.CheckGlobalFlagMask(flag, flagBit) : SceneState.instance.CheckFlag(flag, flagBit);
            if(val)
                target.SetActive(setActive);
        }
    }

    void Awake() {
        if(target == null)
            target = gameObject;
    }

    void Start() {
        mStarted = true;

        bool val = global ? SceneState.instance.CheckGlobalFlagMask(flag, flagBit) : SceneState.instance.CheckFlag(flag, flagBit);
        if(val)
            target.SetActive(setActive);
    }
}

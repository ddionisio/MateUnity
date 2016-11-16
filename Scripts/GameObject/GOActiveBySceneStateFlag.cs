using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Active By SceneState Flag")]
    public class GOActiveBySceneStateFlag : MonoBehaviour {
        [HideInInspector]
        public string flag;

        [HideInInspector]
        public uint flagMask;

        public bool global;

        public bool setActive;

        public GameObject target;

        private bool mStarted;

        void OnEnable() {
            if(mStarted) {
                bool val = (global ? SceneState.instance.global : SceneState.instance.local).CheckFlagMask(flag, flagMask);
                if(val)
                    target.SetActive(setActive);
                else
                    target.SetActive(!setActive);
            }
        }

        void Awake() {
            if(target == null)
                target = gameObject;
        }

        void Start() {
            //ensure scene state has called its Start() and whatever modifications to flags are set during Start() anywhere
            StartCoroutine(DoIt());
        }

        IEnumerator DoIt() {
            yield return new WaitForFixedUpdate();

            mStarted = true;

            bool val = (global ? SceneState.instance.global : SceneState.instance.local).CheckFlagMask(flag, flagMask);
            if(val)
                target.SetActive(setActive);
            else
                target.SetActive(!setActive);

            yield break;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Activate Delay")]
    public class GOActivateDelay : MonoBehaviour {
        public delegate void OnActivate();

        public GameObject target;
        public bool resetActive;

        public float delay = 1.0f;

        public event OnActivate activateCallback;

        void OnDestroy() {
            activateCallback = null;
        }

        void OnEnable() {
            if(resetActive && target)
                target.SetActive(false);

            Invoke("OnActivateTarget", delay);
        }

        void OnDisable() {
            CancelInvoke();
        }

        void OnActivateTarget() {
            if(target)
                target.SetActive(true);

            if(activateCallback != null)
                activateCallback();
        }
    }
}
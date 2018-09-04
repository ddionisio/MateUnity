using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Active Toggle")]
    public class GOActiveToggle : MonoBehaviour {
        public GameObject target;
        public bool activeDefault = true;
        public bool resetOnEnable;

        public void Reset() {
            if(!target)
                target = gameObject;

            target.SetActive(activeDefault);
        }

        public void Toggle() {
            target.SetActive(!target.activeSelf);
        }

        void OnEnable() {
            if(resetOnEnable)
                Reset();
        }

        void Awake() {
            Reset();
        }
    }
}
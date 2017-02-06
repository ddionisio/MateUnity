using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace M8 {
    [AddComponentMenu("M8/Game Object/Active Flip")]
    public class GOActiveFlip : MonoBehaviour {
        public GameObject activeTarget;
        public GameObject inactiveTarget;

        public float delay;

        private Coroutine mRout;

        void OnEnable() {
            mRout = StartCoroutine(DoFlip());
        }

        void OnDisable() {
            if(mRout != null) {
                StopCoroutine(mRout);
                mRout = null;
            }

            activeTarget.SetActive(true);
            inactiveTarget.SetActive(false);
        }
        
        IEnumerator DoFlip() {
            var wait = new WaitForSeconds(delay);

            bool _active = true;

            while(true) {
                activeTarget.SetActive(_active);
                inactiveTarget.SetActive(!_active);

                _active = !_active;

                yield return wait;
            }
        }
    }
}
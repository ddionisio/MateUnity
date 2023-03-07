using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Game Object/Active Blink")]
    public class GOActiveBlink : MonoBehaviour {

        public GameObject target;
        public float delay;
        public bool isRealTime;

        private bool mDefaultActive;
        private float mLastTime;

        void OnEnable() {
            if(target)
                mDefaultActive = target.activeSelf;

            mLastTime = isRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        void OnDisable() {
            if(target)
                target.SetActive(mDefaultActive);
        }

        void Update() {
            if(target) {
                var curTime = isRealTime ? Time.realtimeSinceStartup : Time.time;
                if(curTime - mLastTime >= delay) {
                    target.SetActive(!target.activeSelf);

                    mLastTime = curTime;
                }
            }
        }
    }
}
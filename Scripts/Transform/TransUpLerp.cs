using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/UpLerp")]
    public class TransUpLerp : MonoBehaviour {
        public Vector3 up = Vector3.up;
        public float delay = 1.0f;

        public Transform target; //optional

        private WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

        public void Go() {
            StopAllCoroutines();
            StartCoroutine(DoIt());
        }

        public void ApplyUp() {
            target.up = up;
        }

        IEnumerator DoIt() {
            Vector3 curUp = target.up;

            float time = 0.0f;

            while(time <= delay) {
                float delta = Time.fixedDeltaTime;

                //assuming up properly normalizes
                target.up = Vector3.Lerp(curUp, up, time / delay);

                time += delta;

                yield return mWaitUpdate;
            }

            target.up = up;
        }

        void Awake() {
            if(target == null)
                target = transform;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Transform/UpLookAt")]
    public class TransUpLookAt : MonoBehaviour {
        public Transform target;
        public string targetTag; //if not empty, acquire target via tag

        public Transform source; //the source to set the up vector

        public bool lockX;
        public bool lockY;
        public bool lockZ = true;

        public bool useTrigger; //acquire target via trigger collider, allow for look-at to stop looking upon exit

        public float lookDelay;

        private Transform mTrans;

        private bool mStarted;
        private Vector3 mCurVel;

        void OnTriggerEnter(Collider c) {
            if(useTrigger && target == null)
                target = c.transform;
        }

        void OnTriggerExit(Collider c) {
            if(useTrigger && target == c.transform)
                target = null;
        }

        void OnEnable() {
            if(mStarted) {
                if(target == null && !useTrigger && !string.IsNullOrEmpty(targetTag)) {
                    GameObject go = GameObject.FindGameObjectWithTag(targetTag);
                    if(go)
                        target = go.transform;
                }
            }
        }

        void OnDisable() {
            if(mStarted && useTrigger)
                target = null;

            mCurVel = Vector3.zero;
        }
        
        void Start() {
#if UNITY_EDITOR
            if(!Application.isPlaying) //don't allow during edit mode
                return;
#endif
            mStarted = true;
            OnEnable();
        }

        // Update is called once per frame
        void Update() {
            if(target != null) {
                var src = source ? source : transform;

                Vector3 dpos = target.position - src.position;

                if(lockX)
                    dpos.x = 0.0f;
                if(lockY)
                    dpos.y = 0.0f;
                if(lockZ)
                    dpos.z = 0.0f;

                if(lookDelay > 0.0f) {
                    dpos.Normalize();
                    src.up = Vector3.SmoothDamp(src.up, dpos, ref mCurVel, lookDelay, Mathf.Infinity, Time.deltaTime);
                }
                else
                    src.up = dpos;
            }
        }
    }
}
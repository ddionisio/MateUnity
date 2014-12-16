using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/Follow")]
    public class TransFollow : MonoBehaviour {
        public Transform target;
        public Vector3 ofs;

        public bool lockX;
        public bool lockY;
        public bool lockZ;

        public float delay;

        private Vector3 mCurVel;

        public Vector3 currentVelocity { get { return mCurVel; } }

        public void SnapToTarget() {
            Vector3 pos = transform.position;
            Vector3 tgt = target.position + target.rotation*ofs;
            if(lockX)
                tgt.x = pos.x;
            if(lockY)
                tgt.y = pos.y;
            if(lockZ)
                tgt.z = pos.z;

            transform.position = tgt;
        }

        void OnEnable() {
            mCurVel = Vector3.zero;
        }

        // Update is called once per frame
        void Update() {
            if(target) {
                Vector3 pos = transform.position;
                Vector3 tgt = target.position + target.rotation*ofs;
                if(lockX)
                    tgt.x = pos.x;
                if(lockY)
                    tgt.y = pos.y;
                if(lockZ)
                    tgt.z = pos.z;

                transform.position = Vector3.SmoothDamp(pos, tgt, ref mCurVel, delay, Mathf.Infinity, Time.deltaTime);
            }
        }

        void OnDrawGizmosSelected() {
            if(target) {
                Gizmos.color = Color.green*0.5f;
                Vector3 pt = target.position + target.rotation*ofs;
                Gizmos.DrawSphere(pt, 0.1f);
            }
        }
    }
}
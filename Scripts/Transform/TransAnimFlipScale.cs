using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Toggle between original scale and (original scale)*mod
    /// </summary>
    [AddComponentMenu("M8/Transform/AnimFlipScale")]
    public class TransAnimFlipScale : MonoBehaviour {
        public Transform target;

        public Vector3 mod = Vector3.one;

        public float delay;

        private Vector3 mPrevScale;
        private bool mIsPrev;

        private bool mStarted;

        void OnEnable() {
            if(mStarted && !IsInvoking("DoFlip"))
                InvokeRepeating("DoFlip", delay, delay);
        }

        void OnDisable() {
            if(!mIsPrev) {
                target.localScale = mPrevScale;
                mIsPrev = true;
            }

            CancelInvoke();
        }

        void Awake() {
            if(target == null)
                target = transform;
        }

        void Start() {
            mPrevScale = target.localScale;
            mIsPrev = true;
            mStarted = true;
            InvokeRepeating("DoFlip", delay, delay);
        }

        void DoFlip() {
            if(mIsPrev) {
                target.localScale = new Vector3(mPrevScale.x * mod.x, mPrevScale.y * mod.y, mPrevScale.z * mod.z);
                mIsPrev = false;
            }
            else {
                target.localScale = mPrevScale;
                mIsPrev = true;
            }
        }
    }
}
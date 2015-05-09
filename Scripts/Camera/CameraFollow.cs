using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Camera/Follow")]
    public class CameraFollow : MonoBehaviour {
        public Camera cameraSource;
        public float deadZone;
        public float delay;

        public bool lockZ;

        [SerializeField]
        Transform targetStart;

        private Transform mSource;
        private Transform mFollow;
        private Vector3 mFollowLastPos;
        private bool mMoving;
        private Vector3 mCurVel;
        private bool mStarted;

        public void SetFollow(Transform follow, bool snap) {
            mFollow = follow;
            if(mFollow) {
                if(snap) {
                    Vector3 followPos = mFollow.position;
                    if(lockZ) followPos.z = mSource.position.z;

                    mSource.transform.position = followPos;
                }
                mFollowLastPos = mFollow.position;
                mCurVel = Vector3.zero;
            }

            mMoving = false;
        }

        void OnEnable() {
            if(mStarted) {
                if(mFollow)
                    mFollowLastPos =  mFollow.position;
                mMoving = false;
            }
        }

        void Awake() {
            if(!cameraSource)
                cameraSource = GetComponent<Camera>();
            if(cameraSource)
                mSource = cameraSource.transform;
        }

        // Use this for initialization
        void Start() {
            SetFollow(targetStart, true);
            mStarted = true;
        }

        // Update is called once per frame
        void Update() {
            if(mFollow) {
                Vector3 srcPos = mSource.position;
                Vector3 followPos = mFollow.position;
                if(lockZ) followPos.z = srcPos.z;

                if(mMoving) {
                    Vector3 newSrcPos = Vector3.SmoothDamp(srcPos, mFollowLastPos, ref mCurVel, delay);
                    mSource.position = newSrcPos;
                    if(M8.MathUtil.VectorEqualApproximately(newSrcPos, mFollowLastPos)) {
                        mFollowLastPos = followPos;
                        mMoving = false;
                    }
                }

                if((followPos - mFollowLastPos).sqrMagnitude > deadZone*deadZone) {
                    mFollowLastPos = followPos;
                    mMoving = true;
                }
            }
        }
    }
}
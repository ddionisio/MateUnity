using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimSpinner")]
    public class TransAnimSpinner : MonoBehaviour {
        public enum UpdateType {
            Update,
            FixedUpdate,
            RigidBody, //used for updating rigidbody in fixedupdate
            RealTime
        }

        public Transform target;

        public Vector3 rotatePerSecond;
        public bool local = true;
        public UpdateType updateType = UpdateType.Update;
        public bool resetOnDisable;

        private Vector3 mEulerAnglesOrig;
        private Vector3 mEulerAnglesDefault;
        private float mSpeedScale = 1.0f;
        private Rigidbody mBody;
        private float mLastTime;

        public float speedScale {
            get { return mSpeedScale; }
            set {
                mSpeedScale = value;
            }
        }

        void OnEnable() {
            RefreshLastTime();
        }

        void OnDisable() {
            mSpeedScale = 1.0f;

            if(resetOnDisable) {
                if(local)
                    target.localEulerAngles = mEulerAnglesDefault;
                else
                    target.eulerAngles = mEulerAnglesDefault;
            }
        }

        // Use this for initialization
        void Awake() {
            if(!target) target = transform;

            mBody = target.GetComponent<Rigidbody>();
            mEulerAnglesOrig = target.eulerAngles;

            mEulerAnglesDefault = local ? target.localEulerAngles : mEulerAnglesOrig;
        }

        void RefreshLastTime() {
            switch(updateType) {
                case UpdateType.Update:
                    mLastTime = Time.time;
                    break;
                case UpdateType.FixedUpdate:
                case UpdateType.RigidBody:
                    mLastTime = Time.fixedTime;
                    break;
                case UpdateType.RealTime:
                    mLastTime = Time.realtimeSinceStartup;
                    break;
            }
        }

        // Update is called once per frame
        void Update() {
            if(updateType == UpdateType.Update || updateType == UpdateType.RealTime) {
                float time = updateType == UpdateType.Update ? Time.time : Time.realtimeSinceStartup;
                float dt = time - mLastTime;
                mLastTime = time;

                if(local) {
                    target.localEulerAngles = target.localEulerAngles + (rotatePerSecond * mSpeedScale * dt);
                }
                else {
                    mEulerAnglesOrig += rotatePerSecond * mSpeedScale * dt;
                    target.eulerAngles = mEulerAnglesOrig;
                }
            }
        }

        void FixedUpdate() {
            if(updateType == UpdateType.FixedUpdate) {
                float time = Time.fixedTime;
                float dt = time - mLastTime;
                mLastTime = time;

                if(local) {
                    target.localEulerAngles = target.localEulerAngles + (rotatePerSecond * mSpeedScale * dt);
                }
                else {
                    mEulerAnglesOrig += rotatePerSecond * mSpeedScale * dt;
                    target.eulerAngles = mEulerAnglesOrig;
                }
            }
            else if(updateType == UpdateType.RigidBody && mBody) {
                float time = Time.fixedTime;
                float dt = time - mLastTime;
                mLastTime = time;

                if(local) {
                    Vector3 eulers = target.eulerAngles;
                    mBody.MoveRotation(Quaternion.Euler(eulers + (rotatePerSecond * mSpeedScale * dt)));
                }
                else {
                    mEulerAnglesOrig += rotatePerSecond * mSpeedScale * dt;
                    mBody.MoveRotation(Quaternion.Euler(mEulerAnglesOrig));
                }
            }
        }
    }
}
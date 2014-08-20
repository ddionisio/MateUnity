using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/AnimSpinner")]
public class TransAnimSpinner : MonoBehaviour {
    public enum UpdateType {
        Update,
        FixedUpdate,
        RigidBody, //used for updating rigidbody in fixedupdate
        RealTime
    }

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
                transform.localEulerAngles = mEulerAnglesDefault;
            else
                transform.eulerAngles = mEulerAnglesDefault;
        }
    }

    // Use this for initialization
    void Awake() {
        mBody = rigidbody;
        mEulerAnglesOrig = transform.eulerAngles;

        mEulerAnglesDefault = local ? transform.localEulerAngles : mEulerAnglesOrig;
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
                transform.localEulerAngles = transform.localEulerAngles + (rotatePerSecond * mSpeedScale * dt);
            }
            else {
                mEulerAnglesOrig += rotatePerSecond * mSpeedScale * dt;
                transform.eulerAngles = mEulerAnglesOrig;
            }
        }
    }

    void FixedUpdate() {
        if(updateType == UpdateType.FixedUpdate) {
            float time = Time.fixedTime;
            float dt = time - mLastTime;
            mLastTime = time;

            if(local) {
                transform.localEulerAngles = transform.localEulerAngles + (rotatePerSecond * mSpeedScale * dt);
            }
            else {
                mEulerAnglesOrig += rotatePerSecond * mSpeedScale * dt;
                transform.eulerAngles = mEulerAnglesOrig;
            }
        }
        else if(updateType == UpdateType.RigidBody && mBody) {
            float time = Time.fixedTime;
            float dt = time - mLastTime;
            mLastTime = time;

            if(local) {
                Vector3 eulers = transform.eulerAngles;
                mBody.MoveRotation(Quaternion.Euler(eulers + (rotatePerSecond * mSpeedScale * dt)));
            }
            else {
                mEulerAnglesOrig += rotatePerSecond * mSpeedScale * dt;
                mBody.MoveRotation(Quaternion.Euler(mEulerAnglesOrig));
            }
        }
    }
}

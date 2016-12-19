using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Physics/GravityController")]
    public class GravityController : MonoBehaviour {
        public Vector3 startUp = Vector3.up; //initial up vector, this is to orient the object's up to match this, if zero, init with transform's up

        public bool orientUp = true; //allow orientation of the up vector

        public float gravity = -9.8f; //the gravity accel, -9.8 m/s^2 as default

        public float orientationSpeed = 90.0f;

        public bool ignoreFields = false;

        protected bool mIsOrienting;
        protected Quaternion mRotateTo;
        protected WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

        private bool mGravityLocked = false;
        private Vector3 mUp;
        private float mStartGravity;
        private bool mStarted;
        private float mMoveScale = 1.0f; //NOTE: reset during disable

        private const int mMaxGravityFields = 4;
        private GravityFieldBase[] mGravityFields = new GravityFieldBase[mMaxGravityFields];
        private int mGravityFieldCurCount;

        private Rigidbody mBody;
        private Collider mColl;

        public Rigidbody body { get { return mBody; } }
        public Collider coll { get { return mColl; } }

        public bool gravityLocked { get { return mGravityLocked; } set { mGravityLocked = value; } }
        public float startGravity { get { return mStartGravity; } }

        public float moveScale { get { return mMoveScale; } set { mMoveScale = value; } }

        public Vector3 up {
            get { return mUp; }
            set {
                if(mUp != value) {
                    mUp = value;

                    ApplyUp();
                }
            }
        }

        void OnEnable() {
            if(mStarted) {
                Init();
            }
        }

        void OnDisable() {
            if(mIsOrienting) {
                mIsOrienting = false;
                transform.up = mUp;
            }

            mGravityFieldCurCount = 0;

            gravity = mStartGravity;
            mMoveScale = 1.0f;
        }

        void OnTriggerEnter(Collider col) {
            if(ignoreFields) return;

            if(mGravityFieldCurCount < mMaxGravityFields) {
                GravityFieldBase gravField = col.GetComponent<GravityFieldBase>();
                if(gravField) {
                    int ind = System.Array.IndexOf(mGravityFields, gravField, 0, mGravityFieldCurCount);
                    if(ind == -1) {
                        mGravityFields[mGravityFieldCurCount] = gravField;
                        mGravityFieldCurCount++;
                    }
                }
            }
        }

        void OnTriggerExit(Collider col) {
            if(ignoreFields) return;

            GravityFieldBase gravField = col.GetComponent<GravityFieldBase>();
            if(gravField) {
                int ind = System.Array.IndexOf(mGravityFields, gravField, 0, mGravityFieldCurCount);
                if(ind != -1) {
                    gravField.ItemRemoved(this);

                    if(mGravityFieldCurCount > 1) {
                        mGravityFields[ind] = mGravityFields[mGravityFieldCurCount - 1];
                    }
                    else { //restore
                        up = startUp;
                        gravity = mStartGravity;
                    }

                    mGravityFieldCurCount--;
                }
            }
        }

        protected virtual void Awake() {
            mBody = GetComponent<Rigidbody>();
            mBody.useGravity = false;

            mColl = GetComponent<Collider>();

            if(startUp == Vector3.zero)
                startUp = transform.up;

            mStartGravity = gravity;
        }

        // Use this for initialization
        protected virtual void Start() {
            mStarted = true;
            Init();
        }

        // Update is called once per frame
        protected virtual void FixedUpdate() {
            if(mGravityFieldCurCount > 0) {
                bool fallLimit = false;
                float fallSpeedLimit = Mathf.Infinity;

                Vector3 newUp = Vector3.zero;

                float newGravity = 0.0f;
                int numGravityOverride = 0;

                for(int i = 0; i < mGravityFieldCurCount; i++) {
                    GravityFieldBase gf = mGravityFields[i];
                    if(gf && gf.gameObject.activeSelf && gf.enabled) {
                        newUp += gf.GetUpVector(this);
                        if(gf.gravityOverride) {
                            newGravity += gf.gravity;
                            numGravityOverride++;
                        }

                        if(gf.fallLimit) {
                            fallLimit = true;
                            if(gf.fallSpeedLimit < fallSpeedLimit)
                                fallSpeedLimit = gf.fallSpeedLimit;
                        }
                    }
                    else { //not active, remove it
                        if(mGravityFieldCurCount > 1) {
                            mGravityFields[i] = mGravityFields[mGravityFieldCurCount - 1];
                        }

                        mGravityFieldCurCount--;
                        i--;
                    }
                }

                if(mGravityFieldCurCount > 0) { //in case all were inactive
                    newUp /= ((float)mGravityFieldCurCount); newUp.Normalize();

                    up = newUp;

                    gravity = numGravityOverride > 0 ? newGravity/(float)numGravityOverride : mStartGravity;

                    if(fallLimit) {
                        //assume y-axis, positive up
                        if(mBody && !mBody.isKinematic) {
                            Vector3 localVel = transform.worldToLocalMatrix.MultiplyVector(mBody.velocity);
                            if(localVel.y < -fallSpeedLimit) {
                                localVel.y = -fallSpeedLimit;
                                mBody.velocity = transform.localToWorldMatrix.MultiplyVector(localVel);
                            }
                        }
                    }
                }
                else { //restore
                    up = startUp;
                    gravity = mStartGravity;
                }
            }

            if(!mGravityLocked)
                mBody.AddForce(mUp * gravity * mBody.mass * mMoveScale, ForceMode.Force);
        }

        protected virtual void ApplyUp() {
            if(orientUp) {
                //TODO: figure out better math
                if(M8.MathUtil.RotateToUp(mUp, -transform.right, transform.forward, ref mRotateTo)) {
                    if(!mIsOrienting)
                        StartCoroutine(OrientUp());
                }
            }
        }

        void Init() {
            if(!ignoreFields && GravityFieldBase.global != null) {
                mGravityFields[mGravityFieldCurCount] = GravityFieldBase.global;
                mGravityFieldCurCount++;
            }

            up = startUp;
        }

        protected IEnumerator OrientUp() {
            mIsOrienting = true;

            while(transform.up != mUp) {
                float step = orientationSpeed * Time.fixedDeltaTime;
                mBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, mRotateTo, step));

                yield return mWaitUpdate;
            }

            mIsOrienting = false;
        }
    }
}
using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityController")]
public class GravityController : MonoBehaviour {
    public Vector3 startUp = Vector3.up; //initial up vector, this is to orient the object's up to match this, if zero, init with transform's up

    public bool orientUp = true; //allow orientation of the up vector

    public float gravity = -9.8f; //the gravity accel, -9.8 m/s^2 as default

    public float orientationSpeed = 90.0f;

    protected bool mIsOrienting;
    protected Quaternion mRotateTo;
    protected WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

    private bool mGravityLocked = false;
    private Vector3 mUp;
    private float mStartGravity;
    private bool mStarted;
    private float mMoveScale = 1.0f; //NOTE: reset during disable
    private int mGravityFieldCounter;

    public int gravityFieldCounter { get { return mGravityFieldCounter; } set { mGravityFieldCounter = Mathf.Clamp(value, 0, int.MaxValue); } }
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

        mGravityFieldCounter = 0;

        mMoveScale = 1.0f;
    }
    
    protected virtual void Awake() {
        rigidbody.useGravity = false;

        mStartGravity = gravity;
    }

    // Use this for initialization
    protected virtual void Start() {
        mStarted = true;
        Init();
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
        if(!mGravityLocked)
            rigidbody.AddForce(mUp * gravity * rigidbody.mass * mMoveScale, ForceMode.Force);
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
        if(GravityFieldBase.global != null)
            GravityFieldBase.global.Add(this);

        if(startUp != Vector3.zero) {
            up = startUp;
        }
        else {
            startUp = mUp = transform.up;
        }
    }

    protected IEnumerator OrientUp() {
        mIsOrienting = true;

        while(transform.up != mUp) {
            float step = orientationSpeed * Time.fixedDeltaTime;
            rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, mRotateTo, step));

            yield return mWaitUpdate;
        }

        mIsOrienting = false;
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityController")]
public class GravityController : MonoBehaviour {
    public Vector3 startUp = Vector3.zero; //initial up vector, this is to orient the object's up to match this, if zero, init with transform's up

    public bool orientUp = true; //allow orientation of the up vector

    public float gravity = -9.8f; //the gravity accel, -9.8 m/s^2 as default

    public float orientationSpeed = 90.0f;

    private bool mIsOrienting;
    private Quaternion mRotateTo;
    private WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

    private GravityFieldBase mGravityField; //which field we are currently attached to
    private GravityFieldBase mGravityFieldPrev; //for gravity field that retains, overwritten by a new gravity field
    private Vector3 mUp;

    public Vector3 up {
        get { return mUp; }
        set {
            if(mUp != value) {
                mUp = value;

                ApplyUp();
            }
        }
    }

    public GravityFieldBase gravityField { 
        get { return mGravityField; } 
        set {
            mGravityField = value; 
        } 
    }

    void OnDisable() {
        mIsOrienting = false;
        StopAllCoroutines();
    }

    void Awake() {
        rigidbody.useGravity = false;
    }

    // Use this for initialization
    void Start() {
        if(GravityFieldBase.global != null)
            GravityFieldBase.global.Add(this);

        if(startUp != Vector3.zero) {
            up = startUp;
        }
        else {
            mUp = transform.up;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        rigidbody.AddForce(mUp * gravity * rigidbody.mass, ForceMode.Force);
    }

    void ApplyUp() {
        if(orientUp) {
            Vector3 f = Vector3.Cross(mUp, -transform.right);
            if(f == Vector3.zero || f.sqrMagnitude < 1.0f) {
                Vector3 l = Vector3.Cross(mUp, transform.forward);
                f = Vector3.Cross(l, mUp);
            }

            if(f != Vector3.zero) {
                mRotateTo = Quaternion.LookRotation(f, mUp);

                if(!mIsOrienting)
                    StartCoroutine(OrientUp());
            }
        }
    }

    IEnumerator OrientUp() {
        mIsOrienting = true;

        while(transform.up != mUp) {
            float step = orientationSpeed * Time.fixedDeltaTime;
            rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, mRotateTo, step));

            yield return mWaitUpdate;
        }

        mIsOrienting = false;
    }
}

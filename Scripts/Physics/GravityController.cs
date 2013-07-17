using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityController")]
public class GravityController : MonoBehaviour {
    [SerializeField]
    Vector3 _up = Vector3.up; //initial up vector, this is to orient the object's up to match this

    public bool orientUp = true; //allow orientation of the up vector

    public float gravity = -9.8f; //the gravity accel, -9.8 m/s^2 as default

    public float orientationSpeed = 90.0f;

    private bool mIsOrienting;
    private Quaternion mRotateTo;
    private WaitForFixedUpdate mWaitUpdate = new WaitForFixedUpdate();

    private GravityFieldBase mGravityField; //which field we are currently attached to

    public Vector3 up {
        get { return _up; }
        set {
            if(_up != value) {
                _up = value;

                ApplyUp();
            }
        }
    }

    public GravityFieldBase gravityField { get { return mGravityField; } set { mGravityField = value; } }

    void OnDisable() {
        mIsOrienting = false;
        StopAllCoroutines();
    }

    void Awake() {
        rigidbody.useGravity = false;
    }

    // Use this for initialization
    void Start() {
        ApplyUp();
    }

    // Update is called once per frame
    void FixedUpdate() {
        rigidbody.AddForce(_up * gravity * rigidbody.mass, ForceMode.Force);
    }

    void ApplyUp() {
        if(orientUp) {
            Vector3 f = Vector3.Cross(_up, -transform.right);
            if(f == Vector3.zero || f.sqrMagnitude < 1.0f) {
                Vector3 l = Vector3.Cross(_up, transform.forward);
                f = Vector3.Cross(l, _up);
            }

            mRotateTo = Quaternion.LookRotation(f, _up);

            if(!mIsOrienting)
                StartCoroutine(OrientUp());            
        }
    }

    IEnumerator OrientUp() {
        mIsOrienting = true;

        while(transform.up != _up) {
            float step = orientationSpeed * Time.fixedDeltaTime;
            rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, mRotateTo, step));

            yield return mWaitUpdate;
        }

        mIsOrienting = false;
    }
}

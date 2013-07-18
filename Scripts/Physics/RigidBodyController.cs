using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: right now it assumes a sphere collider
//make sure to add a physics material to the collider
//and set it with a high static friction to reduce sliding,
//and be able to stand still on platforms
[AddComponentMenu("M8/Physics/RigidBodyController")]
public class RigidBodyController : MonoBehaviour {
    const float moveCosCheck = 0.01745240643728351281941897851632f; //cos(89)

    public struct CollideInfo {
        public CollisionFlags flag;
        public Vector3 contactPoint;
        public Vector3 normal;

    }

    public Transform dirHolder; //the forward vector of this determines our forward movement, put this as a child of this gameobject
                                //you'll want this as an attach for camera as well.

    public float moveForce = 60.0f;
    public float moveAirForce = 10.0f;
    public float moveMaxSpeed = 3.5f;

    public float slopSlideForce = 20.0f;

    public float airDrag = 0.05f; //if there is no ground collision, this is the drag
    public float groundDrag = 0.0f; //if there is ground and/or side collision and/or we are moving
    public float standDrag = 0.0f; //drag when we are standing still

    public float slopLimit = 45.0f; //if we are standing still and slope is high, just use groundDrag

    public float topBottomCollisionAngle = 30.0f; //criteria to determine collision flag

    private Vector2 mCurMoveAxis;
    private Vector3 mCurMoveDir;

    //private HashSet<Collider> mColls = new HashSet<Collider>();
    private Dictionary<Collider, CollideInfo> mColls = new Dictionary<Collider, CollideInfo>(16);

    private CollisionFlags mCollFlags;

    private float mTopBottomColCos;

    private bool mIsSlopSlide;
    private Vector3 mSlopNormal;

    private Vector3 mGroundMoveVel;

    public bool isSlopSlide { get { return mIsSlopSlide; } }
    public Vector2 moveAxis { get { return mCurMoveAxis; } set { mCurMoveAxis = value; } }
    public Vector3 moveDir { get { return mCurMoveDir; } }
    public CollisionFlags collisionFlags { get { return mCollFlags; } }
    public bool isGrounded { get { return (mCollFlags & CollisionFlags.Below) != 0; } }
    public Dictionary<Collider, CollideInfo> collisions { get { return mColls; } }

    public bool CheckCollide(Collider col) {
        return mColls.ContainsKey(col);
    }

    void OnCollisionEnter(Collision col) {
        //refresh during stay
        //mCollFlags = CollisionFlags.None;
        //mSlide = false;

        //mColls.Clear();

        Vector3 up = transform.up;
        Vector3 pos = collider.bounds.center;

        foreach(ContactPoint contact in col.contacts) {
            if(!mColls.ContainsKey(contact.otherCollider)) {
                //mColls.Add(contact.otherCollider, contact);

                CollisionFlags colFlag = M8.PhysicsUtil.GetCollisionFlagsSphereCos(up, pos, mTopBottomColCos, contact.point);

                mColls.Add(contact.otherCollider, new CollideInfo() { flag = colFlag, normal = contact.normal, contactPoint = contact.point });
            }
        }

        RefreshCollInfo();
    }

    void OnCollisionStay(Collision col) {
        Vector3 up = transform.up;
        Vector3 pos = collider.bounds.center;

        //refresh contact infos
        foreach(ContactPoint contact in col.contacts) {
            if(mColls.ContainsKey(contact.otherCollider)) {
                CollisionFlags colFlag = M8.PhysicsUtil.GetCollisionFlagsSphereCos(up, pos, mTopBottomColCos, contact.point);

                mColls[contact.otherCollider] = new CollideInfo() { flag = colFlag, normal = contact.normal, contactPoint = contact.point };
            }
        }

        //recalculate flags
        RefreshCollInfo();
    }

    void OnCollisionExit(Collision col) {
        foreach(ContactPoint contact in col.contacts) {
            if(mColls.ContainsKey(contact.otherCollider))
                mColls.Remove(contact.otherCollider);
        }

        RefreshCollInfo();
    }

    protected virtual void Awake() {
        mTopBottomColCos = Mathf.Cos(topBottomCollisionAngle);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
#if UNITY_EDITOR
        mTopBottomColCos = Mathf.Cos(topBottomCollisionAngle);
#endif

        if(mIsSlopSlide) {
            rigidbody.drag = groundDrag;

            Vector3 dir = M8.MathUtil.Slide(-transform.up, mSlopNormal);
            dir.Normalize();
            rigidbody.AddForce(dir * slopSlideForce);
        }

        if(mCurMoveAxis != Vector2.zero) {
            //move
            if(isGrounded) {
                rigidbody.drag = groundDrag;

                Move(moveForce);
            }
            else {
                rigidbody.drag = airDrag;

                Move(moveAirForce);
            }
        }
        else {
            mCurMoveDir = Vector3.zero;

            rigidbody.drag = isGrounded ? (mCollFlags == CollisionFlags.Below ? standDrag : groundDrag) : airDrag;
        }
    }

    //return true if we moved
    bool Move(float force) {
        //compute move direction
        Quaternion dirRot = dirHolder.rotation;
        Vector3 moveDelta = dirRot * Vector3.forward * mCurMoveAxis.y;
        moveDelta += dirRot * Vector3.right * mCurMoveAxis.x;

        mCurMoveDir = moveDelta.normalized;

        //check if we need to slide off walls
        foreach(KeyValuePair<Collider, CollideInfo> pair in mColls) {
            if(pair.Value.flag == CollisionFlags.Sides || pair.Value.flag == CollisionFlags.Below) {
                if(Vector3.Dot(mCurMoveDir, pair.Value.normal) < 0.0f) {
                    moveDelta = M8.MathUtil.Slide(mCurMoveDir, pair.Value.normal);
                    break;
                }
            }
        }

        //check if we can move based on speed or if going against new direction
        Vector3 vel = rigidbody.velocity - mGroundMoveVel;
        //if( < 0.0f)
        //Debug.Log("wtf: "+Vector3.Angle(vel, mCurMoveDir));

        float velMagSqr = vel.sqrMagnitude;
        bool canMove = velMagSqr < moveMaxSpeed * moveMaxSpeed;
        if(!canMove) { //see if we are trying to move the opposite dir
            Vector3 velDir = vel / Mathf.Sqrt(velMagSqr);
            canMove = Vector3.Dot(mCurMoveDir, velDir) < moveCosCheck;
        }

        if(canMove) {
            rigidbody.AddForce(moveDelta * force);
            return true;
        }

        return false;
    }

    void RefreshCollInfo() {
        mCollFlags = CollisionFlags.None;
        mIsSlopSlide = false;
        mGroundMoveVel = Vector3.zero;

        bool groundNoSlope = false; //prevent slope slide if we are also touching a non-slidable ground (standing on the corner base of slope)

        Vector3 up = transform.up;
        //
        foreach(KeyValuePair<Collider, CollideInfo> pair in mColls) {
            Vector3 n = pair.Value.normal;
            CollisionFlags flag = pair.Value.flag;

            mCollFlags |= pair.Value.flag;

            if(flag == CollisionFlags.Below) {
                if(!groundNoSlope) {
                    mIsSlopSlide = Vector3.Angle(up, n) > slopLimit;
                    if(mIsSlopSlide)
                        mSlopNormal = n;
                    else
                        groundNoSlope = true;
                }

                //for platforms
                Rigidbody body = pair.Key.rigidbody;
                if(body != null && body.velocity != Vector3.zero) {
                    mGroundMoveVel += body.velocity;
                }
            }
        }
    }
}

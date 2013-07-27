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

    public delegate void CallbackEvent();

    public Transform dirHolder; //the forward vector of this determines our forward movement, put this as a child of this gameobject
                                //you'll want this as an attach for camera as well.

    public float moveForce = 60.0f;
    public float moveAirForce = 10.0f;
    public float moveMaxSpeed = 3.5f;
    public bool moveNormalize = true;

    public float slopSlideForce = 20.0f;

    public float sideBounceImpulse = 0.72f; //bounce off sides slightly

    public float airDrag = 0.05f; //if there is no ground collision, this is the drag
    public float groundDrag = 0.0f; //if there is ground and/or side collision and/or we are moving
    public float waterDrag = 20.0f; //if within water

    public LayerMask waterLayerMask;

    public float slopLimit = 45.0f; //if we are standing still and slope is high, just use groundDrag

    public float topBottomCollisionAngle = 30.0f; //criteria to determine collision flag

    public event CallbackEvent waterEnterCallback;
    public event CallbackEvent waterExitCallback;
        
    private Vector2 mCurMoveAxis;
    private Vector3 mCurMoveDir;

    //private HashSet<Collider> mColls = new HashSet<Collider>();
    private Dictionary<Collider, CollideInfo> mColls = new Dictionary<Collider, CollideInfo>(16);

    private CollisionFlags mCollFlags;

    private float mTopBottomColCos;

    private bool mIsSlopSlide;
    private Vector3 mSlopNormal;

    private Vector3 mGroundMoveVel;

    private int mWaterCounter; //counter for water triggers

    public float moveForward { get { return mCurMoveAxis.y; } set { mCurMoveAxis.y = value; } }
    public float moveSide { get { return mCurMoveAxis.x; } set { mCurMoveAxis.x = value; } }

    public bool isSlopSlide { get { return mIsSlopSlide; } }
    
    public Vector3 moveDir { get { return mCurMoveDir; } }
    public CollisionFlags collisionFlags { get { return mCollFlags; } }
    public bool isGrounded { get { return (mCollFlags & CollisionFlags.Below) != 0; } }
    public Dictionary<Collider, CollideInfo> collisions { get { return mColls; } }
    public bool isUnderWater { get { return mWaterCounter > 0; } }

    /// <summary>
    /// Check if given collision is currently colliding with this object.
    /// </summary>
    public bool CheckCollide(Collider col) {
        return mColls.ContainsKey(col);
    }

    /// <summary>
    /// Set dirHolder's forward axis to point towards given pos,
    /// if lockUpVector=true, only rotate along the up vector.
    /// </summary>
    public void DirTo(Vector3 pos, bool lockUpVector) {
        if(lockUpVector) {
            float angle = M8.MathUtil.AngleForwardAxis(
                dirHolder.worldToLocalMatrix,
                dirHolder.position,
                Vector3.forward,
                pos);
            dirHolder.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
        }
        else {
            dirHolder.LookAt(pos);
        }
    }

    public void DirTo(GameObject targetGO, bool lockUpVector) {
        DirTo(targetGO.transform.position, lockUpVector);
    }

    public void DirTo(Transform target, bool lockUpVector) {
        DirTo(target.position, lockUpVector);
    }

    // implements

    protected virtual void WaterEnter() {
    }

    protected virtual void WaterExit() {
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

                if(colFlag == CollisionFlags.Sides && sideBounceImpulse > 0.0f)
                    rigidbody.AddForce(contact.normal * sideBounceImpulse, ForceMode.Impulse);

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

    void OnTriggerEnter(Collider col) {
        if((waterLayerMask & (1 << col.gameObject.layer)) != 0) {
            mWaterCounter++;
        }

        if(isUnderWater) {
            WaterEnter();

            if(waterEnterCallback != null)
                waterEnterCallback();
        }
    }

    void OnTriggerExit(Collider col) {
        if((waterLayerMask & (1 << col.gameObject.layer)) != 0) {
            mWaterCounter--;
        }

        if(!isUnderWater) {
            WaterExit();

            if(waterExitCallback != null)
                waterExitCallback();
        }
    }

    protected virtual void OnDestroy() {
        waterEnterCallback = null;
        waterExitCallback = null;
    }
        
    protected virtual void Awake() {
        mTopBottomColCos = Mathf.Cos(topBottomCollisionAngle);
    }

    protected virtual void Start() {
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
#if UNITY_EDITOR
        mTopBottomColCos = Mathf.Cos(topBottomCollisionAngle);
#endif

        if(mIsSlopSlide) {
            rigidbody.drag = isUnderWater ? waterDrag : groundDrag;

            Vector3 dir = M8.MathUtil.Slide(-transform.up, mSlopNormal);
            dir.Normalize();
            rigidbody.AddForce(dir * slopSlideForce);
        }

        if(mCurMoveAxis != Vector2.zero) {
            //move
            if(isGrounded) {
                rigidbody.drag = isUnderWater ? waterDrag : groundDrag;

                Move(dirHolder.rotation, mCurMoveAxis, moveForce);
            }
            else {
                rigidbody.drag = isUnderWater ? waterDrag : airDrag;

                Move(dirHolder.rotation, mCurMoveAxis, moveAirForce);
            }
        }
        else {
            mCurMoveDir = Vector3.zero;

            rigidbody.drag = isUnderWater ? waterDrag : isGrounded ? groundDrag : airDrag;
        }
    }

    //return true if we moved
    public bool Move(Quaternion dirRot, Vector2 axis, float force) {
        //compute move direction
        Vector3 moveDelta = axis.y != 0.0f ? dirRot * Vector3.forward * axis.y : Vector3.zero;

        if(axis.x != 0.0f)
            moveDelta += dirRot * Vector3.right * axis.x;

        mCurMoveDir = moveDelta.normalized;

        if(moveNormalize)
            moveDelta = mCurMoveDir;
        
        //check if we need to slide off walls
        /*foreach(KeyValuePair<Collider, CollideInfo> pair in mColls) {
            if(pair.Value.flag == CollisionFlags.Sides) {
                Vector3 colN = pair.Value.normal;
                if(Vector3.Dot(mCurMoveDir, colN) < 0.0f) {
                    moveDelta = M8.MathUtil.Slide(mCurMoveDir, colN);
                    moveDelta.Normalize();
                    mCurMoveDir = moveDelta;
                }
            }
        }*/

        //M8.DebugUtil.DrawArrow(transform.position, mCurMoveDir);

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
            //M8.Debug.
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
                    else {
                        mIsSlopSlide = false;
                        groundNoSlope = true;
                    }
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

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

    public float moveForce = 50.0f;
    public float moveAirForce = 20.0f;
    public float moveMaxSpeed = 2.5f;

    public float slopSlideForce = 20.0f;

    public float airDrag = 0.015f; //if there is no ground collision, this is the drag
    
    public float groundDrag = 0.0f; //if there is ground and/or side collision and/or we are moving
    
    public float standDrag = 10.0f;
    public LayerMask standDragLayer;

    public float waterDrag = 15.0f; //if within water

    public string waterTag = "Water";
    public LayerMask waterLayer;

    public float slopLimit = 45.0f; //if we are standing still and slope is high, just use groundDrag

    public float topBottomCollisionAngle = 30.0f; //criteria to determine collision flag

    public event CallbackEvent waterEnterCallback;
    public event CallbackEvent waterExitCallback;
        
    private Vector2 mCurMoveAxis;
    private Vector3 mCurMoveDir;

    //private HashSet<Collider> mColls = new HashSet<Collider>();
    protected Dictionary<Collider, CollideInfo> mColls = new Dictionary<Collider, CollideInfo>(16);

    private CollisionFlags mCollFlags;
    private int mCollGroundLayerMask = 0;

    private float mTopBottomColCos;

    private bool mIsSlopSlide;
    private Vector3 mSlopNormal;

    private Vector3 mGroundMoveVel;

    private int mWaterCounter; //counter for water triggers

    private bool mRenewColls = false;

    private float mRadius = 0.0f;
        
    public float moveForward { get { return mCurMoveAxis.y; } set { mCurMoveAxis.y = value; } }
    public float moveSide { get { return mCurMoveAxis.x; } set { mCurMoveAxis.x = value; } }

    public bool isSlopSlide { get { return mIsSlopSlide; } }
    
    public Vector3 moveDir { get { return mCurMoveDir; } }
    public CollisionFlags collisionFlags { get { return mCollFlags; } }
    public bool isGrounded { get { return (mCollFlags & CollisionFlags.Below) != 0; } }
    public Dictionary<Collider, CollideInfo> collisions { get { return mColls; } }
    public bool isUnderWater { get { return mWaterCounter > 0; } }
    public float radius { get { return mRadius; } }
    
    /// <summary>
    /// Get the first occurence of CollideInfo based on given flags
    /// </summary>
    public bool GetCollideInfo(CollisionFlags flags, out CollideInfo info) {
        foreach(KeyValuePair<Collider, CollideInfo> pair in mColls) {
            if((pair.Value.flag & flags) != 0) {
                info = pair.Value;
                return true;
            }
        }

        info = new CollideInfo();
        return false;
    }

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

    public virtual void ResetCollision() {
        mCollFlags = CollisionFlags.None;
        mCollGroundLayerMask = 0;
        mIsSlopSlide = false;
        mGroundMoveVel = Vector3.zero;
        mColls.Clear();
        mWaterCounter = 0;
        mCurMoveAxis = Vector2.zero;
        mCurMoveDir = Vector3.zero;

        mRenewColls = true;
    }

    // implements

    protected virtual void WaterEnter() {
    }

    protected virtual void WaterExit() {
    }

    protected void ClearCollFlags() {
        mCollFlags = 0;
    }

    void OnCollisionEnter(Collision col) {
        if(mRenewColls) {
            mColls.Clear();
            mRenewColls = false;
        }

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
        if(mRenewColls) {
            mColls.Clear();
            mRenewColls = false;
        }

        Vector3 up = transform.up;
        Vector3 pos = collider.bounds.center;

        //refresh contact infos
        foreach(ContactPoint contact in col.contacts) {
            CollisionFlags colFlag = M8.PhysicsUtil.GetCollisionFlagsSphereCos(up, pos, mTopBottomColCos, contact.point);

            if(mColls.ContainsKey(contact.otherCollider)) {
                mColls[contact.otherCollider] = new CollideInfo() { flag = colFlag, normal = contact.normal, contactPoint = contact.point };
            }
            else {
                mColls.Add(contact.otherCollider, new CollideInfo() { flag = colFlag, normal = contact.normal, contactPoint = contact.point });
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

    protected virtual void OnTriggerEnter(Collider col) {
        if(M8.Util.CheckLayerAndTag(col.gameObject, waterLayer, waterTag)) {
            mWaterCounter++;
        }

        if(isUnderWater) {
            WaterEnter();

            if(waterEnterCallback != null)
                waterEnterCallback();
        }
    }

    protected virtual void OnTriggerStay(Collider col) {
        if(mWaterCounter == 0 && M8.Util.CheckLayerAndTag(col.gameObject, waterLayer, waterTag)) {
            mWaterCounter++;

            WaterEnter();

            if(waterEnterCallback != null)
                waterEnterCallback();
        }
    }

    protected virtual void OnTriggerExit(Collider col) {
        if(M8.Util.CheckLayerAndTag(col.gameObject, waterLayer, waterTag)) {
            mWaterCounter--;
            if(mWaterCounter < 0)
                mWaterCounter = 0;
        }

        if(!isUnderWater) {
            WaterExit();

            if(waterExitCallback != null)
                waterExitCallback();
        }
    }

    protected virtual void OnDisable() {
        ResetCollision();
    }

    protected virtual void OnDestroy() {
        waterEnterCallback = null;
        waterExitCallback = null;
    }
        
    protected virtual void Awake() {
        mTopBottomColCos = Mathf.Cos(topBottomCollisionAngle);

        if(collider != null) {
            if(collider is SphereCollider)
                mRadius = ((SphereCollider)collider).radius;
            else if(collider is CapsuleCollider)
                mRadius = ((CapsuleCollider)collider).radius;
        }
    }

    protected virtual void Start() {
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
#if UNITY_EDITOR
        mTopBottomColCos = Mathf.Cos(topBottomCollisionAngle);
#endif

        if(mIsSlopSlide) {
            //rigidbody.drag = isUnderWater ? waterDrag : groundDrag;

            Vector3 dir = M8.MathUtil.Slide(-transform.up, mSlopNormal);
            dir.Normalize();
            rigidbody.AddForce(dir * slopSlideForce);
        }

        if(mCurMoveAxis != Vector2.zero) {
            //move
            if(isGrounded) {
                rigidbody.drag = isUnderWater ? waterDrag : groundDrag;

                Move(dirHolder.rotation, Vector3.forward, Vector3.right, mCurMoveAxis, moveForce);
            }
            else {
                rigidbody.drag = isUnderWater ? waterDrag : airDrag;

                Move(dirHolder.rotation, Vector3.forward, Vector3.right, mCurMoveAxis, moveAirForce);
            }
        }
        else {
            mCurMoveDir = Vector3.zero;

            rigidbody.drag = isUnderWater ? waterDrag : isGrounded && !mIsSlopSlide ? (standDragLayer & mCollGroundLayerMask) == 0 ? groundDrag : standDrag : airDrag;
        }
    }

    //return true if we moved
    public bool Move(Quaternion dirRot, Vector2 axis, float force) {
        return Move(dirRot, Vector3.forward, Vector3.right, axis, force);
    }

    //return true if we moved
    public bool Move(Quaternion dirRot, Vector3 forward, Vector3 right, Vector2 axis, float force) {
        //compute move direction
        Vector3 moveDelta = axis.y != 0.0f ? dirRot * forward * axis.y : Vector3.zero;

        if(axis.x != 0.0f)
            moveDelta += dirRot * right * axis.x;

        mCurMoveDir = moveDelta.normalized;

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
        mCollGroundLayerMask = 0;
        mIsSlopSlide = false;
        mGroundMoveVel = Vector3.zero;

        bool groundNoSlope = false; //prevent slope slide if we are also touching a non-slidable ground (standing on the corner base of slope)

        Vector3 up = transform.up;
        //

        foreach(KeyValuePair<Collider, CollideInfo> pair in mColls) {
            if(pair.Key == null) {
                mRenewColls = true;
                continue;
            }

            Vector3 n = pair.Value.normal;
            CollisionFlags flag = pair.Value.flag;

            mCollFlags |= pair.Value.flag;

            if(flag == CollisionFlags.Below) {
                if(!groundNoSlope) {
                    float a = Vector3.Angle(up, n);
                    mIsSlopSlide = a > slopLimit;
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

                mCollGroundLayerMask |= 1 << pair.Key.gameObject.layer;
            }
        }
    }
}

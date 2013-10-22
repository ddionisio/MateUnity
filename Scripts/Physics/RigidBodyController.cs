using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO: right now it assumes a sphere collider
//make sure to add a physics material to the collider
//and set it with a high static friction to reduce sliding,
//and be able to stand still on platforms
[AddComponentMenu("M8/Physics/RigidBodyController")]
public class RigidBodyController : MonoBehaviour {
    protected const float moveCosCheck = 0.01745240643728351281941897851632f; //cos(89)

    public class CollideInfo {
        public Collider collider;
        public CollisionFlags flag;
        public Vector3 contactPoint;
        public Vector3 normal;
    }

    public delegate void CallbackEvent(RigidBodyController controller);
    public delegate void CollisionCallbackEvent(RigidBodyController controller, Collision col);
    public delegate void TriggerCallbackEvent(RigidBodyController controller, Collider col);

    public Transform dirHolder; //the forward vector of this determines our forward movement, put this as a child of this gameobject
    //you'll want this as an attach for camera as well.

    public float speedCap = 0.0f; //set to > 0 to cap speed

    public float moveForce = 50.0f;
    public float moveAirForce = 20.0f;
    public float moveMaxSpeed = 2.5f;

    public float slopSlideForce = 20.0f;

    public float airMaxSpeed = 1.5f;
    public float airDrag = 0.015f; //if there is no ground collision, this is the drag

    public float groundDrag = 0.0f; //if there is ground and/or side collision and/or we are moving

    public float standDrag = 10.0f;
    public LayerMask standDragLayer;

    public float waterDrag = 15.0f; //if within water

    public string waterTag = "Water";
    public LayerMask waterLayer;

    public float slopLimit = 45.0f; //if we are standing still and slope is high, just use groundDrag, also determines collideflag below
    public float aboveLimit = 145.0f; //determines collideflag above, should be > 90, around 140'ish
    public float slideLimit = 75.0f;

    public event CallbackEvent waterEnterCallback;
    public event CallbackEvent waterExitCallback;
    public event CollisionCallbackEvent collisionEnterCallback;
    public event CollisionCallbackEvent collisionStayCallback;
    public event TriggerCallbackEvent triggerEnterCallback;

    private Vector2 mCurMoveAxis;
    private Vector3 mCurMoveDir;

    //private HashSet<Collider> mColls = new HashSet<Collider>();
    protected const int maxColls = 32;
    protected CollideInfo[] mColls;
    protected int mCollCount = 0;

    //protected Dictionary<Collider, List<CollideInfo>> mColls = new Dictionary<Collider, List<CollideInfo>>(16);

    protected CollisionFlags mCollFlags;
    protected int mCollGroundLayerMask = 0;

    private float mSlopLimitCos;
    private float mAboveLimitCos;

    private bool mIsSlopSlide;
    private Vector3 mSlopNormal;

    private Vector3 mGroundMoveVel;

    private Vector3 mLocalVelocity;

    private int mWaterCounter; //counter for water triggers

    private float mRadius = 0.0f;

    private GravityController mGravCtrl;

    private CapsuleCollider mCapsuleColl;

    private bool mLockDrag = false;

    public float moveForward { get { return mCurMoveAxis.y; } set { mCurMoveAxis.y = value; } }
    public float moveSide { get { return mCurMoveAxis.x; } set { mCurMoveAxis.x = value; } }

    public bool isSlopSlide { get { return mIsSlopSlide; } }

    /// <summary>
    /// Use this to override the rigidbody's drag such that ground/air/stand drag is ignored
    /// </summary>
    public bool lockDrag { get { return mLockDrag; } set { mLockDrag = value; } }

    public Vector3 moveDir { get { return mCurMoveDir; } }
    public CollisionFlags collisionFlags { get { return mCollFlags; } }
    public bool isGrounded { get { return (mCollFlags & CollisionFlags.Below) != 0; } }

    /// <summary>
    /// Note: This will return the entire array, actual length is collisionCount
    /// </summary>
    public CollideInfo[] collisionData { get { return mColls; } }
    public int collisionCount { get { return mCollCount; } }

    public bool isUnderWater { get { return mWaterCounter > 0; } }
    public float radius { get { return mRadius; } }
    public Vector3 groundMoveVelocity { get { return mGroundMoveVel; } }
    public GravityController gravityController { get { return mGravCtrl; } }
    public Vector3 localVelocity { get { return mLocalVelocity; } }

    /// <summary>
    /// Get the first occurence of CollideInfo based on given flags
    /// </summary>
    public CollideInfo GetCollideInfo(CollisionFlags flags) {
        for(int i = 0; i < mCollCount; i++) {
            CollideInfo inf = mColls[i];
            if((inf.flag & flags) != 0)
                return inf;
        }

        return null;
    }

    /// <summary>
    /// Check if given collision is currently colliding with this object.
    /// </summary>
    public bool CheckCollide(Collider col) {
        for(int i = 0; i < mCollCount; i++) {
            if(mColls[i].collider == col)
                return true;
        }

        return false;
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
        mCollCount = 0;
        mWaterCounter = 0;
        mCurMoveAxis = Vector2.zero;
        mCurMoveDir = Vector3.zero;
    }

    public bool CheckPenetrate(float reduceOfs, LayerMask mask) {
        if(mCapsuleColl) {
            Vector3 p1 = mCapsuleColl.center, p2 = p1;
            float h = mCapsuleColl.height - reduceOfs * 2.0f;
            float hHalf = h * 0.5f;
            float r = mCapsuleColl.radius - reduceOfs;

            switch(mCapsuleColl.direction) {
                case 0: //x
                    p1.x -= hHalf - r;
                    p2.x += hHalf - r;
                    break;
                case 1: //y
                    p1.y -= hHalf - r;
                    p2.y += hHalf - r;
                    break;
                case 2: //z
                    p1.z -= hHalf - r;
                    p2.z += hHalf - r;
                    break;
            }

            Matrix4x4 wrldMtx = transform.localToWorldMatrix;

            p1 = wrldMtx.MultiplyPoint(p1);
            p2 = wrldMtx.MultiplyPoint(p2);

            return Physics.CheckCapsule(p1, p2, r, mask);
        }
        else {
            return Physics.CheckSphere(transform.position, mRadius - reduceOfs, mask);
        }
    }

    // implements

    protected virtual void WaterEnter() {
    }

    protected virtual void WaterExit() {
    }

    protected void ClearCollFlags() {
        mCollFlags = 0;
    }

    CollisionFlags GenCollFlag(Vector3 up, ContactPoint contact) {

        float dot = Vector3.Dot(up, contact.normal);

        //Debug.Log("dot: " + dot);
        //Debug.Log("deg: " + (Mathf.Acos(dot)*Mathf.Rad2Deg));

        if(dot >= mSlopLimitCos)
            return CollisionFlags.Below;
        else if(dot <= mAboveLimitCos)
            return CollisionFlags.Above;

        return CollisionFlags.Sides;
        /*if((

        if(mCapsuleColl) {
            return M8.PhysicsUtil.GetCollisionFlagsCapsule(transform, mCapsuleColl.height, mRadius, capsuleCollisionAbove, capsuleCollisionBelow, mCapsuleColl.center, contact.point);
            //return M8.PhysicsUtil.GetCollisionFlagsCapsule(up, transform.localScale, pos, mCapsuleColl.height, mRadius*0.9f, contactPt);
        }
        else {
            return M8.PhysicsUtil.GetCollisionFlagsSphereCos(up, pos, mTopBottomColCos, contact.point);
        }*/
    }

    private const float _nCompareApprox = 0.01f;

    void GenerateColls(Collision col, bool resetCount) {
        if(resetCount)
            mCollCount = 0;

        Vector3 up = transform.up;

        for(int i = 0, max = col.contacts.Length; i < max; i++) {
            if(mCollCount >= maxColls) {
                Debug.LogWarning("Ran out of collideInfo");
                break;
            }

            ContactPoint contact = col.contacts[i];

            Collider whichColl = contact.thisCollider != collider ? contact.thisCollider : contact.otherCollider;

            Vector3 n = contact.normal;
            Vector3 p = contact.point;

            //check if already exists
            int ind = -1;
            for(int j = 0; j < mCollCount; j++) {
                CollideInfo inf = mColls[j];
                if(inf.collider == whichColl) {
                    //if(cf == CollisionFlags.None)
                    //cf = GenCollFlag(up, contact);

                    // if(inf.flag == cf) {
                    ind = j;
                    break;
                    // }
                }
                /*else {
                    Debug.Log("fuck: " + inf.flag + " ass: " + cf + " hash: " + whichColl.GetHashCode());
                }*/
            }

            CollideInfo newInfo;
            if(ind == -1) {
                newInfo = mColls[mCollCount];
                mCollCount++;
            }
            else {
                newInfo = mColls[ind];
            }

            newInfo.collider = whichColl;
            newInfo.contactPoint = p;
            newInfo.normal = n;
            newInfo.flag = GenCollFlag(up, contact);
        }
    }

    void RemoveColl(int ind) {
        if(mCollCount > 0) {
            int lastInd = mCollCount - 1;
            if(ind < lastInd && mCollCount > 1) {
                CollideInfo removeInf = mColls[ind];
                removeInf.collider = null;
                removeInf.flag = CollisionFlags.None;

                CollideInfo lastInf = mColls[lastInd];
                mColls[ind] = lastInf;
            }
            mCollCount--;
        }
    }

    void OnCollisionEnter(Collision col) {
        //Debug.Log("enter: " + col.gameObject.name);

        /*foreach(ContactPoint cp in col.contacts) {
            Debug.Log("in: " + cp.otherCollider.name + " n: " + cp.normal);
        }*/

        GenerateColls(col, false);

        RefreshCollInfo();

        if(collisionEnterCallback != null)
            collisionEnterCallback(this, col);

        //Debug.Log("count: " + mCollCount);
    }

    void OnCollisionStay(Collision col) {
        //int pc = mCollCount;

        GenerateColls(col, false);

        //recalculate flags
        RefreshCollInfo();

        if(collisionStayCallback != null)
            collisionStayCallback(this, col);

        /*if(pc != mCollCount)
            Debug.Log("scount: " + mCollCount);*/
    }

    void OnCollisionExit(Collision col) {

        /*foreach(ContactPoint cp in col.contacts) {
            Debug.Log("out: " + cp.otherCollider.name + " n: " + cp.normal);
        }*/

        //Debug.Log("exit: " + col.gameObject.name);

        //Vector3 up = transform.up;

        foreach(ContactPoint contact in col.contacts) {
            Collider whichColl = contact.thisCollider != collider ? contact.thisCollider : contact.otherCollider;
            //Vector3 n = contact.normal;
            //Vector3 p = contact.point;
            //CollisionFlags cf = CollisionFlags.None;

            for(int i = 0; i < mCollCount; i++) {
                CollideInfo inf = mColls[i];
                if(inf.collider == whichColl) {
                    //if(cf == CollisionFlags.None)
                    //cf = GenCollFlag(up, contact);

                    //if(inf.flag == cf)
                    RemoveColl(i);
                }
            }
        }

        RefreshCollInfo();

        //Debug.Log("exit count: " + mCollCount);
    }

    protected virtual void OnTriggerEnter(Collider col) {
        if(M8.Util.CheckLayerAndTag(col.gameObject, waterLayer, waterTag)) {
            mWaterCounter++;
        }

        if(isUnderWater) {
            WaterEnter();

            if(waterEnterCallback != null)
                waterEnterCallback(this);
        }
        else {
            if(triggerEnterCallback != null)
                triggerEnterCallback(this, col);
        }
    }

    protected virtual void OnTriggerStay(Collider col) {
        if(mWaterCounter == 0 && M8.Util.CheckLayerAndTag(col.gameObject, waterLayer, waterTag)) {
            mWaterCounter++;

            WaterEnter();

            if(waterEnterCallback != null)
                waterEnterCallback(this);
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
                waterExitCallback(this);
        }
    }

    protected virtual void OnDisable() {
        ResetCollision();
    }

    protected virtual void OnDestroy() {
        collisionEnterCallback = null;
        collisionStayCallback = null;
        waterEnterCallback = null;
        waterExitCallback = null;
        triggerEnterCallback = null;
    }

    protected virtual void Awake() {
        mColls = new CollideInfo[maxColls];
        for(int i = 0; i < maxColls; i++)
            mColls[i] = new CollideInfo();

        mGravCtrl = GetComponent<GravityController>();

        //mTopBottomColCos = Mathf.Cos(sphereCollisionAngle * Mathf.Deg2Rad);
        mSlopLimitCos = Mathf.Cos(slopLimit * Mathf.Deg2Rad);
        mAboveLimitCos = Mathf.Cos(aboveLimit * Mathf.Deg2Rad);

        if(collider != null) {
            if(collider is SphereCollider)
                mRadius = ((SphereCollider)collider).radius;
            else if(collider is CapsuleCollider) {
                mCapsuleColl = collider as CapsuleCollider;
                mRadius = mCapsuleColl.radius;
            }
        }
    }

    protected virtual void Start() {
    }

    // Update is called once per frame
    protected virtual void FixedUpdate() {
        if(speedCap > 0.0f) {
            Vector3 vel = rigidbody.velocity;
            float spdSqr = vel.sqrMagnitude;
            if(spdSqr > speedCap * speedCap) {
                rigidbody.velocity = (vel / Mathf.Sqrt(spdSqr)) * speedCap;
            }
        }

#if UNITY_EDITOR
        //mTopBottomColCos = Mathf.Cos(sphereCollisionAngle * Mathf.Deg2Rad);
        mSlopLimitCos = Mathf.Cos(slopLimit * Mathf.Deg2Rad);
        mAboveLimitCos = Mathf.Cos(aboveLimit * Mathf.Deg2Rad);
#endif
        ComputeLocalVelocity();

        if(mIsSlopSlide) {
            //rigidbody.drag = isUnderWater ? waterDrag : groundDrag;

            Vector3 dir = M8.MathUtil.Slide(-transform.up, mSlopNormal);
            dir.Normalize();
            rigidbody.AddForce(dir * slopSlideForce);
        }

        if(mCurMoveAxis != Vector2.zero) {
            //move
            if(isGrounded) {
                if(!mLockDrag)
                    rigidbody.drag = isUnderWater ? waterDrag : groundDrag;

                Move(dirHolder.rotation, Vector3.forward, Vector3.right, mCurMoveAxis, moveForce);
            }
            else {
                if(!mLockDrag)
                    rigidbody.drag = isUnderWater ? waterDrag : airDrag;

                Move(dirHolder.rotation, Vector3.forward, Vector3.right, mCurMoveAxis, moveAirForce);
            }
        }
        else {
            mCurMoveDir = Vector3.zero;

            if(!mLockDrag)
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

        float maxSpeed = 0.0f;

        //allow for moving diagonally downwards
        //TODO: test for non-platformer
        if(isGrounded) {
            for(int i = 0; i < mCollCount; i++) {
                CollideInfo inf = mColls[i];
                if(inf.flag == CollisionFlags.Below) {
                    moveDelta = M8.MathUtil.Slide(moveDelta, inf.normal);
                    mCurMoveDir = moveDelta.normalized;
                    break;
                }
            }

            maxSpeed = moveMaxSpeed;
        }
        else {
            maxSpeed = airMaxSpeed;
        }

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
        bool canMove = CanMove(mCurMoveDir, maxSpeed);

        if(canMove) {
            //M8.Debug.
            rigidbody.AddForce(moveDelta * force);
            return true;
        }

        return false;
    }

    protected virtual bool CanMove(Vector3 dir, float maxSpeed) {
        Vector3 vel = rigidbody.velocity - mGroundMoveVel;
        float sqrMag = vel.sqrMagnitude;

        bool ret = sqrMag < maxSpeed * maxSpeed;

        //see if we are trying to move the opposite dir
        if(!ret) { //see if we are trying to move the opposite dir
            Vector3 velDir = rigidbody.velocity.normalized;
            ret = Vector3.Dot(dir, velDir) < moveCosCheck;
        }

        return ret;
    }

    protected virtual void RefreshCollInfo() {
        mCollFlags = CollisionFlags.None;
        mCollGroundLayerMask = 0;
        mIsSlopSlide = false;
        mGroundMoveVel = Vector3.zero;

        bool groundNoSlope = false; //prevent slope slide if we are also touching a non-slidable ground (standing on the corner base of slope)

        Vector3 up = transform.up;
        //

        for(int i = 0; i < mCollCount; i++) {
            CollideInfo inf = mColls[i];
            if(inf.collider == null || !inf.collider.gameObject.activeSelf) {
                //remove
                RemoveColl(i);
                continue;
            }

            Vector3 n = inf.normal;
            CollisionFlags flag = inf.flag;

            mCollFlags |= inf.flag;

            //sliding
            if(flag == CollisionFlags.Below || flag == CollisionFlags.Sides) {
                if(!groundNoSlope) {
                    float a = Vector3.Angle(up, n);
                    mIsSlopSlide = a > slopLimit && a <= slideLimit;
                    if(mIsSlopSlide) {
                        //Debug.Log("a: " + a);
                        mSlopNormal = n;
                    }
                    else if(flag == CollisionFlags.Below) {
                        mIsSlopSlide = false;
                        groundNoSlope = true;
                    }
                }

                //for platforms
                Rigidbody body = inf.collider.rigidbody;
                if(body != null) {
                    mGroundMoveVel += body.velocity;
                }

                mCollGroundLayerMask |= 1 << inf.collider.gameObject.layer;
            }
        }
    }

    protected void ComputeLocalVelocity() {
        mLocalVelocity = dirHolder.worldToLocalMatrix.MultiplyVector(rigidbody.velocity);
    }
}

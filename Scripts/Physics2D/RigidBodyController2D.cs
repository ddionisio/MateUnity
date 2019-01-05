using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Physics2D/RigidBodyController")]
    public class RigidBodyController2D : MonoBehaviour {
        protected const float moveCosCheck = 0.01745240643728351281941897851632f; //cos(89)

        public struct CollideInfo {
            public Collider2D collider;
            public CollisionFlags flag;
            public Vector2 contactPoint;
            public Vector2 normal;
        }

        public delegate void CallbackEvent(RigidBodyController2D controller);
        public delegate void CollisionCallbackEvent(RigidBodyController2D controller, Collision2D col);
        public delegate void TriggerCallbackEvent(RigidBodyController2D controller, Collider2D col);

        public Transform dirHolder; //the forward vector of this determines our forward movement, put this as a child of this gameobject
                                    //you'll want this as an attach for camera as well.

        public Vector2 speedCap = Vector2.zero; //set to > 0 to cap speed

        public float moveForce = 30.0f;
        public float moveAirForce = 5.0f;
        public float moveMaxSpeed = 10f;

        public float slopSlideForce = 40.0f;

        public float airMaxSpeed = 6f;
        public float airDrag = 0.015f; //if there is no ground collision, this is the drag

        public float groundDrag = 5.0f; //if there is ground and/or side collision and/or we are moving

        public float standDrag = 60.0f;
        public LayerMask standDragLayer;

        public float effectorDrag = 0.015f;
        public LayerMask effectorLayer;

        public float slopeLimit = 50.0f; //if we are standing still and slope is high, just use groundDrag, also determines collideflag below
        public float aboveLimit = 145.0f; //determines collideflag above, should be > 90, around 140'ish
        public float slideLimit = 80.0f;
        public bool slideDisable;

        public event CollisionCallbackEvent collisionEnterCallback;
        public event CollisionCallbackEvent collisionStayCallback;
        public event CollisionCallbackEvent collisionExitCallback;
        public event TriggerCallbackEvent triggerEnterCallback;
        public event TriggerCallbackEvent triggerExitCallback;

        private Vector2 mCurMoveAxis;
        private Vector2 mCurMoveDir;

        protected const int maxColls = 16;
        protected M8.CacheList<CollideInfo> mColls;
        protected ContactPoint2D[] mContactPoints;

        protected CollisionFlags mCollFlags;
        protected int mCollLayerMask = 0; //layer masks that are colliding us (ground and side)

        private float mSlopeLimitCos;
        private float mAboveLimitCos;

        private bool mIsSlopSlide;
        private Vector2 mSlopNormal;

        private Vector2 mLocalVelocity;
        private Vector2 mLastVelocity;

        private float mRadius = 0.0f;

        private CapsuleCollider2D mCapsuleColl;

        private int mLockDragCounter = 0;

        private float mMoveScale = 1.0f;

        private int mEffectorCount;

        protected Rigidbody2D mBody;
        protected Collider2D mColl;

        public Rigidbody2D body { get { return mBody; } }
        public Collider2D coll { get { return mColl; } }

        public float moveVertical { get { return mCurMoveAxis.y; } set { mCurMoveAxis.y = value; } }
        public float moveHorizontal { get { return mCurMoveAxis.x; } set { mCurMoveAxis.x = value; } }

        public bool isSlopSlide { get { return mIsSlopSlide; } }

        /// <summary>
        /// Use this to override the rigidbody's drag such that ground/air/stand drag is ignored, set to 0 to unlock
        /// </summary>
        public int lockDragCounter { get { return mLockDragCounter; } set { mLockDragCounter = value; if(mLockDragCounter < 0) mLockDragCounter = 0; } }

        public Vector2 moveDir { get { return mCurMoveDir; } }
        public CollisionFlags collisionFlags { get { return mCollFlags; } }
        public bool isGrounded { get { return (mCollFlags & CollisionFlags.Below) != 0; } }

        public bool isInEffector { get { return mEffectorCount > 0; } }

        /// <summary>
        /// Note: This will return the entire array, actual length is collisionData.Count or collisionCount
        /// </summary>
        public M8.CacheList<CollideInfo> collisionData { get { return mColls; } }
        public int collisionCount { get { return mColls.Count; } }

        public float radius { get { return mRadius; } }

        public Vector2 localVelocity {
            get { return mLocalVelocity; }
            set {
                if(mLocalVelocity != value) {
                    mLocalVelocity = value;

                    //update body
                    if(mBody.simulated)
                        mBody.velocity = dirHolder.localToWorldMatrix.MultiplyVector(mLocalVelocity);
                }
            }
        }

        public CapsuleCollider2D capsuleCollider { get { return mCapsuleColl; } }

        public float moveScale {
            get { return mMoveScale; }
            set { mMoveScale = value; }
        }

        /// <summary>
        /// Get the first occurence of CollideInfo based on given flags
        /// </summary>
        public bool TryGetCollideInfo(CollisionFlags flags, out CollideInfo inf) {
            for(int i = 0; i < mColls.Count; i++) {
                CollideInfo _inf = mColls[i];
                if((_inf.flag & flags) != 0) {
                    inf = _inf;
                    return true;
                }
            }

            inf = new CollideInfo();
            return false;
        }

        public bool TryGetCollideInfo(Collider col, out CollideInfo inf) {
            for(int i = 0; i < mColls.Count; i++) {
                if(mColls[i].collider == col) {
                    inf = mColls[i];
                    return true;
                }
            }

            inf = new CollideInfo();
            return false;
        }

        /// <summary>
        /// Check if given collision is currently colliding with this object.
        /// </summary>
        public bool CheckCollide(Collider col) {
            for(int i = 0; i < mColls.Count; i++) {
                if(mColls[i].collider == col)
                    return true;
            }

            return false;
        }

        public virtual void ResetCollision() {
            if(mColls == null) return; //not initialized yet

            mCollFlags = CollisionFlags.None;
            mCollLayerMask = 0;
            mIsSlopSlide = false;
            mColls.Clear();
            mCurMoveAxis = Vector2.zero;
            mCurMoveDir = Vector2.zero;
            mLastVelocity = Vector2.zero;
        }

        void GetCapsuleInfo(out Vector2 p1, out Vector2 p2, out float r, float reduceOfs) {
            p1 = mCapsuleColl.offset;
            p2 = p1;

            float h = mCapsuleColl.size.y - reduceOfs;
            float hHalf = h * 0.5f;

            r = mCapsuleColl.size.x * 0.5f;

            switch(mCapsuleColl.direction) {
                case CapsuleDirection2D.Horizontal: //x
                    p1.x -= hHalf - r;
                    p2.x += hHalf - r;
                    break;
                case CapsuleDirection2D.Vertical: //y
                    p1.y -= hHalf - r;
                    p2.y += hHalf - r;
                    break;
            }

            Matrix4x4 wrldMtx = transform.localToWorldMatrix;

            p1 = wrldMtx.MultiplyPoint(p1);
            p2 = wrldMtx.MultiplyPoint(p2);
        }

        public bool CheckPenetrate(float reduceOfs, LayerMask mask) {
            if(mCapsuleColl) {
                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(mCapsuleColl.offset);
                Vector2 collSize = mCapsuleColl.size; collSize.y -= reduceOfs;

                return Physics2D.OverlapCapsule(collPos, collSize, mCapsuleColl.direction, collT.eulerAngles.z, mask) != null;
            }
            else {
                return Physics2D.OverlapCircle(transform.position, mRadius - reduceOfs, mask) != null;
            }
        }

        public bool CheckCast(float reduceOfs, Vector2 dir, out RaycastHit2D hit, float dist, int mask) {
            if(mCapsuleColl) {
                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(mCapsuleColl.offset);
                Vector2 collSize = mCapsuleColl.size; collSize.y -= reduceOfs;

                hit = Physics2D.CapsuleCast(collPos, collSize, mCapsuleColl.direction, collT.eulerAngles.z, dir, dist, mask);
                return hit.collider != null;
            }
            else {
                hit = Physics2D.CircleCast(transform.position, mRadius - reduceOfs, dir, dist, mask);
                return hit.collider != null;
            }
        }

        public RaycastHit2D[] CheckAllCasts(Vector2 posOfs, float reduceOfs, Vector2 dir, float dist, int mask) {
            if(mCapsuleColl) {
                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(mCapsuleColl.offset + posOfs);
                Vector2 collSize = mCapsuleColl.size; collSize.y -= reduceOfs;

                return Physics2D.CapsuleCastAll(collPos, collSize, mCapsuleColl.direction, collT.eulerAngles.z, dir, dist, mask);
            }
            else {
                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(posOfs);

                return Physics2D.CircleCastAll(collPos, mRadius - reduceOfs, dir, dist, mask);
            }
        }
        
        public CollisionFlags GetCollisionFlag(Vector2 up, Vector2 normal) {
            float dot = Vector2.Dot(up, normal);

            //Debug.Log("dot: " + dot);
            //Debug.Log("deg: " + (Mathf.Acos(dot)*Mathf.Rad2Deg));

            if(dot >= mSlopeLimitCos)
                return CollisionFlags.Below;
            else if(dot <= mAboveLimitCos)
                return CollisionFlags.Above;

            return CollisionFlags.Sides;
        }

        protected void ClearCollFlags() {
            mCollFlags = 0;
        }

        CollisionFlags GenCollFlag(Vector2 up, ContactPoint2D contact) {
            return GetCollisionFlag(up, contact.normal);
        }

        private const float _nCompareApprox = 0.01f;

        void GenerateColls(Collision2D col, bool resetCount) {
            if(resetCount)
                mColls.Clear();

            Vector2 up = dirHolder.up;

            int contactCount = col.GetContacts(mContactPoints);
            for(int i = 0; i < contactCount; i++) {
                ContactPoint2D contact = mContactPoints[i];

                Collider2D whichColl = col.collider;// contact.thisCollider != collider ? contact.thisCollider : contact.otherCollider;

                Vector2 n = contact.normal;
                Vector2 p = contact.point;
                CollisionFlags cf = GenCollFlag(up, contact);

                //check if already exists
                int ind = -1;
                for(int j = 0; j < mColls.Count; j++) {
                    CollideInfo inf = mColls[j];
                    if(inf.collider == whichColl) {
                        //if(cf == CollisionFlags.None)
                        //cf = GenCollFlag(up, contact);

                        if(inf.flag == cf) {
                            ind = j;
                            break;
                        }
                    }
                }

                var newCollInf = new CollideInfo() {
                    collider = whichColl,
                    contactPoint = p,
                    normal = n,
                    flag = cf,
                };

                if(ind == -1) { //add new
                    if(mColls.Count == mColls.Capacity)
                        mColls.Expand();

                    mColls.Add(newCollInf);
                }
                else //update info
                    mColls[ind] = newCollInf;
            }
        }

        protected void RemoveColl(int ind) {
            if(ind < mColls.Count)
                mColls.RemoveAt(ind);
        }

        void OnCollisionEnter2D(Collision2D col) {
            //Debug.Log("enter: " + col.gameObject.name);

            /*foreach(ContactPoint cp in col.contacts) {
                Debug.Log("in: " + cp.otherCollider.name + " n: " + cp.normal);
            }*/

            //GenerateColls(col, false);
            //mCollCount = 0;
            //RefreshCollInfo();

            //remove existing information with given collider
            /*for(int j = 0; j < mCollCount; j++) {
                CollideInfo inf = mColls[j];
                if(inf.collider == col.collider) {
                    RemoveColl(j);
                    j--;
                }
            }*/

            GenerateColls(col, false);

            //recalculate flags
            RefreshCollInfo();


            if(collisionEnterCallback != null)
                collisionEnterCallback(this, col);

            //Debug.Log("count: " + mCollCount);
        }

        void OnCollisionStay2D(Collision2D col) {
            //remove existing information with given collider
            for(int j = mColls.Count - 1; j >= 0; j--) {
                CollideInfo inf = mColls[j];
                if(inf.collider == col.collider) {
                    RemoveColl(j);
                }
            }

            GenerateColls(col, false);

            //recalculate flags
            RefreshCollInfo();

            if(collisionStayCallback != null)
                collisionStayCallback(this, col);

            /*if(pc != mCollCount)
                Debug.Log("scount: " + mCollCount);*/
        }

        void OnCollisionExit2D(Collision2D col) {
            //foreach(ContactPoint cp in col.contacts) {
            //Debug.Log("out: " + cp.otherCollider.name + " n: " + cp.normal);
            //}
            //mCollCount = 0;
            //remove existing information with given collider
            for(int j = mColls.Count - 1; j >= 0; j--) {
                CollideInfo inf = mColls[j];
                if(inf.collider == col.collider) {
                    RemoveColl(j);
                }
            }

            RefreshCollInfo();

            if(collisionExitCallback != null)
                collisionExitCallback(this, col);

            //Debug.Log("exit count: " + mCollCount);
        }

        protected virtual void OnTriggerEnter2D(Collider2D col) {
            if(((1 << col.gameObject.layer) & effectorLayer) != 0) {
                //Debug.Log("Enter: " + col.name);

                mEffectorCount++;
            }

            if(triggerEnterCallback != null)
                triggerEnterCallback(this, col);
        }

        protected virtual void OnTriggerExit2D(Collider2D col) {
            if(((1 << col.gameObject.layer) & effectorLayer) != 0) {
                //Debug.Log("Exit: " + col.name);

                mEffectorCount--;
                if(mEffectorCount < 0) {
                    //Debug.LogWarning("Effect Count Under 0, one of the trigger didn't 'exit'");
                    mEffectorCount = 0;
                }
            }

            if(triggerExitCallback != null)
                triggerExitCallback(this, col);
        }

        protected virtual void OnDisable() {
            ResetCollision();
        }

        protected virtual void OnDestroy() {
            ResetCollision();

            collisionEnterCallback = null;
            collisionStayCallback = null;
            collisionExitCallback = null;
            triggerEnterCallback = null;
            triggerExitCallback = null;
        }

        protected virtual void Awake() {
            if(!dirHolder)
                dirHolder = transform;

            mBody = GetComponent<Rigidbody2D>();

            mColls = new M8.CacheList<CollideInfo>(maxColls);
            mContactPoints = new ContactPoint2D[maxColls];

            //mTopBottomColCos = Mathf.Cos(sphereCollisionAngle * Mathf.Deg2Rad);
            mSlopeLimitCos = Mathf.Cos(slopeLimit * Mathf.Deg2Rad);
            mAboveLimitCos = Mathf.Cos(aboveLimit * Mathf.Deg2Rad);

            mColl = GetComponent<Collider2D>();
            if(mColl != null) {
                if(mColl is CircleCollider2D)
                    mRadius = ((CircleCollider2D)mColl).radius;
                else if(mColl is CapsuleCollider2D) {
                    mCapsuleColl = mColl as CapsuleCollider2D;
                    mRadius = mCapsuleColl.size.y * 0.5f;
                }
            }
        }

        // Update is called once per frame
        protected virtual void FixedUpdate() {
            if(!mBody.simulated)
                return;

#if UNITY_EDITOR
            //mTopBottomColCos = Mathf.Cos(sphereCollisionAngle * Mathf.Deg2Rad);
            mSlopeLimitCos = Mathf.Cos(slopeLimit * Mathf.Deg2Rad);
            mAboveLimitCos = Mathf.Cos(aboveLimit * Mathf.Deg2Rad);
#endif

            //update velocity
            var curVel = mBody.velocity;
            if(mLastVelocity != curVel) {
                mLocalVelocity = dirHolder.worldToLocalMatrix.MultiplyVector(mBody.velocity);

                bool applyLocalVelocity = false;

                if(speedCap.x > 0.0f) {
                    var spdCapXSqr = speedCap.x * speedCap.x;
                    var spdXSqr = mLocalVelocity.x * mLocalVelocity.x;
                    if(spdXSqr > spdCapXSqr) {
                        mLocalVelocity.x = (mLocalVelocity.x / Mathf.Sqrt(spdXSqr)) * speedCap.x;

                        applyLocalVelocity = true;
                    }
                }

                if(speedCap.y > 0.0f) {
                    var spdCapYSqr = speedCap.y * speedCap.y;
                    var spdYSqr = mLocalVelocity.y * mLocalVelocity.y;
                    if(spdYSqr > spdCapYSqr) {
                        mLocalVelocity.y = (mLocalVelocity.y / Mathf.Sqrt(spdYSqr)) * speedCap.y;

                        applyLocalVelocity = true;
                    }
                }

                if(applyLocalVelocity) //local velocity modified, apply back to body
                    mBody.velocity = curVel = dirHolder.localToWorldMatrix.MultiplyVector(mLocalVelocity);

                mLastVelocity = curVel;
            }
            //

            //slide
            if(mIsSlopSlide) {
                Vector2 up = dirHolder.up;
                Vector2 dir = MathUtil.Slide(-up, mSlopNormal);
                dir.Normalize();
                mBody.AddForce(dir * slopSlideForce * moveScale);
            }
            //

            if(mCurMoveAxis != Vector2.zero) {
                float rotAngle = dirHolder.eulerAngles.z;
                Vector2 moveAxis = rotAngle != 0f ? M8.MathUtil.Rotate(mCurMoveAxis, rotAngle) : mCurMoveAxis;

                //move
                if(isGrounded) {
                    if(mLockDragCounter == 0)
                        mBody.drag = isInEffector ? effectorDrag : groundDrag;

                    Move(moveAxis, moveForce);
                }
                else {
                    if(mLockDragCounter == 0)
                        mBody.drag = isInEffector ? effectorDrag : airDrag;

                    Move(moveAxis, moveAirForce);
                }
            }
            else {
                mCurMoveDir = Vector2.zero;

                if(mLockDragCounter == 0 && !mBody.isKinematic)
                    mBody.drag = isInEffector ? effectorDrag : isGrounded && !mIsSlopSlide ? (standDragLayer & mCollLayerMask) == 0 ? groundDrag : standDrag : airDrag;
            }
        }

        //return true if we moved
        public bool Move(Vector2 axis, float force) {
            //compute move direction
            Vector2 moveDelta = axis;

            mCurMoveDir = moveDelta.normalized;

            float maxSpeed = 0.0f;

            //allow for moving diagonally downwards
            //TODO: test for non-platformer
            if(isGrounded) {
                for(int i = 0; i < mColls.Count; i++) {
                    CollideInfo inf = mColls[i];
                    if(inf.flag == CollisionFlags.Below) {
                        Vector2 moveDeltaX = new Vector2(moveDelta.x, 0f);
                        moveDeltaX = M8.MathUtil.Slide(moveDeltaX, inf.normal);

                        moveDelta = new Vector2(moveDeltaX.x, moveDeltaX.y + moveDelta.y);

                        mCurMoveDir = moveDelta.normalized;
                        break;
                    }
                }

                maxSpeed = moveMaxSpeed;
            }
            else {
                maxSpeed = airMaxSpeed;
            }

#if MATE_PHYSICS_DEBUG
            M8.DebugUtil.DrawArrow(transform.position, mCurMoveDir);
#endif

            //check if we can move based on speed or if going against new direction
            bool canMove = CanMove(mCurMoveDir, maxSpeed * moveScale);

            if(canMove) {
                //M8.Debug.
                mBody.AddForce(moveDelta * force * moveScale);
                return true;
            }

            return false;
        }

        protected virtual bool CanMove(Vector2 dir, float maxSpeed) {
            var vel = mBody.velocity;

            float sqrMag = vel.sqrMagnitude;

            bool ret = sqrMag < maxSpeed * maxSpeed;

            //see if we are trying to move the opposite dir
            if(!ret) { //see if we are trying to move the opposite dir
                Vector2 velDir = vel.normalized;
                ret = Vector2.Dot(dir, velDir) < moveCosCheck;
            }

            return ret;
        }

        protected virtual void RefreshCollInfo() {
            mCollFlags = CollisionFlags.None;
            mCollLayerMask = 0;
            mIsSlopSlide = false;

            bool groundNoSlope = false; //prevent slope slide if we are also touching a non-slidable ground (standing on the corner base of slope)

            Vector2 up = dirHolder.up;
            //

            for(int i = mColls.Count - 1; i >= 0; i--) {
                CollideInfo inf = mColls[i];
                if(inf.collider == null || inf.collider.gameObject == null || !inf.collider.gameObject.activeInHierarchy) {
                    RemoveColl(i);
                    continue;
                }

                Vector2 n = inf.normal;
                CollisionFlags flag = inf.flag;

                mCollFlags |= inf.flag;

                if(flag == CollisionFlags.Below || flag == CollisionFlags.Sides) {
                    //sliding
                    if(!(slideDisable || groundNoSlope)) {
                        float a = Vector2.Angle(up, n);
                        mIsSlopSlide = a > slopeLimit && a <= slideLimit;
                        if(mIsSlopSlide) {
                            //Debug.Log("a: " + a);
                            mSlopNormal = n;
                        }
                        else if(flag == CollisionFlags.Below) {
                            mIsSlopSlide = false;
                            groundNoSlope = true;
                        }
                    }

                    mCollLayerMask |= 1 << inf.collider.gameObject.layer;
                }
            }

            mEffectorCount = 0;
        }
    }
}
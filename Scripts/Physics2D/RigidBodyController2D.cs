using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Physics2D/RigidBodyController")]
    public class RigidBodyController2D : MonoBehaviour {
        public const int collisionInfoCapacity = 8;
        public const int contactPointCapacity = 8;

        protected const float moveCosCheck = 0.01745240643728351281941897851632f; //cos(89)

        public class CollisionInfo {
            public Collider2D collider { get; private set; }
            public Collider2D otherCollider { get; private set; }
            public int contactCount { get; private set; }

            private ContactPoint2D[] mContacts = new ContactPoint2D[contactPointCapacity];

            public ContactPoint2D GetContact(int index) {
                return mContacts[index];
            }

            public void Reset() {
                collider = null;
                otherCollider = null;
                contactCount = 0;
            }

            public void Apply(Collision2D coll) {
                collider = coll.collider;
                otherCollider = coll.otherCollider;
                contactCount = coll.GetContacts(mContacts);
            }
        }

        public Transform dirHolder; //the forward vector of this determines our forward movement, put this as a child of this gameobject
                                    //you'll want this as an attach for camera as well.

        public Vector2 speedCap = Vector2.zero; //set to > 0 to cap speed

        public float moveForce = 30.0f;
        public float moveAirForce = 5.0f;

        public bool moveMaxSpeedLimit; //if true, don't move based on moveMaxSpeed limit
        public Vector2 moveMaxSpeed = new Vector2(10f, 10000f); //for horizontal and vertical, local speed. If limit is reached, that axis is not considered when adding force
        public Vector2 moveMaxAirSpeed = new Vector2(6f, 10000f);

        public float slideForce = 40.0f;
        
        public float slopeLimit = 50.0f; //if we are standing still and slope is high, just use groundDrag, also determines collideflag below
        public float aboveLimit = 145.0f; //determines collideflag above, should be > 90, around 140'ish
        public float slideLimit = 80.0f;
        public bool slideDisable;

        public Rigidbody2D body { get; private set; }
        public Collider2D bodyCollision { get; private set; }

        public float moveVertical { get { return mCurMoveAxis.y; } set { mCurMoveAxis.y = value; } }
        public float moveHorizontal { get { return mCurMoveAxis.x; } set { mCurMoveAxis.x = value; } }

        public bool isSlide { get { return mIsSlide; } }
        
        public CollisionFlags collisionFlags { get { return mCollFlags; } }
        public bool isGrounded { get { return (mCollFlags & CollisionFlags.Below) != 0; } }

        public int collisionCount { get; private set; }

        public float radius { get { return mRadius; } }

        public Vector2 localVelocity {
            get { return mLocalVelocity; }
            set {
                if(mLocalVelocity != value) {
                    mLocalVelocity = value;

                    //update body
                    if(body.simulated)
                        body.velocity = dirHolder.localToWorldMatrix.MultiplyVector(mLocalVelocity);
                }
            }
        }

        public float moveScale {
            get { return mMoveScale; }
            set { mMoveScale = value; }
        }

        public Vector2 normalGround { get; private set; }
        public Vector2 normalSide { get; private set; }
        public Vector2 normalAbove { get; private set; }

        protected CollisionInfo[] mColls;

        protected CollisionFlags mCollFlags;
        protected int mCollLayerMask = 0; //layer masks that are colliding us (ground and side)

        private Vector2 mCurMoveAxis;
                
        private float mSlopeLimitCos;
        private float mAboveLimitCos;
        private float mSlideLimitCos;

        private bool mIsSlide;

        private Vector2 mLocalVelocity;
        private Vector2 mLastVelocity;

        private float mRadius = 0.0f;
        
        private float mMoveScale = 1.0f;

        /// <summary>
        /// Collision infos that are currently in contact. Use collisionCount to iterate.
        /// </summary>
        public CollisionInfo GetCollisionInfo(int index) {
            return mColls[index];
        }

        /// <summary>
        /// Get the first occurence of CollideInfo based on given flags. If collider is null, then no match is found
        /// </summary>
        public ContactPoint2D TryGetContact(CollisionFlags flags) {
            Vector2 up = dirHolder.up;

            for(int i = 0; i < collisionCount; i++) {
                var coll = mColls[i];

                for(int j = 0; j < coll.contactCount; j++) {
                    var contact = coll.GetContact(j);

                    var contactFlag = GetCollisionFlag(up, contact.normal);
                    if((contactFlag & flags) != CollisionFlags.None) {
                        return contact;
                    }
                }
            }

            return new ContactPoint2D();
        }

        /// <summary>
        /// Check if given collision is currently colliding with this object.
        /// </summary>
        public bool CheckCollide(Collider col) {
            for(int i = 0; i < collisionCount; i++) {
                if(mColls[i].collider == col)
                    return true;
            }

            return false;
        }

        public virtual void ResetCollision() {
            if(mColls == null) return; //not initialized yet

            mCollFlags = CollisionFlags.None;
            mCollLayerMask = 0;
            mIsSlide = false;            
            mCurMoveAxis = Vector2.zero;
            mLastVelocity = Vector2.zero;

            ResetCollisionInfo();
        }

        private void ResetCollisionInfo() {
            for(int i = 0; i < collisionCount; i++)
                mColls[i].Reset();

            collisionCount = 0;
        }

        private void AddCollisionInfo(Collision2D collision) {
            if(collisionCount < collisionInfoCapacity) {
                mColls[collisionCount].Apply(collision);
                collisionCount++;
            }

            //TODO: expand limit?
        }

        private void RemoveCollisionInfo(Collision2D collision) {
            for(int i = 0; i < collisionCount; i++) {
                var coll = mColls[i];
                if(coll.collider == collision.collider && coll.otherCollider == collision.otherCollider) {
                    mColls[i] = mColls[collisionCount - 1]; //swap from last
                    coll.Reset();
                    collisionCount--;
                    break;
                }
            }
        }

        void GetCapsuleInfo(out Vector2 p1, out Vector2 p2, out float r, float reduceOfs) {
            p1 = bodyCollision.offset;
            p2 = p1;

            var capsuleColl = bodyCollision as CapsuleCollider2D;
            if(!capsuleColl) {
                r = 0f;
                return;
            }

            float h = capsuleColl.size.y - reduceOfs;
            float hHalf = h * 0.5f;

            r = capsuleColl.size.x * 0.5f;

            switch(capsuleColl.direction) {
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
            if(bodyCollision is CapsuleCollider2D) {
                var capsuleColl = (CapsuleCollider2D)bodyCollision;

                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(capsuleColl.offset);
                Vector2 collSize = capsuleColl.size; collSize.y -= reduceOfs;

                return Physics2D.OverlapCapsule(collPos, collSize, capsuleColl.direction, collT.eulerAngles.z, mask) != null;
            }
            else {
                return Physics2D.OverlapCircle(transform.position, mRadius - reduceOfs, mask) != null;
            }
        }

        public bool CheckCast(float reduceOfs, Vector2 dir, out RaycastHit2D hit, float dist, int mask) {
            if(bodyCollision is CapsuleCollider2D) {
                var capsuleColl = (CapsuleCollider2D)bodyCollision;

                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(capsuleColl.offset);
                Vector2 collSize = capsuleColl.size; collSize.y -= reduceOfs;

                hit = Physics2D.CapsuleCast(collPos, collSize, capsuleColl.direction, collT.eulerAngles.z, dir, dist, mask);
                return hit.collider != null;
            }
            else {
                hit = Physics2D.CircleCast(transform.position, mRadius - reduceOfs, dir, dist, mask);
                return hit.collider != null;
            }
        }

        public RaycastHit2D[] CheckAllCasts(Vector2 posOfs, float reduceOfs, Vector2 dir, float dist, int mask) {
            if(bodyCollision is CapsuleCollider2D) {
                var capsuleColl = (CapsuleCollider2D)bodyCollision;

                Transform collT = transform;
                Vector2 collPos = collT.position + collT.localToWorldMatrix.MultiplyPoint3x4(capsuleColl.offset + posOfs);
                Vector2 collSize = capsuleColl.size; collSize.y -= reduceOfs;

                return Physics2D.CapsuleCastAll(collPos, collSize, capsuleColl.direction, collT.eulerAngles.z, dir, dist, mask);
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
        
        void OnCollisionEnter2D(Collision2D col) {
            AddCollisionInfo(col);
        }

        void OnCollisionStay2D(Collision2D col) {
            //update collision info
            for(int i = collisionCount - 1; i >= 0; i--) {
                var colInf = mColls[i];

                //invalid?
                if(!colInf.collider || !colInf.otherCollider) {
                    mColls[i] = mColls[collisionCount - 1];
                    colInf.Reset();
                    collisionCount--;
                    continue;
                }

                if(colInf.collider == col.collider && colInf.otherCollider == col.otherCollider) {
                    colInf.Apply(col);
                    return;
                }
            }

            //not found, add
            AddCollisionInfo(col);
        }

        void OnCollisionExit2D(Collision2D col) {
            RemoveCollisionInfo(col);
        }
        
        protected virtual void OnDisable() {
            ResetCollision();
        }

        protected virtual void OnDestroy() {
            ResetCollision();
        }

        protected virtual void Awake() {
            if(!dirHolder)
                dirHolder = transform;

            body = GetComponent<Rigidbody2D>();

            //mTopBottomColCos = Mathf.Cos(sphereCollisionAngle * Mathf.Deg2Rad);
            mSlopeLimitCos = Mathf.Cos(slopeLimit * Mathf.Deg2Rad);
            mAboveLimitCos = Mathf.Cos(aboveLimit * Mathf.Deg2Rad);
            mSlideLimitCos = Mathf.Cos(slideLimit * Mathf.Deg2Rad);

            bodyCollision = GetComponent<Collider2D>();
            if(bodyCollision != null) {
                if(bodyCollision is CircleCollider2D)
                    mRadius = ((CircleCollider2D)bodyCollision).radius;
                else if(bodyCollision is CapsuleCollider2D) {
                    var capsuleColl = (CapsuleCollider2D)bodyCollision;
                    mRadius = capsuleColl.size.y * 0.5f;
                }
            }

            mColls = new CollisionInfo[collisionInfoCapacity];
            for(int i = 0; i < mColls.Length; i++)
                mColls[i] = new CollisionInfo();
        }

        // Update is called once per frame
        protected virtual void FixedUpdate() {
            if(!body.simulated)
                return;

#if UNITY_EDITOR
            //mTopBottomColCos = Mathf.Cos(sphereCollisionAngle * Mathf.Deg2Rad);
            mSlopeLimitCos = Mathf.Cos(slopeLimit * Mathf.Deg2Rad);
            mAboveLimitCos = Mathf.Cos(aboveLimit * Mathf.Deg2Rad);
            mSlideLimitCos = Mathf.Cos(slideLimit * Mathf.Deg2Rad);
#endif

            //update velocity
            var curVel = body.velocity;
            if(mLastVelocity != curVel) {
                mLocalVelocity = dirHolder.worldToLocalMatrix.MultiplyVector(body.velocity);

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
                    body.velocity = curVel = dirHolder.localToWorldMatrix.MultiplyVector(mLocalVelocity);

                mLastVelocity = curVel;
            }
            //

            Vector2 up = dirHolder.up;
            float rotAngle = dirHolder.eulerAngles.z;

            //setup move axis
            bool axisHorzEnabled = mCurMoveAxis.x != 0f;
            Vector2 axisHorz = new Vector2(mCurMoveAxis.x, 0f);
            bool axisVertEnabled = mCurMoveAxis.y != 0f;
            Vector2 axisVert = new Vector2(0f, mCurMoveAxis.y);

            if(moveMaxSpeedLimit) {
                var _maxAxisSpeed = isGrounded ? moveMaxSpeed : moveMaxAirSpeed;

                //limit axis based on speed limit
                if((axisHorz.x < 0f && localVelocity.x < 0f && -localVelocity.x > _maxAxisSpeed.x) || (axisHorz.x > 0f && localVelocity.x > 0f && localVelocity.x > _maxAxisSpeed.x))
                    axisHorzEnabled = false;

                if((axisVert.y < 0f && localVelocity.y < 0f && -localVelocity.y > _maxAxisSpeed.y) || (axisVert.y > 0f && localVelocity.y > 0f && localVelocity.y > _maxAxisSpeed.y))
                    axisVertEnabled = false;
            }

            //rotate move axis            
            if(rotAngle != 0f) {
                if(axisHorzEnabled)
                    axisHorz = MathUtil.Rotate(axisHorz, rotAngle);
                if(axisVertEnabled)
                    axisVert = MathUtil.Rotate(axisVert, rotAngle);
            }

            //update flags and such
            mCollFlags = CollisionFlags.None;
            mCollLayerMask = 0;
            mIsSlide = false;

            bool groundNoSlope = false; //prevent slope slide if we are also touching a non-slidable ground (standing on the corner base of slope)

            normalGround = Vector2.zero;
            int normalGroundCount = 0;
            normalAbove = Vector2.zero;
            int normalAboveCount = 0;
            normalSide = Vector2.zero;
            int normalSideCount = 0;

            int horzSlideCount = 0;

            for(int i = 0; i < collisionCount; i++) {
                var coll = mColls[i];

                if(!coll.collider)
                    continue;

                mCollLayerMask |= 1 << coll.collider.gameObject.layer;
                                
                for(int j = 0; j < coll.contactCount; j++) {
                    var contact = coll.GetContact(j);

                    //generate flag
                    CollisionFlags contactFlag;

                    float dot = Vector2.Dot(up, contact.normal);

                    //Debug.Log("dot: " + dot);
                    //Debug.Log("deg: " + (Mathf.Acos(dot)*Mathf.Rad2Deg));

                    if(dot >= mSlopeLimitCos) {
                        contactFlag = CollisionFlags.Below;
                        normalGround += contact.normal;
                        normalGroundCount++;
                    }
                    else if(dot <= mAboveLimitCos) {
                        contactFlag = CollisionFlags.Above;
                        normalAbove += contact.normal;
                        normalAboveCount++;
                    }
                    else {
                        contactFlag = CollisionFlags.Sides;
                        normalSide += contact.normal;
                        normalSideCount++;
                    }
                    //

                    //update slide
                    if(!(slideDisable || groundNoSlope) && (contactFlag == CollisionFlags.Below || contactFlag == CollisionFlags.Sides)) {
                        //sliding
                        mIsSlide = dot >= mSlideLimitCos && dot < mSlopeLimitCos;
                        if(!mIsSlide && contactFlag == CollisionFlags.Below)
                            groundNoSlope = true;
                    }

                    //allow horizontal to move along ground (useful for moving down a slope)
                    if(axisHorzEnabled && contactFlag == CollisionFlags.Below) {
                        var horzSlide = MathUtil.Slide(axisHorz, contact.normal);
                        if(horzSlideCount == 0)
                            axisHorz = horzSlide;
                        else
                            axisHorz += horzSlide;

                        horzSlideCount++;
                    }

                    mCollFlags |= contactFlag;
                }
            }

            if(normalGroundCount > 1)
                normalGround = (normalGround / normalGroundCount).normalized;
            if(normalAboveCount > 1)
                normalAbove = (normalAbove / normalAboveCount).normalized;
            if(normalSideCount > 1)
                normalSide = (normalSide / normalSideCount).normalized;

            if(horzSlideCount > 1)
                axisHorz /= horzSlideCount;
            //

#if MATE_PHYSICS_DEBUG
            M8.DebugUtil.DrawArrow(transform.position, mCurMoveDir);
#endif
                //

            Vector2 force = Vector2.zero;

            //slide
            if(mIsSlide) {
                Vector2 dir = MathUtil.Slide(-up, normalSide);
                dir.Normalize();
                force += dir * slideForce;
            }
            //

            //move
            if(axisHorzEnabled)
                force += axisHorz * moveForce;

            if(axisVertEnabled)
                force += axisVert * moveForce;
            //

            if(force != Vector2.zero)
                body.AddForce(force * moveScale);
        }
    }
}
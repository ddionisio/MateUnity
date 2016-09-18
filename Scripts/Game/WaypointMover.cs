using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Use to move given transform
    /// </summary>
    [AddComponentMenu("M8/Game/WaypointMover")]
    public class WaypointMover : MonoBehaviour {
        public enum Type {
            Loop,
            Once,
            Reverse,
            Stay
        }

        public enum Move {
            Lerp,
            EaseIn,
            EaseOut,
            Damp
        }

        //for use with saving states
        public class SaveState {
            private WaypointMover mMover;

            private bool mPaused = false;

            private int mCurInd;
            private int mNextInd;
            private bool mReversed;

            private Vector3 mCurPos;
            private float mCurTime;

            public SaveState(WaypointMover mover) {
                mMover = mover;

                mCurPos = mover.target.position;
                mPaused = mover.mPaused;
                mCurInd = mover.mCurInd;
                mNextInd = mover.mNextInd;
                mReversed = mover.mReversed;
                mCurTime = mover.mCurTime;
            }

            public void Restore() {
                mMover.target.position = mCurPos;
                mMover.mPaused = mPaused;
                mMover.mCurInd = mCurInd;
                mMover.mNextInd = mNextInd;
                mMover.mReversed = mReversed;
                mMover.mCurTime = mCurTime;

                Vector3 mStartPos = mMover.mWPs[mCurInd].position, mEndPos = mMover.mWPs[mNextInd].position;

                mMover.mDir = (mEndPos - mStartPos).normalized;

                mMover.mCurVel = Vector3.zero;

                mMover.mRestore = true;
            }
        }

        public delegate void OnMoveCall(WaypointMover wm);

        public string waypoint;

        public Transform target;

        public Type type;
        public Move move;

        public float startWait;
        public float nodeWait;
        public float moveDelay;

        public int startIndex = 0;

        public bool startPause = false;

        public event OnMoveCall moveBeginCallback; //starting to move to destination
        public event OnMoveCall movePauseCallback; //starting to move to destination

        private bool mPaused = false;

        private int mCurInd;
        private int mNextInd;
        private bool mReversed;

        private Vector3 mCurVel;
        private Vector3 mDir;

        private bool mStarted = false;

        private List<Transform> mWPs;

        private float mCurTime;

        private WaitForFixedUpdate mWaitUpdate;
        private WaitForSeconds mWaitStartDelay;
        private WaitForSeconds mWaitDelay;

        private Rigidbody mTargetBody;
        private Collider mTargetColl;

        private bool mRestore = false;

        public bool pause {
            get { return mPaused; }
            set {
                if(mPaused != value) {
                    mPaused = value;

                    if(mPaused) {
                        if(movePauseCallback != null)
                            movePauseCallback(this);
                    }
                    else {
                        if(moveBeginCallback != null)
                            moveBeginCallback(this);
                    }
                }
            }
        }

        public Vector3 dir {
            get { return mDir; }
        }

        public bool reverse {
            get { return mReversed; }
            set {
                if(mReversed != value) {
                    mReversed = value;

                    int curInd = mCurInd;
                    mCurInd = mNextInd;
                    mNextInd = curInd;

                    if(type == Type.Stay && mCurInd == mNextInd) {
                        if(mNextInd == mWPs.Count - 1)
                            mNextInd = mWPs.Count - 2;
                        else
                            mNextInd = 1;

                        SetCurrent();
                    }
                    else {
                        mDir *= -1.0f;
                        mCurVel *= -1.0f;
                        mCurTime = Mathf.Clamp(moveDelay - mCurTime, 0.0f, moveDelay);
                    }

                    mRestore = true;
                }
            }
        }

        public Vector3 curVelocity {
            get { return mCurVel; }
        }

        void OnDestroy() {
            moveBeginCallback = null;
            movePauseCallback = null;
        }

        void OnEnable() {
            if(mStarted) {
                if(mWPs.Count > 1)
                    StartCoroutine(DoIt());
                else {
                    mDir = Vector3.zero;
                    mCurVel = Vector3.zero;
                    ApplyPosition(mWPs[0].position, false);
                }
            }
        }

        void Awake() {
            if(target == null)
                target = transform;

            mTargetBody = target.GetComponent<Rigidbody>();
            mTargetColl = target.GetComponent<Collider>();

            mWaitUpdate = new WaitForFixedUpdate();
            mWaitStartDelay = new WaitForSeconds(startWait);
            mWaitDelay = nodeWait > 0.0f ? new WaitForSeconds(nodeWait) : null;
        }

        // Use this for initialization
        void Start() {
            mStarted = true;

            mWPs = WaypointManager.instance.GetWaypoints(waypoint);

            if(mWPs.Count > 1)
                StartCoroutine(DoIt());
            else
                ApplyPosition(mWPs[startIndex].position, false);
        }

        IEnumerator DoIt() {
            if(!mRestore) {
                //reset data
                mCurInd = startIndex;
                mReversed = false;
                mPaused = startPause;

                yield return mWaitStartDelay;
            }

            do {
                if(mRestore) {
                    mRestore = false;
                }
                else {
                    SetCurrent();

                    if(!mPaused) {
                        if(moveBeginCallback != null)
                            moveBeginCallback(this);
                    }
                }

                while(!mRestore && type == Type.Stay && mCurInd == mNextInd)
                    yield return mWaitUpdate;

                //move it
                while(mCurTime < moveDelay) {
                    Vector3 mStartPos = mWPs[mCurInd].position, mEndPos = mWPs[mNextInd].position;

                    if(!mPaused) {
                        mCurTime += Time.fixedDeltaTime;

                        switch(move) {
                            case Move.Lerp:
                                ApplyPosition(Vector3.Lerp(mStartPos, mEndPos, mCurTime / moveDelay), true);
                                break;

                            case Move.EaseIn:
                                ApplyPosition(
                                    new Vector3(
                                        Easing.In(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                        Easing.In(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                        Easing.In(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z)), true);
                                break;

                            case Move.EaseOut:
                                ApplyPosition(
                                    new Vector3(
                                        Easing.Out(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                        Easing.Out(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                        Easing.Out(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z)), true);
                                break;

                            case Move.Damp:
                                ApplyPosition(Vector3.SmoothDamp(target.position, mEndPos, ref mCurVel, Time.fixedDeltaTime), false);
                                break;
                        }
                    }

                    yield return mWaitUpdate;
                }

                ApplyPosition(mWPs[mNextInd].position, true);

                if(mWaitDelay != null) {
                    if(movePauseCallback != null)
                        movePauseCallback(this);

                    yield return mWaitDelay;
                }
            } while(mRestore || !Next());

            yield break;
        }

        void SetCurrent() {
            Vector3 mStartPos, mEndPos;

            mStartPos = mWPs[mCurInd].position;

            switch(move) {
                case Move.Damp:
                    ApplyPosition(mWPs[mCurInd].position, false);
                    break;
            }

            mCurTime = 0.0f;

            mCurVel = Vector3.zero;

            switch(type) {
                case Type.Loop:
                    mNextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                    if(mNextInd < 0) {
                        mNextInd = 1;
                    }
                    else if(mNextInd >= mWPs.Count) {
                        mNextInd = 0;
                    }
                    break;

                case Type.Reverse:
                    mNextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                    if(mNextInd < 0) {
                        mNextInd = 1;
                    }
                    else if(mNextInd >= mWPs.Count) {
                        mNextInd = mCurInd - 1;
                    }
                    break;

                default:
                    mNextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                    if(mNextInd < 0 || mNextInd >= mWPs.Count) {
                        mNextInd = mCurInd;
                    }
                    break;
            }

            mEndPos = mWPs[mNextInd].position;

            mDir = (mEndPos - mStartPos).normalized;
        }

        //true when done
        bool Next() {
            bool ret = false;

            mCurInd = mNextInd;

            switch(type) {
                case Type.Reverse:
                    if(mCurInd == 0 || mCurInd == mWPs.Count - 1) {
                        mReversed = !mReversed;
                    }
                    break;

                case Type.Stay:
                    if(mCurInd == mNextInd)
                        mCurTime = moveDelay;
                    break;

                case Type.Once:
                    ret = mCurInd == mNextInd;
                    break;
            }

            return ret;
        }

        void ApplyPosition(Vector3 pos, bool computeVel) {
            if(computeVel) {
                Vector3 dpos = pos - target.position;
                mCurVel = dpos / Time.fixedDeltaTime;
            }

            if(mTargetColl != null)
                pos -= target.worldToLocalMatrix.MultiplyPoint(mTargetColl.bounds.center);

            if(mTargetBody != null)
                mTargetBody.MovePosition(pos);
            else
                target.position = pos;
        }
    }
}
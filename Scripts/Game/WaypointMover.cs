using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Use to move given transform
/// </summary>
[AddComponentMenu("M8/Game/WaypointMover")]
public class WaypointMover : MonoBehaviour {
    public enum Type {
        Loop,
        Once,
        Reverse
    }

    public enum Move {
        Lerp,
        EaseIn,
        EaseOut,
        Damp
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

    public event OnMoveCall moveBeginCallback; //starting to move to destination
    public event OnMoveCall movePauseCallback; //starting to move to destination

    private bool mPaused = false;

    private int mCurInd;
    private bool mReversed;

    private Vector3 mCurVel;
    private Vector3 mStartPos;
    private Vector3 mEndPos;
    private Vector3 mDir;

    private bool mStarted = false;

    private List<Transform> mWPs;

    private float mCurTime;

    private WaitForFixedUpdate mWaitUpdate;
    private WaitForSeconds mWaitStartDelay;
    private WaitForSeconds mWaitDelay;

    private Rigidbody mTargetBody;

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
                Vector3 pos = mStartPos;
                mStartPos = mEndPos;
                mEndPos = pos;
                mDir *= -1.0f;
                mCurVel *= -1.0f;
                mCurTime = Mathf.Clamp(moveDelay - mCurTime, 0.0f, moveDelay);

                Next();

                mReversed = value;
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

        mTargetBody = target.rigidbody;

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
        //reset data
        mCurInd = startIndex;
        mReversed = false;
        mPaused = false;

        yield return mWaitStartDelay;

        do {
            SetCurrent(false);

            if(!mPaused) {
                if(moveBeginCallback != null)
                    moveBeginCallback(this);
            }

            //move it
            while(mCurTime < moveDelay) {
                if(!mPaused) {
                    mCurTime += Time.fixedDeltaTime;

                    switch(move) {
                        case Move.Lerp:
                            ApplyPosition(Vector3.Lerp(mStartPos, mEndPos, mCurTime / moveDelay), true);
                            break;

                        case Move.EaseIn:
                            ApplyPosition(
                                new Vector3(
                                    M8.Ease.In(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                    M8.Ease.In(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                    M8.Ease.In(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z)), true);
                            break;

                        case Move.EaseOut:
                            ApplyPosition(
                                new Vector3(
                                    M8.Ease.Out(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                    M8.Ease.Out(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                    M8.Ease.Out(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z)), true);
                            break;

                        case Move.Damp:
                            ApplyPosition(Vector3.SmoothDamp(target.position, mEndPos, ref mCurVel, Time.fixedDeltaTime), false);
                            break;
                    }
                }

                yield return mWaitUpdate;
            }

            ApplyPosition(mEndPos, true);

            if(mWaitDelay != null) {
                if(movePauseCallback != null)
                    movePauseCallback(this);

                yield return mWaitDelay;
            }

        } while(!Next());

        yield break;
    }

    void SetCurrent(bool isUpdate) {
        if(isUpdate) {
            mStartPos = target.position;

            mCurTime = Mathf.Clamp(moveDelay - mCurTime, 0.0f, moveDelay);
        }
        else {
            switch(move) {
                case Move.Damp:
                    ApplyPosition(mWPs[mCurInd].position, false);
                    break;

                default:
                    mStartPos = mWPs[mCurInd].position;
                    break;
            }

            mCurTime = 0.0f;
        }

        mCurVel = Vector3.zero;

        int nextInd;

        switch(type) {
            case Type.Loop:
                nextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                if(nextInd < 0) {
                    nextInd = 1;
                }
                else if(nextInd >= mWPs.Count) {
                    nextInd = 0;
                }
                break;
      
            case Type.Reverse:
                nextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                if(nextInd < 0) {
                    nextInd = 1;
                }
                else if(nextInd >= mWPs.Count) {
                    nextInd = mCurInd - 1;
                }
                break;

            default:
                nextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                if(nextInd < 0 || nextInd >= mWPs.Count) {
                    nextInd = mCurInd;
                }
                break;
        }

        mEndPos = mWPs[nextInd].position;

        mDir = (mEndPos - mStartPos).normalized;
    }

    //true when done
    bool Next() {
        bool ret = false;

        switch(type) {
            case Type.Loop:
                mCurInd += mReversed ? -1 : 1;
                if(mCurInd >= mWPs.Count)
                    mCurInd = 0;
                else if(mCurInd < 0)
                    mCurInd = mWPs.Count - 1;
                break;

            case Type.Reverse:
                mCurInd += mReversed ? -1 : 1;
                if(mCurInd < 0) {
                    mCurInd = 1;
                    mReversed = false;
                }
                else if(mCurInd >= mWPs.Count) {
                    mCurInd = mWPs.Count - 2;
                    mReversed = true;
                }
                break;

            case Type.Once:
                mCurInd += mReversed ? -1 : 1;
                if(mCurInd >= mWPs.Count) {
                    mCurInd = mWPs.Count - 1;
                    ret = true;
                }
                else if(mCurInd < 0) {
                    mCurInd = 0;
                    ret = true;
                }
                break;
        }

        return ret;
    }

    void ApplyPosition(Vector3 pos, bool computeVel) {
        if(computeVel) {
            Vector3 dpos = pos - target.position;
            mCurVel = dpos / Time.fixedDeltaTime;
        }

        if(mTargetBody != null)
            mTargetBody.MovePosition(pos);
        else
            target.position = pos;
    }
}

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

    void OnDestroy() {
        moveBeginCallback = null;
        movePauseCallback = null;
    }

    void OnEnable() {
        if(mStarted) {
            if(mWPs.Count > 1)
                StartCoroutine(DoIt());
            else
                ApplyPosition(mWPs[0].position);
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
            ApplyPosition(mWPs[startIndex].position);
    }

    IEnumerator DoIt() {
        //reset data
        mCurInd = startIndex;
        mReversed = false;
        mPaused = false;

        yield return mWaitStartDelay;

        do {
            SetCurrent();

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
                            ApplyPosition(Vector3.Lerp(mStartPos, mEndPos, mCurTime / moveDelay));
                            break;

                        case Move.EaseIn:
                            ApplyPosition(
                                new Vector3(
                                    M8.Ease.In(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                    M8.Ease.In(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                    M8.Ease.In(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z)));
                            break;

                        case Move.EaseOut:
                            ApplyPosition(
                                new Vector3(
                                    M8.Ease.Out(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                    M8.Ease.Out(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                    M8.Ease.Out(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z)));
                            break;

                        case Move.Damp:
                            ApplyPosition(Vector3.SmoothDamp(target.position, mEndPos, ref mCurVel, Time.fixedDeltaTime));
                            break;
                    }
                }

                yield return mWaitUpdate;
            }

            ApplyPosition(mEndPos);

            if(mWaitDelay != null) {
                if(movePauseCallback != null)
                    movePauseCallback(this);

                yield return mWaitDelay;
            }

        } while(!Next());

        yield break;
    }

    void SetCurrent() {
        switch(move) {
            case Move.Damp:
                mCurVel = Vector3.zero;
                ApplyPosition(mWPs[mCurInd].position);
                break;

            default:
                mStartPos = mWPs[mCurInd].position;
                break;
        }

        int nextInd;

        switch(type) {
            case Type.Loop:
                nextInd = mCurInd + 1;
                if(nextInd == mWPs.Count) {
                    nextInd = 0;
                }
                break;
      
            case Type.Reverse:
                nextInd = mReversed ? mCurInd - 1 : mCurInd + 1;
                if(nextInd < 0) {
                    nextInd = 1;
                }
                else if(nextInd == mWPs.Count) {
                    nextInd = mCurInd - 1;
                }
                break;

            default:
                nextInd = mCurInd + 1;
                if(nextInd == mWPs.Count) {
                    nextInd = mCurInd;
                }
                break;
        }

        mEndPos = mWPs[nextInd].position;

        mDir = (mEndPos - mStartPos).normalized;

        mCurTime = 0.0f;
    }

    //true when done
    bool Next() {
        bool ret = false;

        switch(type) {
            case Type.Loop:
                mCurInd++;
                if(mCurInd == mWPs.Count)
                    mCurInd = 0;
                break;

            case Type.Reverse:
                mCurInd += mReversed ? -1 : 1;
                if(mCurInd < 0) {
                    mCurInd = 1;
                    mReversed = false;
                }
                else if(mCurInd == mWPs.Count) {
                    mCurInd = mWPs.Count - 2;
                    mReversed = true;
                }
                break;

            case Type.Once:
                mCurInd++;
                if(mCurInd == mWPs.Count) {
                    mCurInd = mWPs.Count - 1;
                    ret = true;
                }
                break;
        }

        return ret;
    }

    void ApplyPosition(Vector3 pos) {
        if(mTargetBody != null)
            mTargetBody.MovePosition(pos);
        else
            target.position = pos;
    }
}

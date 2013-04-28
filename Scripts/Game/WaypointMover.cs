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

    public string waypoint;

    public Transform target;

    public Type type;
    public Move move;

    public float startWait;
    public float nodeWait;
    public float moveDelay;

    private int mCurInd;
    private bool mReversed;

    private Vector3 mCurVel;
    private Vector3 mStartPos;
    private Vector3 mEndPos;

    private bool mStarted = false;

    private List<Transform> mWPs;

    private float mCurTime;

    private WaitForFixedUpdate mWaitUpdate;
    private WaitForSeconds mWaitStartDelay;
    private WaitForSeconds mWaitDelay;

    void OnEnable() {
        if(mStarted) {
            if(mWPs.Count > 1)
                StartCoroutine(DoIt());
            else
                target.position = mWPs[0].position;
        }
    }

    void Awake() {
        if(target == null)
            target = transform;

        mWaitUpdate = new WaitForFixedUpdate();
        mWaitStartDelay = new WaitForSeconds(startWait);
        mWaitDelay = new WaitForSeconds(nodeWait);
    }

    // Use this for initialization
    void Start() {
        mStarted = true;

        mWPs = WaypointManager.instance.GetWaypoints(waypoint);

        if(mWPs.Count > 1)
            StartCoroutine(DoIt());
        else
            target.position = mWPs[0].position;
    }

    IEnumerator DoIt() {
        //reset data
        mCurInd = 0;
        mReversed = false;

        yield return mWaitStartDelay;

        do {
            SetCurrent();

            //move it
            while(mCurTime < moveDelay) {
                mCurTime += Time.fixedDeltaTime;

                switch(move) {
                    case Move.Lerp:
                        target.position = Vector3.Lerp(mStartPos, mEndPos, mCurTime / moveDelay);
                        break;

                    case Move.EaseIn:
                        target.position =
                            new Vector3(
                                M8.Ease.In(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                M8.Ease.In(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                M8.Ease.In(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z));
                        break;

                    case Move.EaseOut:
                        target.position =
                            new Vector3(
                                M8.Ease.Out(mCurTime, moveDelay, mStartPos.x, mEndPos.x - mStartPos.x),
                                M8.Ease.Out(mCurTime, moveDelay, mStartPos.y, mEndPos.y - mStartPos.y),
                                M8.Ease.Out(mCurTime, moveDelay, mStartPos.z, mEndPos.z - mStartPos.z));
                        break;

                    case Move.Damp:
                        target.position = Vector3.SmoothDamp(target.position, mEndPos, ref mCurVel, Time.fixedDeltaTime);
                        break;
                }

                yield return mWaitUpdate;
            }

            target.position = mEndPos;

            yield return mWaitDelay;
        } while(!Next());

        yield break;
    }

    void SetCurrent() {
        switch(move) {
            case Move.Damp:
                mCurVel = Vector3.zero;
                target.position = mWPs[mCurInd].position;
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
}

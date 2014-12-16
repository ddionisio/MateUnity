using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Game/WaypointData")]
    public class WaypointData : MonoBehaviour {
        public enum Mode {
            Repeat,
            SeeSaw,
            Random,
        }

        [SerializeField]
        Mode loopMode = Mode.Repeat;

        [SerializeField]
        int loopCount = -1; //-1 = infinite

        [SerializeField]
        string startWaypoint;

        [SerializeField]
        int startIndex = 0;

        [SerializeField]
        bool startReverse = false;

        private int mCurInd = 0;
        private string mCurWaypointName;
        private List<Transform> mWaypoints;
        private int mCounter = 0;
        private bool mReversed = false;

        public string waypoint { get { return mCurWaypointName; } set { Apply(value); } }

        public int currentIndex { get { return mCurInd; } }

        public Transform current { get { return mWaypoints != null ? mWaypoints[mCurInd] : null; } }

        public bool reversed {
            get { return mReversed; }
            set { mReversed = value; }
        }

        public bool isDone {
            get {
                return loopCount == -1 || mWaypoints == null ? false : mCounter < loopCount * mWaypoints.Count;
            }
        }

        public int counter { get { return mCounter; } }

        public void Restart() {
            if(mWaypoints != null) {
                mCurInd = startIndex < mWaypoints.Count ? startIndex : 0;
                mReversed = startReverse;
                mCounter = 0;

                switch(loopMode) {
                    case Mode.Random:
                        M8.Util.ShuffleList(mWaypoints);
                        break;
                }
            }
        }

        public bool Apply(string name) {
            if(mWaypoints == null || mCurWaypointName != name) {
                List<Transform> wps = WaypointManager.instance != null ? WaypointManager.instance.GetWaypoints(name) : null;

                if(wps != null && wps.Count > 0) {
                    mCurWaypointName = name;

                    mCurInd = startIndex < wps.Count ? startIndex : 0;
                    mReversed = startReverse;
                    mCounter = 0;

                    switch(loopMode) {
                        case Mode.Random:
                            mWaypoints = new List<Transform>(wps);
                            M8.Util.ShuffleList(mWaypoints);
                            break;

                        default:
                            mWaypoints = wps;
                            break;
                    }

                    return true;
                }
                else {
                    Debug.LogWarning("Unable to get waypoints for: " + name);
                }
            }

            return false;
        }

        public void Next() {
            if(!isDone) {
                mCounter++;

                //loop completed
                if(mCounter % mWaypoints.Count == 0) {
                    switch(loopMode) {
                        case Mode.Random:
                            M8.Util.ShuffleList(mWaypoints);
                            break;
                    }
                }

                mCurInd = GetNextInd(mReversed ? -1 : 1);
            }
        }

        public void Prev() {
            if(!isDone) {
                if(mCounter > 0) {
                    mCounter--;

                    mCurInd = GetNextInd(mReversed ? 1 : -1);
                }
            }
        }

        int GetNextInd(int delta) {
            int nextInd = mCurInd + delta;

            if(nextInd < 0) {
                switch(loopMode) {
                    case Mode.SeeSaw:
                        mReversed = !mReversed;
                        nextInd = mWaypoints.Count == 1 ? 0 : 1;
                        break;

                    default:
                        nextInd = mWaypoints.Count - 1;
                        break;
                }
            }
            else if(nextInd >= mWaypoints.Count) {
                switch(loopMode) {
                    case Mode.SeeSaw:
                        mReversed = !mReversed;
                        nextInd = mWaypoints.Count == 1 ? 0 : mWaypoints.Count - 2;
                        break;

                    default:
                        nextInd = 0;
                        break;
                }
            }

            return nextInd;
        }

        void Awake() {
            mCurInd = startIndex;
            mReversed = startReverse;
        }

        void Start() {
            if(!string.IsNullOrEmpty(startWaypoint)) {
                Apply(startWaypoint);
            }
        }
    }
}
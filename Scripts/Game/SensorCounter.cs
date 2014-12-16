using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    /// <summary>
    /// Use this to determine number of active objects within a trigger
    /// </summary>
    [AddComponentMenu("M8/Game/SensorCounter")]
    public class SensorCounter : MonoBehaviour {
        public float updateCheckDelay = 0.2f;

        private HashSet<Collider> mUnits = new HashSet<Collider>();
        private WaitForSeconds mWait = null;
        private bool mDoUpdateActive = false;
        private int mLastCount = -1;

#if PLAYMAKER
    private PlayMakerFSM mFSM;
#endif

        public HashSet<Collider> items {
            get {
                return mUnits;
            }
        }

        public int count {
            get {
                return mUnits.Count;
            }
        }

        /// <summary>
        /// returns true if there are changes to mUnits
        /// </summary>
        public bool CleanUp() {
            return mUnits.RemoveWhere(IsUnitInvalid) > 0;
        }

        void OnDisable() {
            mUnits.Clear();

            mDoUpdateActive = false;
        }

        void OnTriggerEnter(Collider other) {
            mUnits.Add(other);

            if(!mDoUpdateActive)
                StartCoroutine(DoCheck());
        }

        void OnTriggerExit(Collider other) {
            mUnits.Remove(other);

            if(!mDoUpdateActive)
                StartCoroutine(DoCheck());
        }

        bool IsUnitInvalid(Collider unit) {
            if(unit != null) {
                if(!unit.gameObject.activeInHierarchy) {
                    return true;
                }

                return false;
            }

            return true;
        }

        void Awake() {
            mWait = new WaitForSeconds(updateCheckDelay);

#if PLAYMAKER
        mFSM = GetComponent<PlayMakerFSM>();
#endif
        }

        IEnumerator DoCheck() {
            mLastCount = -1;
            mDoUpdateActive = true;

            while(true) {
                bool changes = CleanUp();

                if(changes || mLastCount != mUnits.Count) {
#if PLAYMAKER
                mFSM.SendEvent("SensorCounterUpdate");
#endif

                    mLastCount = mUnits.Count;
                }

                if(mUnits.Count == 0)
                    break;

                yield return mWait;
            }

            mDoUpdateActive = false;
        }
    }
}
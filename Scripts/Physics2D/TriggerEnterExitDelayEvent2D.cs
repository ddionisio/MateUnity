using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Physics2D/Trigger Enter and Exit Delay Event")]
    public class TriggerEnterExitDelayEvent2D : MonoBehaviour {
        [Tooltip("Which tags is allowed to invoke callback. Set this to empty to allow any collision.")]
        [TagSelector]
        public string[] tagFilters;
        [Tooltip("Delay to next call when changing from enter to exit, and vice versa.")]
        public float changeDelay = 2f;

        public UnityEventCollider2D enterCallback;
        public UnityEventCollider2D exitCallback;

        private Collider2D mColl;
        private bool mIsTriggered;
        private float mLastTriggerTime;

        private Coroutine mRout;

        void OnTriggerEnter2D(Collider2D collision) {
            if(IsValid(collision)) {
                mColl = collision;
                mIsTriggered = true;
                mLastTriggerTime = Time.time;

                if(mRout == null)
                    mRout = StartCoroutine(DoEnter());
            }
        }

        void OnTriggerExit2D(Collider2D collision) {
            if(IsValid(collision)) {
                mColl = collision;
                mIsTriggered = false;
                mLastTriggerTime = Time.time;

                if(mRout == null)
                    mRout = StartCoroutine(DoExit());
            }
        }

        bool IsValid(Collider2D collision) {
            if(tagFilters.Length > 0) {
                for(int i = 0; i < tagFilters.Length; i++) {
                    var tagFilter = tagFilters[i];
                    if(!string.IsNullOrEmpty(tagFilter) && collision.CompareTag(tagFilter))
                        return true;
                }

                return false;
            }

            return true;
        }

        void OnEnable() {
            mIsTriggered = false;
            mLastTriggerTime = 0f;
        }

        void OnDisable() {
            mColl = null;
            mRout = null;
        }

        IEnumerator DoEnter() {
            enterCallback.Invoke(mColl);

            while(Time.time - mLastTriggerTime < changeDelay)
                yield return null;

            if(!mIsTriggered)
                mRout = StartCoroutine(DoExit());
            else
                mRout = null;
        }

        IEnumerator DoExit() {
            exitCallback.Invoke(mColl);

            while(Time.time - mLastTriggerTime < changeDelay)
                yield return null;

            if(mIsTriggered)
                mRout = StartCoroutine(DoEnter());
            else
                mRout = null;
        }
    }
#endif
}
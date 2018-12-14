using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace M8 {
    /// <summary>
    ///use this to have entities in the map be deactivated at start,
    ///waking up when certain things trigger it
    ///make sure to have this as a child,
    ///destroy it if we are currently active
    ///</summary>
    [AddComponentMenu("M8/Game/Activator")]
    public class Activator : MonoBehaviour {
        public const string activatorGOName = "__activatorHolder";
        //public const string ActivatorHolderTag = "ActivatorHolder"; //make sure this is in the scene root

        public bool deactivateOnEnable = true;
        public float deactivateDelay = 2.0f;

        public UnityEvent awakeEvent;
        public UnityEvent sleepEvent;

        private bool mIsActive = true;

        private Transform mDefaultParent; //the parent when we are instantiated        
        private Vector3 mLocalPos;

        private Coroutine mDeactivateRout;

        private static Transform mActivatorHolder = null;

        /// <summary>
        /// If true, then we are currently waiting for trigger. If false, then parent should be inactive
        /// </summary>
        public bool isActive { get { return mIsActive; } }

        public Transform defaultParent { get { return mDefaultParent; } }

        //TODO: need something for this
        public virtual bool isVisible { get { return true; } }

        /// <summary>
        /// This is usually called via derivatives of this class such as Trigger during Entered
        /// </summary>
        public void Activate() {
            if(!mIsActive) {
                //put ourself back in parent
                mDefaultParent.gameObject.SetActive(true);
                SetParent(mDefaultParent);

                mIsActive = true;

                //yield return new WaitForFixedUpdate();

                awakeEvent.Invoke();
            }
            else
                StopDeactivateRout();
        }

        /// <summary>
        /// This is usually called via derivatives of this class such as Trigger during Exited
        /// </summary>
        public void Deactivate() {
            if(mIsActive) {
                if(deactivateDelay > 0.0f) {
                    if(mDeactivateRout == null)
                        mDeactivateRout = StartCoroutine(DoInActiveDelay());
                }
                else {
                    InActive(true);
                }
            }
        }

        public void ForceDeactivate(bool notifySleep) {
            StopDeactivateRout();
            InActive(notifySleep);
        }
                
        /// <summary>
        /// Call this when you are about to be released or destroyed. If destroy = true, then destroy this object
        /// </summary>
        public virtual void Release(bool destroy) {
            //put ourself back in parent
            if(destroy) {
                Object.Destroy(gameObject);
            }
            else {
                //put back to parent
                SetParent(mDefaultParent);
            }
            
            StopDeactivateRout();
            mIsActive = true;
        }

        protected virtual void OnEnable() {
            if(deactivateOnEnable) {
                InActive(false);
            }
        }

        protected virtual void OnDisable() {
            StopDeactivateRout();
        }

        protected virtual void Awake() {
            if(!mActivatorHolder) {
                var go = GameObject.Find(activatorGOName);
                if(!go) {
                    go = new GameObject(activatorGOName);
                    DontDestroyOnLoad(go);
                }

                mActivatorHolder = go.transform;
            }

            mDefaultParent = transform.parent;
            mLocalPos = transform.localPosition;
        }
                
        protected virtual void SetParent(Transform toParent) {
            if(toParent != mDefaultParent) {
                Vector3 pos = transform.position;
                transform.parent = toParent;
                transform.position = pos;
            }
            else {
                transform.parent = toParent;
                transform.position = toParent.position;
                transform.localPosition = mLocalPos;
            }
        }

        IEnumerator DoInActiveDelay() {
            yield return new WaitForSeconds(deactivateDelay);

            InActive(true);
        }

        protected void InActive(bool notifySleep) {
            if(mIsActive) {
                //StopCoroutine("DoActive");

                //remove from parent
                if(mActivatorHolder != null) {
                    SetParent(mActivatorHolder);
                }
                else {
                    SetParent(null);
                }

                mDefaultParent.gameObject.SetActive(false);

                mIsActive = false;

                if(notifySleep)
                    sleepEvent.Invoke();
            }
        }

        private void StopDeactivateRout() {
            if(mDeactivateRout != null) {
                StopCoroutine(mDeactivateRout);
                mDeactivateRout = null;
            }
        }
    }
}
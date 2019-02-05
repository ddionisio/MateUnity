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
    [AddComponentMenu("M8/Game Object/Activator")]
    public class GOActivator : MonoBehaviour, IPoolDespawn {
        public const string activatorGOName = "__activatorHolder";
        //public const string ActivatorHolderTag = "ActivatorHolder"; //make sure this is in the scene root

        public bool deactivateOnEnable = true;
        public float deactivateDelay = 2.0f;
        public bool resetTransformOnActivate = false; //if true, reset parent's transform to mParentInit*

        public UnityEvent awakeEvent;
        public UnityEvent sleepEvent;

        private bool mIsActive = true;

        private Transform mDefaultParent; //the parent when we are instantiated        
        private Vector3 mLocalPos;

        private Coroutine mDeactivateRout;
        
        private Vector3 mParentInitPos;
        private Vector3 mParentInitScale;
        private Quaternion mParentInitRot;

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
                if(!mDefaultParent) { //parent was destroyed at some point
                    Destroy(gameObject);
                    return;
                }

                //put ourself back in parent
                if(resetTransformOnActivate) {
                    mDefaultParent.position = mParentInitPos;
                    mDefaultParent.rotation = mParentInitRot;
                    mDefaultParent.localScale = mParentInitScale;
                }

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

            mParentInitPos = mDefaultParent.position;
            mParentInitRot = mDefaultParent.rotation;
            mParentInitScale = mDefaultParent.localScale;

            mLocalPos = transform.localPosition;
        }
        
        protected virtual void SetParent(Transform toParent) {
            if(toParent != mDefaultParent) {
                Vector3 pos = transform.position;
                transform.SetParent(toParent);
                transform.position = pos;
            }
            else {
                transform.SetParent(toParent);
                transform.position = toParent.position;
                transform.localPosition = mLocalPos;
            }
        }

        IEnumerator DoInActiveDelay() {
            float curTime = 0f;
            while(curTime < deactivateDelay) {
                yield return null;
                curTime += Time.deltaTime;

                if(!mDefaultParent) { //parent lost?
                    Destroy(gameObject);
                    yield break;
                }
            }

            InActive(true);

            //check if parent is still alive
            while(mDefaultParent) {
                if(mDefaultParent.gameObject.activeSelf) { //parent got activated manually
                    Activate();
                    yield break;
                }

                yield return null;
            }

            Destroy(gameObject);

            mDeactivateRout = null;
        }

        protected void InActive(bool notifySleep) {
            if(mIsActive) {
                if(!mDefaultParent) { //parent was destroyed at some point
                    Destroy(gameObject);
                    return;
                }

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

        void IPoolDespawn.OnDespawned() {
            //put back to parent
            SetParent(mDefaultParent);

            StopDeactivateRout();
            mIsActive = true;
        }
    }
}
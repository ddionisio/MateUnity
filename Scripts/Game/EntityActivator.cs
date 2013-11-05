using UnityEngine;
using System.Collections;

/// <summary>
///use this to have entities in the map be deactivated at start,
///waking up when certain things trigger it
///make sure to have this as a child,
///destroy it if we are currently active
///</summary>
[AddComponentMenu("M8/Entity/EntityActivator")]
public class EntityActivator : MonoBehaviour {
    //public const string ActivatorHolderTag = "ActivatorHolder"; //make sure this is in the scene root
    
    public delegate void Callback();

    public bool deactivateOnStart = true;
    public float deactivateDelay = 2.0f;

    public event Callback awakeCallback;
    public event Callback sleepCallback;

    protected const string InActiveDelayInvoke = "InActiveDelay";

    private static GameObject mActivatorGO;

    private bool mIsActive = true;

    private Transform mDefaultParent; //the parent when we are instantiated
    private Transform mActivatorHolder = null;
    private Vector3 mLocalPos;
    
    /// <summary>
    /// If true, then we are currently waiting for trigger. If false, then parent should be inactive
    /// </summary>
    public bool isActive { get { return mIsActive; } }

    public Transform defaultParent { get { return mDefaultParent; } }

    public void ForceActivate() {
        DoActive();
    }

    /// <summary>
    /// Initialize, call this when you are about to be re-added to the scene
    /// </summary>
    public virtual void Start() {
        //if(mActivatorGO == null) {
            //mActivatorGO = GameObject.FindGameObjectWithTag(ActivatorHolderTag);
            if(mActivatorGO == null) {
                mActivatorGO = new GameObject("_activate");
                //mActivatorGO.tag = ActivatorHolderTag;
            }
        //}

        mActivatorHolder = mActivatorGO.transform;

        if(deactivateOnStart) {
            DoInActive(false);
        }
    }

    /// <summary>
    /// Call this when you are about to be released or destroyed. If destroy = true, then destroy this object
    /// </summary>
    public virtual void Release(bool destroy) {
        if(!mIsActive) {
            //put ourself back in parent
            if(destroy) {
                Object.Destroy(gameObject);
            }
            else {
                //put back to parent
                SetParent(mDefaultParent);
            }

            //StopCoroutine("DoActive");
            CancelInvoke(InActiveDelayInvoke);
            mIsActive = true;
        }
    }

    protected virtual void Awake() {
        mDefaultParent = transform.parent;
        mLocalPos = transform.localPosition;
    }

    protected virtual void SetParent(Transform toParent) {
        transform.parent = toParent;
        transform.localPosition = mLocalPos;
    }

    void OnDestroy() {
        awakeCallback = null;
        sleepCallback = null;
    }

    void OnTriggerEnter(Collider c) {
        //StartCoroutine("DoActive");
        DoActive();
    }

    void OnTriggerExit(Collider c) {
        if(mIsActive) {
            if(deactivateDelay > 0.0f) {
                Invoke(InActiveDelayInvoke, deactivateDelay);
            }
            else {
                DoInActive(true);
            }
        }
    }

    /*IEnumerator*/
    protected void DoActive() {
        if(!mIsActive) {
            //put ourself back in parent
            mDefaultParent.gameObject.SetActive(true);
            SetParent(mDefaultParent);

            mIsActive = true;

            //yield return new WaitForFixedUpdate();

            if(awakeCallback != null) {
                awakeCallback();
            }
        }
        else {
            CancelInvoke(InActiveDelayInvoke);
        }

        //yield break;
    }

    void InActiveDelay() {
        DoInActive(true);
    }

    protected void DoInActive(bool notifySleep) {
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

            if(notifySleep && sleepCallback != null) {
                sleepCallback();
            }
        }
    }
}

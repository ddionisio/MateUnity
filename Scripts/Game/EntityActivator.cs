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
    public const string ActivatorHolderTag = "ActivatorHolder"; //make sure this is in the scene root

    public delegate void Callback();

    public bool deactivateOnStart = true;
    public float deactivateDelay = 2.0f;

    public event Callback awakeCallback;
    public event Callback sleepCallback;

    private bool mIsActive = true;
    private GameObject mParentGo;

    private Transform mActivatorHolder = null;

    /// <summary>
    /// If true, then we are currently waiting for trigger. If false, then parent should be inactive
    /// </summary>
    public bool isActive { get { return mIsActive; } }

    /// <summary>
    /// Initialize, call this when you are about to be re-added to the scene
    /// </summary>
    public void Start() {
        GameObject go = GameObject.FindGameObjectWithTag(ActivatorHolderTag);
        mActivatorHolder = go != null ? go.transform : null;

        if(deactivateOnStart) {
            DoInActive(false);
        }
    }

    /// <summary>
    /// Call this when you are about to be released or destroyed
    /// </summary>
    public void Release(bool destroy) {
        if(!mIsActive) {
            //put ourself back in parent
            if(destroy) {
                Object.Destroy(gameObject);
            }
            else {
                //put back to parent
                transform.parent = mParentGo.transform;
            }

            //StopCoroutine("DoActive");
            CancelInvoke("InActiveDelay");
            mParentGo = null;
            mIsActive = true;
        }
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
                Invoke("InActiveDelay", deactivateDelay);
            }
            else {
                DoInActive(true);
            }
        }
    }

    /*IEnumerator*/
    void DoActive() {
        if(!mIsActive) {
            //put ourself back in parent
            mParentGo.SetActive(true);
            transform.parent = mParentGo.transform;

            mParentGo = null;

            mIsActive = true;

            //yield return new WaitForFixedUpdate();

            if(awakeCallback != null) {
                awakeCallback();
            }
        }
        else {
            CancelInvoke("InActiveDelay");
        }

        //yield break;
    }

    void InActiveDelay() {
        DoInActive(true);
    }

    void DoInActive(bool notifySleep) {
        if(mIsActive) {
            //StopCoroutine("DoActive");

            if(notifySleep && sleepCallback != null) {
                sleepCallback();
            }

            //remove from parent
            mParentGo = transform.parent.gameObject;

            if(mActivatorHolder != null) {
                transform.parent = mActivatorHolder;
            }
            else {
                transform.parent = null;
            }

            mParentGo.SetActive(false);

            mIsActive = false;
        }
    }
}

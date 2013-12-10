using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public abstract class GravityFieldBase : MonoBehaviour {
    public const int startCapacity = 5;

    public struct ItemData {
        public GravityController controller;

        public ItemData(GravityController ctrl) {
            controller = ctrl;
            if(ctrl != null) {
                ctrl.gravityFieldCounter++;
            }
        }

        public void Revert(bool restore) {
            if(controller.gameObject.activeSelf && controller.enabled) {
                controller.gravityFieldCounter--;
                if(controller.gravityFieldCounter <= 0 && restore) {
                    controller.up = controller.startUp;
                    controller.gravity = controller.startGravity;
                }
            }
        }
    }

    public bool gravityOverride = false; //if true, use the gravity value to controller
    public float gravity = -9.8f;
    public bool retainGravity = false; //if true, then gravity and up orientation of GravityController will persist when it exits the field
    public bool isGlobal = false; //check as the default gravity field, there should only be one of these.
    public float updateDelay = 0.0f;

    public bool fallLimit; //if enabled, reduce speed based on fallSpeedLimit
    public float fallSpeedLimit;

    private static GravityFieldBase mGlobal = null;

    private Dictionary<Collider, ItemData> mItems = new Dictionary<Collider, ItemData>(startCapacity);

    private bool mIsProcessing = false;

    public static GravityFieldBase global { get { return mGlobal; } }

    public void Add(GravityController ctrl) {
        if(!mItems.ContainsKey(ctrl.collider) && ctrl.rigidbody != null && !ctrl.rigidbody.isKinematic) {
            mItems.Add(ctrl.collider, new ItemData(ctrl));
            if(!mIsProcessing) StartCoroutine(DoEval());
        }
    }

    protected abstract Vector3 GetUpVector(GravityController entity);

    protected virtual void ItemRemoved(GravityController ctrl) {
    }

    protected virtual void OnDisable() {
        if(mGlobal == this)
            mGlobal = null;

        mIsProcessing = false;
        StopAllCoroutines();
    }

    protected virtual void OnEnable() {
        if(isGlobal)
            mGlobal = this;

        if(mItems.Count > 0 && !mIsProcessing)
            StartCoroutine(DoEval());
    }

    void OnTriggerEnter(Collider col) {
        if(!mItems.ContainsKey(col)) {
            GravityController ctrl = col.GetComponent<GravityController>();
            if(ctrl != null)
                Add(ctrl);
        }
    }

    void OnTriggerExit(Collider col) {
        ItemData itm;
        if(mItems.TryGetValue(col, out itm)) {
            mItems.Remove(col);
            ItemRemoved(itm.controller);
            itm.Revert(!retainGravity);
        }
    }

    IEnumerator DoEval() {
        mIsProcessing = true;

        YieldInstruction wait;
        if(updateDelay > 0.0f)
            wait = new WaitForSeconds(updateDelay);
        else
            wait = new WaitForFixedUpdate();

        while(mItems.Count > 0) {
            foreach(KeyValuePair<Collider, ItemData> pair in mItems) {
                if(!pair.Key)
                    continue;

                GravityController ctrl = pair.Value.controller;
                if(ctrl.enabled) {
                    ctrl.up = GetUpVector(ctrl);

                    if(gravityOverride)
                        ctrl.gravity = gravity;

                    //speed limit
                    if(fallLimit) {
                        //assume y-axis, positive up
                        Rigidbody body = ctrl.rigidbody;
                        if(body && !body.isKinematic) {
                            Vector3 localVel = ctrl.transform.worldToLocalMatrix.MultiplyVector(body.velocity);
                            if(localVel.y < -fallSpeedLimit) {
                                localVel.y = -fallSpeedLimit;
                                body.velocity = ctrl.transform.localToWorldMatrix.MultiplyVector(localVel);
                            }
                        }
                    }
                }
            }

            yield return wait;
        }

        mIsProcessing = false;
    }
}

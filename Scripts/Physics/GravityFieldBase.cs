using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public abstract class GravityFieldBase : MonoBehaviour {
    public const int startCapacity = 5;

    public struct ItemData {
        public GravityController controller;
        public Vector3 prevUp;
        public float prevGravity;

        public ItemData(GravityController ctrl) {
            controller = ctrl;
            if(ctrl != null) {
                prevUp = ctrl.up;
                prevGravity = ctrl.gravity;
            }
            else {
                prevUp = Vector3.zero;
                prevGravity = 0.0f;
            }
        }

        public void Restore() {
            controller.up = prevUp;
            controller.gravity = prevGravity;
        }
    }

    public bool gravityOverride = false; //if true, use the gravity value to controller
    public float gravity = -9.8f;
    public bool retainGravity = false; //if true, then gravity and up orientation of GravityController will persist when it exits the field
    public bool isGlobal = false; //check as the default gravity field, there should only be one of these.
    public float updateDelay = 0.0f;

    private static GravityFieldBase mGlobal = null;

    private Dictionary<Collider, ItemData> mItems = new Dictionary<Collider, ItemData>(startCapacity);

    private bool mIsProcessing = false;

    public static GravityFieldBase global { get { return mGlobal; } }

    public void Add(GravityController ctrl) {
        if(!mItems.ContainsKey(ctrl.collider) && ctrl.rigidbody != null && !ctrl.rigidbody.isKinematic) {
            if(ctrl.gravityField != null) { //another field holds the item, transfer ownership
                ItemData item = ctrl.gravityField.RemoveItem(ctrl.collider, false);
                if(item.controller != null) {//invalid for some reason?

                    mItems.Add(ctrl.collider, item);
                    if(!mIsProcessing) StartCoroutine(DoEval());

                    ctrl.gravityField = this;
                }
            }
            else {
                mItems.Add(ctrl.collider, new ItemData(ctrl));
                if(!mIsProcessing) StartCoroutine(DoEval());
                ctrl.gravityField = this;
            }
        }
    }

    /// <summary>
    /// Used by other field to transfer ownership of item
    /// </summary>
    public ItemData RemoveItem(Collider col, bool restore) {
        ItemData itm;
        if(mItems.TryGetValue(col, out itm)) {
            if(restore)
                itm.Restore();

            itm.controller.gravityField = null;

            mItems.Remove(col);
            ItemRemoved(itm.controller);
        }
        else
            itm = new ItemData(null);

        return itm;
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

            if(!retainGravity) {
                itm.Restore();
            }

            //add to global
            if(mGlobal != null) {
                mGlobal.mItems.Add(col, itm);
                if(!mIsProcessing) StartCoroutine(DoEval());

                itm.controller.gravityField = mGlobal;
            }
            else
                itm.controller.gravityField = null;
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
                }
            }

            yield return wait;
        }

        mIsProcessing = false;
    }
}

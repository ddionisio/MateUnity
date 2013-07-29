using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("")]
public abstract class GravityFieldBase : MonoBehaviour {
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

    public float gravity = -9.8f;
    public bool retainGravity = false; //if true, then gravity and up orientation of GravityController will persist when it exits the field
    public bool isGlobal = false; //check as the default gravity field, there should only be one of these.
    public float updateDelay = 0.0f;

    private static GravityFieldBase mGlobal = null;
    
    private Dictionary<Collider, ItemData> mItems = new Dictionary<Collider,ItemData>(16);

    private YieldInstruction mWaitDelay;

    public static GravityFieldBase global { get { return mGlobal; } }

    public void Add(GravityController ctrl) {
        mItems.Add(ctrl.collider, new ItemData(ctrl));
        ctrl.gravityField = this;
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
        }
        else
            itm = new ItemData(null);

        return itm;
    }

    protected abstract Vector3 GetUpVector(Transform entity);

    protected virtual void OnDisable() {
        if(mGlobal == this)
            mGlobal = null;

        StopAllCoroutines();
    }

    protected virtual void OnEnable() {
        if(isGlobal)
            mGlobal = this;

        if(mWaitDelay == null) {
            if(updateDelay > 0.0f)
                mWaitDelay = new WaitForSeconds(updateDelay);
            else
                mWaitDelay = new WaitForFixedUpdate();
        }

        StartCoroutine(DoEval());
    }

    void OnTriggerEnter(Collider col) {
        if(!mItems.ContainsKey(col)) {
            GravityController ctrl = col.GetComponent<GravityController>();
            if(ctrl != null && col.rigidbody != null && !col.rigidbody.isKinematic) {
                if(ctrl.gravityField != null) { //another field holds the item, transfer ownership
                    ItemData item = ctrl.gravityField.RemoveItem(col, false);
                    if(item.controller != null) {//invalid for some reason?
                        mItems.Add(col, item);
                        ctrl.gravityField = this;
                    }
                }
                else {
                    Add(ctrl);
                }
            }
        }
    }

    void OnTriggerExit(Collider col) {
        ItemData itm;
        if(mItems.TryGetValue(col, out itm)) {
            mItems.Remove(col);
                        
            if(!retainGravity) {
                itm.Restore();
            }

            //add to global
            if(mGlobal != null) {
                mGlobal.mItems.Add(col, itm);
                itm.controller.gravityField = mGlobal;
            }
            else
                itm.controller.gravityField = null;
        }
    }

    IEnumerator DoEval() {
        while(true) {
            foreach(KeyValuePair<Collider, ItemData> pair in mItems) {
                GravityController ctrl = pair.Value.controller;
                if(ctrl.enabled) {
                    Transform t = ctrl.transform;

                    ctrl.up = GetUpVector(t);

                    ctrl.gravity = gravity;
                }
            }

            yield return mWaitDelay;
        }
    }
}

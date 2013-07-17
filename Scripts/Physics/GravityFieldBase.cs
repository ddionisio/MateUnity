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

    private Dictionary<Collider, ItemData> mItems = new Dictionary<Collider,ItemData>(16);

    /// <summary>
    /// Used by other field to transfer ownership of item
    /// </summary>
    public ItemData RemoveItem(Collider col, bool restore) {
        ItemData itm;
        if(mItems.TryGetValue(col, out itm)) {
            if(restore)
                itm.Restore();

            itm.controller.gravityField = null;
        }
        else
            itm = new ItemData(null);

        return itm;
    }

    protected abstract Vector3 GetUpVector(Vector3 position);

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
                    mItems.Add(col, new ItemData(ctrl));
                    ctrl.gravityField = this;
                }
            }
        }
    }

    void OnTriggerExit(Collider col) {
        ItemData itm;
        if(mItems.TryGetValue(col, out itm)) {
            if(!retainGravity)
                itm.Restore();

            itm.controller.gravityField = null;
            mItems.Remove(col);
        }
    }

    void LateUpdate() {
        foreach(KeyValuePair<Collider, ItemData> pair in mItems) {
            GravityController ctrl = pair.Value.controller;
            ctrl.up = GetUpVector(ctrl.transform.position);
            ctrl.gravity = gravity;
        }
    }
}

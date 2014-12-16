using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("")]
    public abstract class GravityFieldBase : MonoBehaviour {
        public bool gravityOverride = false; //if true, use the gravity value to controller
        public float gravity = -9.8f;
        public bool retainGravity = false; //if true, then gravity and up orientation of GravityController will persist when it exits the field
        public bool isGlobal = false; //check as the default gravity field, there should only be one of these.
        public float updateDelay = 0.0f;

        public bool fallLimit; //if enabled, reduce speed based on fallSpeedLimit
        public float fallSpeedLimit;

        private static GravityFieldBase mGlobal = null;

        public static GravityFieldBase global { get { return mGlobal; } }

        public abstract Vector3 GetUpVector(GravityController entity);

        public virtual void ItemRemoved(GravityController ctrl) {
        }

        protected virtual void OnDisable() {
            if(mGlobal == this)
                mGlobal = null;
        }

        protected virtual void OnEnable() {
            if(isGlobal)
                mGlobal = this;
        }
    }
}
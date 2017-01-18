using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    public abstract class Sensor2D<T> : MonoBehaviour where T : Component {
        public int cacheCapacity = 16;

        protected abstract bool UnitVerify(T unit);
        protected abstract void UnitAdded(T unit);
        protected abstract void UnitRemoved(T unit);

        protected Collider2D mCollider;

        private CacheList<T> mUnits;

        public CacheList<T> items {
            get {
                return mUnits;
            }
        }

        public void CleanUp() {
            mUnits.RemoveAll(IsUnitInvalid);
        }

        protected virtual bool ColliderVerify(Collider2D other) {
            return true;
        }

        protected virtual void OnEnable() {
            mCollider.enabled = true;
        }

        protected virtual void OnDisable() {
            mCollider.enabled = false;
        }

        protected virtual void Awake() {
            mCollider = GetComponent<Collider2D>();

            mUnits = new CacheList<T>(cacheCapacity);
        }

        void OnTriggerEnter2D(Collider2D other) {
            CleanUp();

            if(mUnits.IsFull) {
                Debug.LogWarning(name+": Unit Capacity is full.");
                return;
            }

            if(ColliderVerify(other)) {
                T unit = other.GetComponent<T>();
                if(unit != null && UnitVerify(unit) && !mUnits.Exists(unit)) {
                    mUnits.Add(unit);
                    UnitAdded(unit);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other) {            
            if(ColliderVerify(other)) {
                CleanUp();

                T unit = other.GetComponent<T>();
                if(unit != null && mUnits.Remove(unit)) {
                    UnitRemoved(unit);
                }
            }
        }

        bool IsUnitInvalid(T unit) {
            if(unit != null) {
                if(!unit.gameObject.activeInHierarchy || !UnitVerify(unit)) {
                    UnitRemoved(unit);
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
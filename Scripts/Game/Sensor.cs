using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    public abstract class Sensor<T> : MonoBehaviour where T : Component {
        protected abstract bool UnitVerify(T unit);
        protected abstract void UnitAdded(T unit);
        protected abstract void UnitRemoved(T unit);

        protected Collider mCollider;

        private HashSet<T> mUnits = new HashSet<T>();

        public HashSet<T> items {
            get {
                return mUnits;
            }
        }

        public void CleanUp() {
            mUnits.RemoveWhere(IsUnitInvalid);
        }

        protected virtual void OnEnable() {
            mCollider.enabled = true;
        }

        protected virtual void OnDisable() {
            mCollider.enabled = false;
        }

        protected virtual void Awake() {
            mCollider = GetComponent<Collider>();
        }

        void OnTriggerEnter(Collider other) {
            CleanUp();
            T unit = other.GetComponent<T>();
            if(unit != null && UnitVerify(unit)) {
                if(mUnits.Add(unit)) {
                    UnitAdded(unit);
                }
            }
        }

        void OnTriggerExit(Collider other) {
            CleanUp();
            T unit = other.GetComponent<T>();
            if(unit != null && mUnits.Remove(unit)) {
                UnitRemoved(unit);
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
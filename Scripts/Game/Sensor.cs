using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    public abstract class Sensor<T> : MonoBehaviour where T : Component {
        protected abstract bool UnitVerify(T unit);
        protected abstract void UnitAdded(T unit);
        protected abstract void UnitRemoved(T unit);

#if !M8_PHYSICS_DISABLED
        protected Collider mCollider;
#endif

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
#if !M8_PHYSICS_DISABLED
            mCollider.enabled = true;
#endif
        }

        protected virtual void OnDisable() {
#if !M8_PHYSICS_DISABLED
            mCollider.enabled = false;
#endif
        }

        protected virtual void Awake() {
#if !M8_PHYSICS_DISABLED
            mCollider = GetComponent<Collider>();
#endif
        }

#if !M8_PHYSICS_DISABLED
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
#endif

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
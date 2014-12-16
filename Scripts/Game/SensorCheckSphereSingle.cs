using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    public abstract class SensorCheckSphereSingle<T> : MonoBehaviour where T : Component {
        public delegate void OnUnitChange();

        public event OnUnitChange unitAddRemoveCallback;

        public LayerMask mask;

        public float radius;

        public float delay = 0.1f;

        public Color gizmoColor = Color.green;

        private T mUnit;

        public T unit {
            get { return mUnit; }
        }

        protected abstract bool UnitVerify(T unit);
        protected abstract void UnitAdded(T unit);
        protected abstract void UnitRemoved(T unit);
        protected virtual void UnitUpdate() { }

        public void CleanUp() {
            if(IsUnitInvalid(mUnit)) {
                mUnit = null;
            }
        }

        protected virtual void OnEnable() {
            InvokeRepeating("Check", delay, delay);
        }

        protected virtual void OnDisable() {
            CancelInvoke();
        }

        protected virtual void OnDestroy() {
            unitAddRemoveCallback = null;
        }

        void Check() {
            //TODO: mode -> nearest, etc.
            T lastUnit = mUnit;
            mUnit = null;

            Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask.value);
            foreach(Collider col in cols) {
                T u = col.GetComponent<T>();
                if(u != null && UnitVerify(u)) {
                    mUnit = u;
                    break;
                }
            }

            if(mUnit != lastUnit) {
                if(lastUnit != null)
                    UnitRemoved(lastUnit);

                if(mUnit != null)
                    UnitAdded(mUnit);

                if(unitAddRemoveCallback != null)
                    unitAddRemoveCallback();
            }

            UnitUpdate();
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        bool IsUnitInvalid(T u) {
            if(u != null) {
                if(!u.gameObject.activeInHierarchy || !UnitVerify(u)) {
                    UnitRemoved(u);

                    if(unitAddRemoveCallback != null)
                        unitAddRemoveCallback();

                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
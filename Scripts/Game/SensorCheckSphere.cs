using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    public abstract class SensorCheckSphere<T> : MonoBehaviour where T : Component {
        public delegate void OnUnitChange();

        public event OnUnitChange unitAddRemoveCallback;

        public LayerMask mask;

        public float radius;

        public float delay = 0.1f;

        public Color gizmoColor = Color.green;

        private HashSet<T> mUnits = new HashSet<T>();
        private HashSet<T> mGatherUnits = new HashSet<T>();

        public HashSet<T> items {
            get { return mUnits; }
        }

        protected abstract bool UnitVerify(T unit);
        protected abstract void UnitAdded(T unit);
        protected abstract void UnitRemoved(T unit);
        protected virtual void UnitUpdate() { }

        /// <summary>
        /// Grabs one unit in the set
        /// </summary>
        public T GetSingleUnit() {
            T ret = null;

            if(mUnits.Count > 0) {
                foreach(T unit in mUnits) {
                    if(unit != null) {
                        ret = unit;
                        break;
                    }
                }
            }

            return ret;
        }

        public void CleanUp() {
            mUnits.RemoveWhere(IsUnitInvalid);
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
            int prevCount = mUnits.Count;

            CleanUp();

            //get units in area
            mGatherUnits.Clear();

            Collider[] cols = Physics.OverlapSphere(transform.position, radius, mask.value);
            foreach(Collider col in cols) {
                T unit = col.GetComponent<T>();
                if(unit != null && UnitVerify(unit)) {
                    mGatherUnits.Add(unit);

                    //check if not in current units
                    if(!mUnits.Contains(unit)) {
                        //enter
                        UnitAdded(unit);
                    }
                    else {
                        mUnits.Remove(unit);
                    }
                }
            }

            //swap
            HashSet<T> prevSet = mUnits;
            mUnits = mGatherUnits;
            mGatherUnits = prevSet;

            //what remains should be removed
            foreach(T other in prevSet) {
                UnitRemoved(other);
            }

            if(unitAddRemoveCallback != null && prevCount != mUnits.Count)
                unitAddRemoveCallback();

            UnitUpdate();
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, radius);
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
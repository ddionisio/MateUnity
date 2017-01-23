using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Game/SensorCollider2D")]
    public class SensorCollider2D : MonoBehaviour {
        public int cacheCapacity = 16;

        public string[] tagFilter; //set to empty to not use filter

        protected Collider2D mCollider;

        private CacheList<Collider2D> mUnits;

        public CacheList<Collider2D> items {
            get {
                return mUnits;
            }
        }

        public void CleanUp() {
            mUnits.RemoveAll(IsUnitInvalid);
        }

        protected virtual bool UnitVerify(Collider2D unit) {
            if(tagFilter.Length <= 0)
                return true;

            for(int i = 0; i < tagFilter.Length; i++) {
                if(unit.CompareTag(tagFilter[i]))
                    return true;
            }

            return false;
        }

        protected virtual void UnitAdded(Collider2D unit) {

        }

        protected virtual void UnitRemoved(Collider2D unit) {

        }

        protected virtual void OnEnable() {
            mCollider.enabled = true;
        }

        protected virtual void OnDisable() {
            mCollider.enabled = false;
        }

        protected virtual void Awake() {
            mCollider = GetComponent<Collider2D>();

            mUnits = new CacheList<Collider2D>(cacheCapacity);
        }

        void OnTriggerEnter2D(Collider2D other) {
            CleanUp();

            if(mUnits.IsFull) {
                Debug.LogWarning(name+": Unit Capacity is full.");
                return;
            }

            if(UnitVerify(other) && !mUnits.Exists(other)) {
                mUnits.Add(other);
                UnitAdded(other);
            }
        }

        void OnTriggerExit2D(Collider2D other) {
            if(UnitVerify(other)) {
                CleanUp();
                
                if(mUnits.Remove(other)) {
                    UnitRemoved(other);
                }
            }
        }

        bool IsUnitInvalid(Collider2D unit) {
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
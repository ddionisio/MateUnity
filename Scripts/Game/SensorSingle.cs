using UnityEngine;
using System.Collections.Generic;

namespace M8 {
    public abstract class SensorSingle<T> : MonoBehaviour where T : Component {
        public delegate void Callback(T unit);

        public event Callback enterCallback;
        public event Callback stayCallback;
        public event Callback exitCallback;

        protected Collider mCollider;

        protected abstract bool UnitVerify(T unit);

        protected virtual void UnitEnter(T unit) { }
        protected virtual void UnitStay(T unit) { }
        protected virtual void UnitExit(T unit) { }

        void OnEnable() {
            mCollider.enabled = true;
        }

        void OnDisable() {
            mCollider.enabled = false;
        }

        void OnDestroy() {
            enterCallback = null;
            stayCallback = null;
            exitCallback = null;
        }

        void Awake() {
            mCollider = GetComponent<Collider>();
        }

        void OnTriggerEnter(Collider other) {
            T unit = other.GetComponent<T>();
            if(unit != null && UnitVerify(unit)) {
                UnitEnter(unit);

                if(enterCallback != null) {
                    enterCallback(unit);
                }
            }
        }

        void OnTriggerStay(Collider other) {
            T unit = other.GetComponent<T>();
            if(unit != null && UnitVerify(unit)) {
                UnitStay(unit);

                if(stayCallback != null) {
                    stayCallback(unit);
                }
            }
        }

        void OnTriggerExit(Collider other) {
            T unit = other.GetComponent<T>();
            if(unit != null && UnitVerify(unit)) {
                UnitExit(unit);

                if(exitCallback != null) {
                    exitCallback(unit);
                }
            }
        }
    }
}
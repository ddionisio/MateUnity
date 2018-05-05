using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// This signal keeps a record of current value. Signal is invoked when value is changed (or set if ignoreChanged is true).
    /// </summary>
    public abstract class SignalValue<T> : ScriptableObject {
        [SerializeField]
        T _initialValue;
        [SerializeField]
        bool _ignoreChanged;

        /// <summary>
        /// This is called with current and previous value, respectively
        /// </summary>
        public event System.Action<T, T> callback;

        public T curValue {
            get { return mCurValue; }

            set {
                if(_ignoreChanged || IsChanged(value)) {
                    var prevVal = mCurValue;

                    ProcessValue(value);

                    mCurValue = value;

                    if(callback != null)
                        callback(value, prevVal);
                }
            }
        }

        private T mCurValue;

        /// <summary>
        /// Call this to force invoke current value
        /// </summary>
        public void Invoke() {
            if(callback != null)
                callback(mCurValue, mCurValue);
        }

        /// <summary>
        /// Called before applying value
        /// </summary>
        protected virtual void ProcessValue(T newValue) {

        }

        /// <summary>
        /// Called to check if new value is a change to current value.
        /// </summary>
        protected virtual bool IsChanged(T newValue) {
            return !mCurValue.Equals(newValue);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Float Proxy")]
    public class SignalFloatProxy : MonoBehaviour {
        public SignalFloat signal;

        [Tooltip("Value to use when calling Invoke()")]
        public float invokeValue;

        public void Invoke() {
            signal.Invoke(invokeValue);
        }

        public void Invoke(float v) {
            signal.Invoke(v);
        }
    }
}
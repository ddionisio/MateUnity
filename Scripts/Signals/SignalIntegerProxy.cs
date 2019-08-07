using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Integer Proxy")]
    public class SignalIntegerProxy : MonoBehaviour {
        public SignalInteger signal;

        [Tooltip("Value to use when calling Invoke()")]
        public int invokeValue;

        public void Invoke() {
            signal.Invoke(invokeValue);
        }

        public void Invoke(int v) {
            signal.Invoke(v);
        }
    }
}
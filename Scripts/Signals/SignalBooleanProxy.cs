using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Boolean Proxy")]
    public class SignalBooleanProxy : MonoBehaviour {
        public SignalBoolean signal;

        [Tooltip("Value to use when calling Invoke()")]
        public bool invokeValue;

        public void Invoke() {
            signal.Invoke(invokeValue);
        }

        public void Invoke(bool v) {
            signal.Invoke(v);
        }
    }
}
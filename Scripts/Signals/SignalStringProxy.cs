using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/String Proxy")]
    public class SignalStringProxy : MonoBehaviour {
        public SignalString signal;

        [Tooltip("Value to use when calling Invoke()")]
        public string invokeValue;

        public void Invoke() {
            signal.Invoke(invokeValue);
        }

        public void Invoke(string v) {
            signal.Invoke(v);
        }
    }
}
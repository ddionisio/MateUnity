using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Vector3 Proxy")]
    public class SignalVector3Proxy : MonoBehaviour {
        public SignalVector3 signal;

        [Tooltip("Value to use when calling Invoke()")]
        public Vector3 invokeValue;

        public void Invoke() {
            signal.Invoke(invokeValue);
        }

        public void Invoke(Vector3 v) {
            signal.Invoke(v);
        }
    }
}
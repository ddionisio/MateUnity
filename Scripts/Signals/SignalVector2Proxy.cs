using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Vector2 Proxy")]
    public class SignalVector2Proxy : MonoBehaviour {
        public SignalVector2 signal;

        [Tooltip("Value to use when calling Invoke()")]
        public Vector2 invokeValue;

        public void Invoke() {
            signal.Invoke(invokeValue);
        }

        public void Invoke(Vector2 v) {
            signal.Invoke(v);
        }
    }
}
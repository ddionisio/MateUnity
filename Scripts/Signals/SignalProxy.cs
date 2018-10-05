using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Proxy")]
    public class SignalProxy : MonoBehaviour {
        public Signal signal;

        public void Invoke() {
            signal.Invoke();
        }
    }
}
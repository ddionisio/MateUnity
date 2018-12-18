using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/State Proxy")]
    public class SignalStateProxy : MonoBehaviour {
        public SignalState signal;

        public void Invoke(State state) {
            signal.Invoke(state);
        }
    }
}
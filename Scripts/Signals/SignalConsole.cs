using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use for signals invoked via Console
    /// </summary>
    [CreateAssetMenu(fileName = "signalConsole", menuName = "Signals/Console")]
    public class SignalConsole : SignalParam<Console> {
    }
}
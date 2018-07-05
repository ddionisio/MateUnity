using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use for signals invoked via EntityBase
    /// </summary>
    [CreateAssetMenu(fileName = "signalEntity", menuName = "Signals/Entity")]
    public class SignalEntity : SignalParam<EntityBase> {
    }
}
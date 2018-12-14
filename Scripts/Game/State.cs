using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use for EntityBase.state, you can derive from this to associate it with more data based on the state
    /// </summary>
    [CreateAssetMenu(fileName = "state", menuName = "M8/State")]
    public class State : ScriptableObject {
    }
}
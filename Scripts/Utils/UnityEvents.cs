using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    [System.Serializable]
    public class UnityEventInputAction : UnityEvent<InputAction> { }

    [System.Serializable]
    public class UnityEventState : UnityEvent<State> { }

    [System.Serializable]
    public class UnityEventBoolean : UnityEvent<bool> { }

    [System.Serializable]
    public class UnityEventFloat : UnityEvent<float> { }

    [System.Serializable]
    public class UnityEventInteger : UnityEvent<int> { }

    [System.Serializable]
    public class UnityEventString : UnityEvent<string> { }

    [System.Serializable]
    public class UnityEventVector3 : UnityEvent<Vector3> { }

    [System.Serializable]
    public class UnityEventVector2 : UnityEvent<Vector2> { }
}
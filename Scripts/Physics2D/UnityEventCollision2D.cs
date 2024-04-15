using UnityEngine;
using UnityEngine.Events;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [System.Serializable]
    public class UnityEventCollision2D : UnityEvent<Collision2D> {
    }
#endif
}
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [System.Serializable]
    public class UnityEventCollider2D : UnityEvent<Collider2D> {
    }
#endif
}
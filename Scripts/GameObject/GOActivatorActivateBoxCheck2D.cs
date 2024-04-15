using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    /// <summary>
    /// Do a periodic check for colliders based on layer mask, ensure Activators have a collider with matching layer.
    /// </summary>
    [AddComponentMenu("M8/Game Object/Activator Activate Box Check 2D")]
    public class GOActivatorActivateBoxCheck2D : GOActivatorActivateCheckBase<Collider2D> {
        public Vector2 extent;

        protected override int PopulateCollisions(Collider2D[] cache) {
            var t = transform;
            return Physics2D.OverlapBoxNonAlloc(t.position, extent * 2.0f, t.eulerAngles.z, cache, layerMask);
        }

        void OnDrawGizmos() {
            if(extent.x > 0f && extent.y > 0f) {
                Gizmos.color = Color.green;
                Gizmo.DrawWireRect(transform.position, transform.eulerAngles.z, extent);
            }
        }
    }
#endif
}
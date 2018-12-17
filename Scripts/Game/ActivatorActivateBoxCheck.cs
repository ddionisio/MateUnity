using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Do a periodic check for colliders based on layer mask, ensure Activators have a collider with matching layer.
    /// </summary>
    [AddComponentMenu("M8/Game/Activator Activate Box Check")]
    public class ActivatorActivateBoxCheck : ActivatorActivateCheckBase<Collider> {
        public Vector3 extent;

        protected override int PopulateCollisions(Collider[] cache) {
            var t = transform;
            return Physics.OverlapBoxNonAlloc(t.position, extent, cache, t.rotation, layerMask);
        }

        void OnDrawGizmos() {
            if(extent.x > 0f && extent.y > 0f && extent.z > 0f) {
                Gizmos.color = Color.green;
                Gizmo.DrawWireCube(transform.position, transform.rotation, extent);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Do a periodic check for colliders based on layer mask, ensure Activators have a collider with matching layer.
    /// </summary>
    [AddComponentMenu("M8/Game/Activator Activate Radius Check 2D")]
    public class ActivatorActivateRadiusCheck2D : ActivatorActivateCheckBase<Collider2D> {
        public float radius;

        protected override int PopulateCollisions(Collider2D[] cache) {
            return Physics2D.OverlapCircleNonAlloc(transform.position, radius, cache, layerMask);
        }

        void OnDrawGizmos() {
            if(radius > 0f) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
}
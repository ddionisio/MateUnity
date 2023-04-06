using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Do a periodic check for colliders based on layer mask, ensure Activators have a collider with matching layer.
    /// </summary>
    [AddComponentMenu("M8/Game Object/Activator Activate Radius Check")]
#if !M8_PHYSICS_DISABLED
    public class GOActivatorActivateRadiusCheck : GOActivatorActivateCheckBase<Collider> {
        public float radius;

        protected override int PopulateCollisions(Collider[] cache) {
            return Physics.OverlapSphereNonAlloc(transform.position, radius, cache, layerMask);
        }

        void OnDrawGizmos() {
            if(radius > 0f) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
#else
    public class GOActivatorActivateRadiusCheck : GOActivatorActivateCheckBase<Component> {
        public float radius;

        protected override int PopulateCollisions(Component[] cache) {
            return 0;
        }

        void OnDrawGizmos() {
            if(radius > 0f) {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, radius);
            }
        }
    }
#endif
}
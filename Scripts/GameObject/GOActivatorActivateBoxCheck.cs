using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Do a periodic check for colliders based on layer mask, ensure Activators have a collider with matching layer.
    /// </summary>
    [AddComponentMenu("M8/Game Object/Activator Activate Box Check")]
#if !M8_PHYSICS_DISABLED
    public class GOActivatorActivateBoxCheck : GOActivatorActivateCheckBase<Collider> {
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
#else
    public class GOActivatorActivateBoxCheck : GOActivatorActivateCheckBase<Component> {
        public Vector3 extent;

        protected override int PopulateCollisions(Component[] cache) {
            return 0;
        }

        void OnDrawGizmos() {
            if(extent.x > 0f && extent.y > 0f && extent.z > 0f) {
                Gizmos.color = Color.green;
                Gizmo.DrawWireCube(transform.position, transform.rotation, extent);
            }
        }
    }
#endif
}
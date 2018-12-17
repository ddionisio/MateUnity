using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Do a periodic check for colliders based on layer mask, ensure Activators have a collider with matching layer.
    /// </summary>
    [AddComponentMenu("M8/Game/Activator Activate Radius Check")]
    public class ActivatorActivateRadiusCheck : ActivatorActivateRadiusCheckBase<Collider> {
        protected override int PopulateCollisions(Collider[] cache) {
            return Physics.OverlapSphereNonAlloc(transform.position, radius, cache, layerMask);
        }
    }
}
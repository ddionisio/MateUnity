using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Physics/GravityFieldUpDir")]
    public class GravityFieldUpDir : GravityFieldBase {
#if !M8_PHYSICS_DISABLED
        public override Vector3 GetUpVector(GravityController entity) {
            return transform.up;
        }
#endif
    }
}
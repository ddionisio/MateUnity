using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Physics2D/ForceFieldDir")]
    public class ForceFieldDir2D : ForceFieldBase2D {
        //angle relative to up
        public float angle;

        public Vector2 dir {
            get {
                return MathUtil.RotateAngle(transform.up, angle);
            }
        }

        public override Vector2 GetDir(ForceController2D entity) {
            return dir;
        }

        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmo.Arrow(transform.position, dir);
        }
    }
#endif
}
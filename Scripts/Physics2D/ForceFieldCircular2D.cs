using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Physics2D/ForceFieldCircular")]
    public class ForceFieldCircular2D : ForceFieldBase2D {
        public bool inward = false;

        public override Vector2 GetDir(ForceController2D entity) {
            Vector2 position = entity.transform.position;
            Vector2 center = transform.position;

            Vector2 dir = inward ? center - position : position - center;
            return dir.normalized;
        }
    }
}
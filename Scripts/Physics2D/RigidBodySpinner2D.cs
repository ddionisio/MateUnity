using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Physics2D/Spinner")]
    public class RigidBodySpinner2D : MonoBehaviour {

        public Rigidbody2D target;

        public float rotateSpeed;

        public float scale { get; private set; } = 1.0f;

        void Awake() {
            if(!target)
                target = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate() {
            float rot = target.rotation;
            float rotDelta = rotateSpeed * Time.fixedDeltaTime * scale;
            target.MoveRotation(rot + rotDelta);
        }
    }
#endif
}
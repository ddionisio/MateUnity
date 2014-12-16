using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/UpLock")]
    [ExecuteInEditMode]
    public class TransUpLock : MonoBehaviour {
        public Vector3 up = Vector3.up;

        public Transform target;

        void Awake() {
            if(target == null)
                target = transform;
        }

        void Update() {
#if UNITY_EDITOR
            if(target == null)
                target = transform;
#endif
            target.up = up;
        }
    }
}
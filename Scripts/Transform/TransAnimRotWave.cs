using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Rotate Z axis back and forth
    /// </summary>
    [AddComponentMenu("M8/Transform/AnimRotWave")]
    public class TransAnimRotWave : MonoBehaviour {

        //TODO: types, lerp modes

        public Transform target;

        public Vector3 rotate;

        public float speed;

        public bool local;

        public bool isRealtime;

        public bool resetOnDisable;

        private Vector3 mOrigin;

        void OnDisable() {
            if(resetOnDisable) {
                if(local)
                    target.localEulerAngles = mOrigin;
                else
                    target.eulerAngles = mOrigin;
            }
        }

        void Awake() {
            if(target == null)
                target = transform;

            mOrigin = local ? target.localEulerAngles : target.eulerAngles;
        }

        // Update is called once per frame
        void Update() {
            Vector3 angles = mOrigin + Mathf.Sin((isRealtime ? Time.realtimeSinceStartup : Time.time) * speed * Mathf.Deg2Rad) * rotate;

            if(local)
                target.localEulerAngles = angles;
            else
                target.eulerAngles = angles;
        }
    }
}
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

        private Vector3 mOrigin;

        void Awake() {
            if(target == null)
                target = transform;

            mOrigin = local ? transform.localEulerAngles : transform.eulerAngles;
        }

        // Update is called once per frame
        void Update() {
            Vector3 angles = mOrigin + Mathf.Sin(Time.time * speed * Mathf.Deg2Rad) * rotate;

            if(local)
                transform.localEulerAngles = angles;
            else
                transform.eulerAngles = angles;
        }
    }
}
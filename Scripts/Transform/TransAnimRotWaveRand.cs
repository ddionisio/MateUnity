using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Rotate Z axis back and forth
    /// </summary>
    [AddComponentMenu("M8/Transform/AnimRotWaveRand")]
    public class TransAnimRotWaveRand : MonoBehaviour {

        //TODO: types, lerp modes

        public Transform target;

        public Vector3 rotate;

        public float speed;
        public float speedRandOfs;

        public bool local;

        private Vector3 mOrigin;

        private float mSpeed;

        void Awake() {
            if(target == null)
                target = transform;

            mOrigin = local ? transform.localEulerAngles : transform.eulerAngles;

            mSpeed = speed + Random.value*speedRandOfs;

            Vector3 a = mOrigin;
            a.x += Random.Range(-rotate.x, rotate.x);
            a.y += Random.Range(-rotate.y, rotate.y);
            a.z += Random.Range(-rotate.z, rotate.z);

            if(local)
                transform.localEulerAngles = a;
            else
                transform.eulerAngles = a;
        }

        // Update is called once per frame
        void Update() {
            Vector3 angles = mOrigin + Mathf.Sin(Time.time * mSpeed * Mathf.Deg2Rad) * rotate;

            if(local)
                transform.localEulerAngles = angles;
            else
                transform.eulerAngles = angles;
        }
    }
}
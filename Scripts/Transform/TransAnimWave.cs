using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimWave")]
    public class TransAnimWave : MonoBehaviour {
        public float pulsePerSecond;

        public Vector3 start;
        public Vector3 end;

        private float mCurPulseTime = 0;

        void OnEnable() {
            mCurPulseTime = 0;
        }

        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            mCurPulseTime += Time.deltaTime;

            float t = Mathf.Sin(Mathf.PI * mCurPulseTime * pulsePerSecond);

            Vector3 newPos = Vector3.Lerp(start, end, t * t);

            transform.localPosition = new Vector3(newPos.x, newPos.y, newPos.z);
        }
    }
}
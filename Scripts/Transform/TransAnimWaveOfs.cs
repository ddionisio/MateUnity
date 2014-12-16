using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Transform/AnimWaveOfs")]
    public class TransAnimWaveOfs : MonoBehaviour {
        public Transform target;

        public float pulsePerSecond;

        public Vector3 ofs;

        public bool local = true;

        private float mCurPulseTime = 0;

        private Vector3 mStartPos;
        private Vector3 mEndPos;

        void OnEnable() {
            mCurPulseTime = 0;
        }

        void Awake() {
            if(target == null)
                target = transform;

            mStartPos = local ? target.localPosition : target.position;
            mEndPos = mStartPos + ofs;
        }

        // Use this for initialization
        void Start() {
        }

        // Update is called once per frame
        void Update() {
            mCurPulseTime += Time.deltaTime;

            float t = Mathf.Sin(Mathf.PI * mCurPulseTime * pulsePerSecond);

            Vector3 newPos = Vector3.Lerp(mStartPos, mEndPos, t * t);

            if(local)
                target.localPosition = newPos;
            else
                target.position = newPos;
        }
    }
}
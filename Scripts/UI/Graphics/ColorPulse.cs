using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Graphics {
    [AddComponentMenu("M8/UI/Graphics/ColorPulse")]
    public class ColorPulse : MonoBehaviour {
        public Graphic target;

        public float pulsePerSecond;
        public Color startColor;
        public Color endColor = Color.white;
        public bool realtime = false;

        private float mCurPulseTime = 0;
        private bool mStarted = false;

        private Color mDefaultColor;
        private float mLastRealtime;

        void OnEnable() {
            if(mStarted) {
                mCurPulseTime = 0;
                target.color = startColor;
                mLastRealtime = Time.realtimeSinceStartup;
            }
        }

        void OnDisable() {
            if(mStarted) {
                target.color = mDefaultColor;
            }
        }

        void Awake() {
            if(target == null)
                target = GetComponent<Graphic>();

            mDefaultColor = target.color;
        }

        // Use this for initialization
        void Start() {
            mStarted = true;
            target.color = startColor;
        }

        // Update is called once per frame
        void Update() {
            if(realtime) {
                mCurPulseTime += Time.realtimeSinceStartup - mLastRealtime;
                mLastRealtime = Time.realtimeSinceStartup;
            }
            else {
                mCurPulseTime += Time.deltaTime;
            }

            float t = Mathf.Sin(Mathf.PI * mCurPulseTime * pulsePerSecond);
            t *= t;

            target.color = Color.Lerp(startColor, endColor, t);
        }
    }
}
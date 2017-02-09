using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Graphics {
    [AddComponentMenu("M8/UI/Graphics/CanvasRendererBlink")]
    public class CanvasRendererBlink : MonoBehaviour {
        public CanvasRenderer target;
        public bool defaultEnabled = true;
        public float delay;
        public bool useRealtime = true;

        private const string blinkFunc = "DoBlink";
        private float mLastTime;
        private bool mIsMod = false;

        void OnEnable() {
            mLastTime = useRealtime ? Time.realtimeSinceStartup : Time.time;
        }

        void OnDisable() {
            target.cull = defaultEnabled;
        }

        void Awake() {
            if(target == null)
                target = GetComponent<CanvasRenderer>();

            target.cull = defaultEnabled;
        }

        void Update() {
            float t = useRealtime ? Time.realtimeSinceStartup : Time.time;
            if(t - mLastTime >= delay) {
                DoBlink();
                mLastTime = t;
            }
        }

        void DoBlink() {
            if(mIsMod)
                target.cull = !defaultEnabled;
            else
                target.cull = defaultEnabled;

            mIsMod = !mIsMod;
        }
    }
}
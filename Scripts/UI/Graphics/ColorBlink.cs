using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace M8.UI.Graphics {
    [AddComponentMenu("M8/UI/Graphics/ColorBlink")]
    public class ColorBlink : MonoBehaviour {
        public Graphic target;
        public Color color;
        public float delay;
        public bool useRealtime = true;

        private const string blinkFunc = "DoBlink";
        private Color mOrigColor;
        private float mLastTime;
        private bool mIsMod = false;

        void OnEnable() {
            mLastTime = useRealtime ? Time.realtimeSinceStartup : Time.time;
        }

        void OnDisable() {
            if(mIsMod) {
                if(target != null)
                    target.color = mOrigColor;
                mIsMod = false;
            }
        }

        void Awake() {
            if(target == null)
                target = GetComponent<Graphic>();

            mOrigColor = target.color;
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
                target.color = mOrigColor;
            else
                target.color = color;

            mIsMod = !mIsMod;
        }
    }
}
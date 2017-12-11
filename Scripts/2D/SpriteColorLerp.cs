using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorLerp")]
    public class SpriteColorLerp : MonoBehaviour {
        public enum Type {
            Once,
            Saw,
            SeeSaw,
            Repeat,

            NumType
        }

        public SpriteRenderer sprite;

        public Type type;

        public float delay;

        public bool useRealTime;

        public Color[] colors;

        private float mCurTime = 0;
        private float mLastTime;

        private bool mStarted = false;
        private bool mActive = false;
        private bool mReverse = false;

        public void SetCurTime(float time) {
            mCurTime = time;
        }

        void OnEnable() {
            if(mStarted) {
                mActive = true;
                mReverse = false;
                mCurTime = 0;
                sprite.color = colors[0];

                mLastTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
            }
        }

        void Awake() {
            if(sprite == null)
                sprite = GetComponent<SpriteRenderer>();
        }

        // Use this for initialization
        void Start() {
            mStarted = true;
            mActive = true;
            mReverse = false;
            sprite.color = colors[0];

            mLastTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
        }

        // Update is called once per frame
        void Update() {
            if(mActive) {
                float time = useRealTime ? Time.realtimeSinceStartup : Time.time;
                float delta = time - mLastTime;
                mLastTime = time;

                mCurTime = mCurTime + (mReverse ? -delta : delta);

                switch(type) {
                    case Type.Once:
                        if(mCurTime >= delay) {
                            mActive = false;
                            sprite.color = colors[colors.Length - 1];
                        }
                        else {
                            sprite.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                        }
                        break;

                    case Type.Repeat:
                        if(mCurTime > delay) {
                            mCurTime -= delay;
                        }

                        sprite.color = M8.ColorUtil.LerpRepeat(colors, mCurTime / delay);
                        break;

                    case Type.Saw:
                        if(mCurTime > delay) {
                            if(mReverse)
                                mCurTime -= delay;
                            else
                                mCurTime = delay - (mCurTime - delay);

                            mReverse = !mReverse;
                        }
                        else if(mReverse && mCurTime <= 0.0f) {
                            mActive = false;
                        }

                        sprite.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                        break;

                    case Type.SeeSaw:
                        if(mCurTime > delay || (mReverse && mCurTime <= 0.0f)) {
                            if(mReverse)
                                mCurTime -= delay;
                            else
                                mCurTime = delay - (mCurTime-delay);

                            mReverse = !mReverse;
                        }

                        sprite.color = M8.ColorUtil.Lerp(colors, mCurTime / delay);
                        break;
                }
            }
        }
    }
}
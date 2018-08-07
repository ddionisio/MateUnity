using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorGroup")]
    public class SpriteColorGroup : MonoBehaviour {
        public enum Type {
            Override,
            Multiply,
            Add
        }

        public Type type = Type.Multiply;
        public bool initOnAwake = true;

        private SpriteRenderer[] mSpriteRenders;
        private Color[] mGraphicDefaultColors;

        private bool mIsApplied;

        public void ApplyColor(Color color) {
            if(mSpriteRenders == null)
                Init();

            switch(type) {
                case Type.Override:
                    for(int i = 0; i < mSpriteRenders.Length; i++) {
                        if(mSpriteRenders[i])
                            mSpriteRenders[i].color = color;
                    }
                    break;
                case Type.Multiply:
                    for(int i = 0; i < mSpriteRenders.Length; i++) {
                        if(mSpriteRenders[i])
                            mSpriteRenders[i].color = mGraphicDefaultColors[i] * color;
                    }
                    break;
                case Type.Add:
                    for(int i = 0; i < mSpriteRenders.Length; i++) {
                        if(mSpriteRenders[i])
                            mSpriteRenders[i].color = new Color(
                                Mathf.Clamp01(mGraphicDefaultColors[i].r + color.r),
                                Mathf.Clamp01(mGraphicDefaultColors[i].g + color.g),
                                Mathf.Clamp01(mGraphicDefaultColors[i].b + color.b),
                                Mathf.Clamp01(mGraphicDefaultColors[i].a + color.a));
                    }
                    break;
            }

            mIsApplied = true;
        }

        public void Revert() {
            if(mIsApplied) {
                mIsApplied = false;

                if(mSpriteRenders == null)
                    return;

                for(int i = 0; i < mSpriteRenders.Length; i++) {
                    if(mSpriteRenders[i])
                        mSpriteRenders[i].color = mGraphicDefaultColors[i];
                }
            }
        }

        public void Init() {
            Revert();

            mSpriteRenders = GetComponentsInChildren<SpriteRenderer>(true);

            mGraphicDefaultColors = new Color[mSpriteRenders.Length];

            for(int i = 0; i < mSpriteRenders.Length; i++)
                mGraphicDefaultColors[i] = mSpriteRenders[i].color;
        }

        void OnDestroy() {
            Revert();
        }

        void Awake() {
            if(initOnAwake)
                Init();
        }
    }
}
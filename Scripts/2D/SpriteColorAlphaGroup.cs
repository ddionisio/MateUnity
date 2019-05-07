using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorAlphaGroup")]
    public class SpriteColorAlphaGroup : MonoBehaviour {
        
        public bool initOnAwake = true;

        public SpriteRenderer[] spriteRenders;

        public float alpha {
            get { return mAlpha; }
            set {
                if(mAlpha != value || !mIsApplied) {
                    Apply(value);
                }
            }
        }

        private float[] mGraphicDefaultAlphas;

        private bool mIsApplied = false;
        private float mAlpha = 1f;

        public void Apply(float a) {
            if(spriteRenders == null || spriteRenders.Length == 0 || (mGraphicDefaultAlphas != null && spriteRenders.Length != mGraphicDefaultAlphas.Length))
                Init();
            else if(mGraphicDefaultAlphas == null)
                InitDefaultData();

            for(int i = 0; i < spriteRenders.Length; i++) {
                if(spriteRenders[i]) {
                    var clr = spriteRenders[i].color;
                    clr.a = mGraphicDefaultAlphas[i] * a;
                    spriteRenders[i].color = clr;
                }
            }

            mIsApplied = true;
            mAlpha = a;
        }

        public void Revert() {
            if(mIsApplied) {
                mIsApplied = false;

                if(spriteRenders == null || mGraphicDefaultAlphas == null)
                    return;

                for(int i = 0; i < spriteRenders.Length; i++) {
                    if(spriteRenders[i]) {
                        var clr = spriteRenders[i].color;
                        clr.a = mGraphicDefaultAlphas[i];
                        spriteRenders[i].color = clr;
                    }
                }

                mAlpha = 1f;
            }
        }

        public void Init() {
            Revert();

            spriteRenders = GetComponentsInChildren<SpriteRenderer>(true);

            InitDefaultData();
        }

        void OnDestroy() {
            Revert();
        }

        void Awake() {
            if(initOnAwake && (spriteRenders == null || spriteRenders.Length == 0))
                Init();
        }

        private void InitDefaultData() {
            mGraphicDefaultAlphas = new float[spriteRenders.Length];

            for(int i = 0; i < spriteRenders.Length; i++)
                mGraphicDefaultAlphas[i] = spriteRenders[i].color.a;
        }
    }
}
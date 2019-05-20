using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Graphics {
    /// <summary>
    /// Use this to change colors within this hierarchy
    /// </summary>
    [AddComponentMenu("M8/UI/Graphics/ColorAlphaGroup")]
    public class ColorAlphaGroup : MonoBehaviour {

        public bool initOnAwake = true;

        public float alpha {
            get { return mAlpha; }
            set {
                if(mAlpha != value || !mIsApplied) {
                    Apply(value);
                }
            }
        }

        private Graphic[] mGraphics;
        private float[] mGraphicDefaultAlphas;

        private bool mIsApplied = false;
        private float mAlpha = 1f;

        public void Apply(float alpha) {
            if(mGraphics == null || mGraphicDefaultAlphas == null || (mGraphics != null && mGraphics.Length != mGraphicDefaultAlphas.Length))
                Init();

            for(int i = 0; i < mGraphics.Length; i++) {
                if(mGraphics[i]) {
                    var clr = mGraphics[i].color;
                    clr.a = mGraphicDefaultAlphas[i] * alpha;
                    mGraphics[i].color = clr;
                }
            }

            mAlpha = alpha;
            mIsApplied = true;
        }

        public void Revert() {
            if(mIsApplied) {
                mIsApplied = false;

                if(mGraphics == null)
                    return;

                for(int i = 0; i < mGraphics.Length; i++) {
                    if(mGraphics[i]) {
                        var clr = mGraphics[i].color;
                        clr.a = mGraphicDefaultAlphas[i];
                        mGraphics[i].color = clr;
                    }
                }
            }
        }

        public void Init() {
            Revert();

            mGraphics = GetComponentsInChildren<Graphic>(true);

            mGraphicDefaultAlphas = new float[mGraphics.Length];
            for(int i = 0; i < mGraphics.Length; i++)
                mGraphicDefaultAlphas[i] = mGraphics[i].color.a;
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
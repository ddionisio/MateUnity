using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Graphics {
    /// <summary>
    /// Use this to change colors within this hierarchy
    /// </summary>
    [AddComponentMenu("M8/UI/Graphics/ColorGroup")]
    public class ColorGroup : MonoBehaviour {
        public enum Type {
            Override,
            Multiply,
            Add
        }

        public Type type = Type.Multiply;
        public bool initOnAwake = true;

        private Graphic[] mGraphics;
        private Color[] mGraphicDefaultColors;

        private bool mIsApplied;

        public void ApplyColor(Color color) {
            if(mGraphics == null)
                Init();

            switch(type) {
                case Type.Override:
                    for(int i = 0; i < mGraphics.Length; i++) {
                        if(mGraphics[i])
                            mGraphics[i].color = color;
                    }
                    break;
                case Type.Multiply:
                    for(int i = 0; i < mGraphics.Length; i++) {
                        if(mGraphics[i])
                            mGraphics[i].color = mGraphicDefaultColors[i] * color;
                    }
                    break;
                case Type.Add:
                    for(int i = 0; i < mGraphics.Length; i++) {
                        if(mGraphics[i])
                            mGraphics[i].color = new Color(
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

                if(mGraphics == null)
                    return;

                for(int i = 0; i < mGraphics.Length; i++) {
                    if(mGraphics[i])
                        mGraphics[i].color = mGraphicDefaultColors[i];
                }
            }
        }

        public void Init() {
            Revert();

            mGraphics = GetComponentsInChildren<Graphic>(true);

            mGraphicDefaultColors = new Color[mGraphics.Length];

            for(int i = 0; i < mGraphics.Length; i++)
                mGraphicDefaultColors[i] = mGraphics[i].color;
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
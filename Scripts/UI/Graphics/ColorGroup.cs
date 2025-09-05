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

        public Color applyColor;

		public Graphic[] graphics;

		private Color[] mGraphicDefaultColors;

        private bool mIsApplied;

		public void ApplyColorToggle(bool isApply) {
			if(isApply)
				ApplyColor(applyColor);
			else
				Revert();
		}

		public void ApplyColor() {
            ApplyColor(applyColor);
        }

        public void ApplyColor(Color color) {
			if(graphics == null || graphics.Length == 0 || (mGraphicDefaultColors != null && graphics.Length != mGraphicDefaultColors.Length))
				Init();
			else if(mGraphicDefaultColors == null)
				InitDefaultData();

			applyColor = color;

            switch(type) {
                case Type.Override:
                    for(int i = 0; i < graphics.Length; i++) {
                        if(graphics[i])
                            graphics[i].color = color;
                    }
                    break;
                case Type.Multiply:
                    for(int i = 0; i < graphics.Length; i++) {
                        if(graphics[i])
                            graphics[i].color = mGraphicDefaultColors[i] * color;
                    }
                    break;
                case Type.Add:
                    for(int i = 0; i < graphics.Length; i++) {
                        if(graphics[i])
                            graphics[i].color = new Color(
                                Mathf.Clamp01(mGraphicDefaultColors[i].r + color.r),
                                Mathf.Clamp01(mGraphicDefaultColors[i].g + color.g),
                                Mathf.Clamp01(mGraphicDefaultColors[i].b + color.b),
                                Mathf.Clamp01(mGraphicDefaultColors[i].a + color.a));
                    }
                    break;
            }

            mIsApplied = true;
        }

		public void RevertToggle(bool isRevert) {
			if(isRevert)
				Revert();
			else
				ApplyColor(applyColor);
		}

		public void Revert() {
            if(mIsApplied) {
                mIsApplied = false;

                if(graphics == null)
                    return;

                for(int i = 0; i < graphics.Length; i++) {
                    if(graphics[i])
                        graphics[i].color = mGraphicDefaultColors[i];
                }
            }
        }

        public void RefreshDefaultColors() {
			if(graphics == null || mGraphicDefaultColors == null)
				return;

			Revert();
                        
            for(int i = 0; i < graphics.Length; i++) {
                var graphic = graphics[i];
                if(graphic)
                    mGraphicDefaultColors[i] = graphic.color;
            }
        }

        public void Init() {
            Revert();

            graphics = GetComponentsInChildren<Graphic>(true);

            InitDefaultData();
		}

        void OnDestroy() {
            Revert();
        }

        void Awake() {
			if(initOnAwake && (graphics == null || graphics.Length == 0))
				Init();
        }

		private void InitDefaultData() {
			mGraphicDefaultColors = new Color[graphics.Length];

			for(int i = 0; i < graphics.Length; i++)
				mGraphicDefaultColors[i] = graphics[i].color;
		}
	}
}
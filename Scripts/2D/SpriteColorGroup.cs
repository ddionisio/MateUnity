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

        [SerializeField]
		Color _applyColor = Color.white;

		public SpriteRenderer[] spriteRenders;

        public Color color {
            get { return _applyColor; }
            set {
                if(_applyColor != value || !mIsApplied) {
                    ApplyColor(value);
                }
            }
        }

        private Color[] mGraphicDefaultColors;

        private bool mIsApplied = false;

        public void ApplyColorToggle(bool isApply) {
            if(isApply)
                ApplyColor(_applyColor);
            else
                Revert();
        }

        public void ApplyColor() {
            ApplyColor(_applyColor);
        }

        public void ApplyColor(Color color) {
            if(spriteRenders == null || spriteRenders.Length == 0 || (mGraphicDefaultColors != null && spriteRenders.Length != mGraphicDefaultColors.Length))
                Init();
            else if(mGraphicDefaultColors == null)
                InitDefaultData();

            switch(type) {
                case Type.Override:
                    for(int i = 0; i < spriteRenders.Length; i++) {
                        if(spriteRenders[i])
                            spriteRenders[i].color = color;
                    }
                    break;
                case Type.Multiply:
                    for(int i = 0; i < spriteRenders.Length; i++) {
                        if(spriteRenders[i])
                            spriteRenders[i].color = mGraphicDefaultColors[i] * color;
                    }
                    break;
                case Type.Add:
                    for(int i = 0; i < spriteRenders.Length; i++) {
                        if(spriteRenders[i])
                            spriteRenders[i].color = new Color(
                                Mathf.Clamp01(mGraphicDefaultColors[i].r + color.r),
                                Mathf.Clamp01(mGraphicDefaultColors[i].g + color.g),
                                Mathf.Clamp01(mGraphicDefaultColors[i].b + color.b),
                                Mathf.Clamp01(mGraphicDefaultColors[i].a + color.a));
                    }
                    break;
            }

            mIsApplied = true;
            _applyColor = color;
        }

        public void RevertToggle(bool isRevert) {
            if(isRevert)
                Revert();
            else
                ApplyColor(_applyColor);
        }

        public void Revert() {
            if(mIsApplied) {
                mIsApplied = false;

                if(spriteRenders == null || mGraphicDefaultColors == null)
                    return;

                for(int i = 0; i < spriteRenders.Length; i++) {
                    if(spriteRenders[i])
                        spriteRenders[i].color = mGraphicDefaultColors[i];
                }

                _applyColor = Color.white;
            }
        }

        public void Init() {
            Revert();

            spriteRenders = GetComponentsInChildren<SpriteRenderer>(true);

            InitDefaultData();
        }

		void OnDidApplyAnimationProperties() {
			ApplyColor(_applyColor);
		}

		void OnDestroy() {
            Revert();
        }

        void Awake() {
            if(initOnAwake && (spriteRenders == null || spriteRenders.Length == 0))
                Init();
        }

        private void InitDefaultData() {
            mGraphicDefaultColors = new Color[spriteRenders.Length];

            for(int i = 0; i < spriteRenders.Length; i++)
                mGraphicDefaultColors[i] = spriteRenders[i].color;
        }
    }
}
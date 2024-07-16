using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorGradientGroup")]
    public class SpriteColorGradientGroup : MonoBehaviour {
		public enum Type {
			Override,
			Multiply,
		}

		public Type type = Type.Multiply;

		public SpriteRenderer[] spriteRenders;

		public Gradient gradient;

		[Range(0f, 1f)]
		[SerializeField]
		float _scale;

		public bool applyOnAwake;

		public float scale {
			get { return _scale; }
			set {
				var val = Mathf.Clamp01(value);
				if(_scale != val) {
					_scale = val;

					if(mIsApplied)
						ApplyGradient();
				}
			}
		}

		private Color[] mGraphicDefaultColors;

		private bool mIsApplied = false;

		public void Apply() {
			if(!mIsApplied) {
				mIsApplied = true;
				ApplyGradient();
			}
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
			}
		}

		void OnDestroy() {
			Revert();
		}

		void Awake() {
			if(applyOnAwake)
				Apply();
		}

		void OnDidApplyAnimationProperties() {
			mIsApplied = true;
			ApplyGradient();
		}

		private void ApplyGradient() {
			if(spriteRenders == null)
				spriteRenders = GetComponentsInChildren<SpriteRenderer>(true);

			if(mGraphicDefaultColors == null || spriteRenders.Length != mGraphicDefaultColors.Length) {
				mGraphicDefaultColors = new Color[spriteRenders.Length];

				for(int i = 0; i < spriteRenders.Length; i++) {
					if(spriteRenders[i])
						mGraphicDefaultColors[i] = spriteRenders[i].color;
				}
			}

			var clr = gradient.Evaluate(_scale);

			switch(type) {
				case Type.Override:
					for(int i = 0; i < spriteRenders.Length; i++) {
						if(spriteRenders[i])
							spriteRenders[i].color = clr;
					}
					break;

				case Type.Multiply:
					for(int i = 0; i < spriteRenders.Length; i++) {
						if(spriteRenders[i])
							spriteRenders[i].color = mGraphicDefaultColors[i] * clr;
					}
					break;
			}
		}
	}
}
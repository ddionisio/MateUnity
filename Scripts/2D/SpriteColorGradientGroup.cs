using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorGradientGroup")]
	[ExecuteInEditMode]
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

		[SerializeField]
		bool _apply;

		public bool isApplied {
			get { return _apply; }
			set {
				if(_apply != value) {
					_apply = value;

					if(_apply)
						ApplyGradient();
					else
						Revert();
				}
			}
		}

		public float scale {
			get { return _scale; }
			set {
				var val = Mathf.Clamp01(value);
				if(_scale != val) {
					_scale = val;

					if(_apply)
						ApplyGradient();
				}
			}
		}

		private Color[] mGraphicDefaultColors;

		public void Refresh() {
			if(_apply)
				ApplyGradient();
			else
				Revert();
		}

		void OnDestroy() {
			if(_apply)
				Revert();
		}

		void OnEnable() {
			if(_apply)
				ApplyGradient();
		}

		void OnDisable() {
			if(_apply)
				Revert();
		}

		void OnDidApplyAnimationProperties() {
			if(_apply)
				ApplyGradient();
		}

		private void Revert() {
			if(spriteRenders == null || mGraphicDefaultColors == null)
				return;

			for(int i = 0; i < spriteRenders.Length; i++) {
				if(spriteRenders[i])
					spriteRenders[i].color = mGraphicDefaultColors[i];
			}
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
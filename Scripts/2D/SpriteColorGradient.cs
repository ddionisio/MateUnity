using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorGradient")]
    [Tooltip("Helpful for applying color gradient to a sprite via proxy calls.")]
	[ExecuteInEditMode]
    public class SpriteColorGradient : MonoBehaviour {
		public SpriteRenderer target;
		public Gradient gradient;

		[SerializeField]
		[Range(0f, 1f)]
		float _scale;

		public float scale {
			get { return _scale; }
			set {
				var val = Mathf.Clamp01(value);
				if(_scale != val) {
					_scale = val;
					ApplyGradient();
				}
			}
		}

		public void ApplyGradient() {
			if(target == null)
				target = GetComponent<SpriteRenderer>();

			target.color = gradient.Evaluate(_scale);
		}

		void OnEnable() {
			if(gradient != null)
				ApplyGradient();
		}

		void OnDidApplyAnimationProperties() {
			ApplyGradient();
		}

		void OnValidate() {
			if(gradient != null)
				ApplyGradient();
		}
	}
}
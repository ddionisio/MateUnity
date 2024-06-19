using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorGradient")]
    [Tooltip("Helpful for applying color gradient to a sprite via proxy calls.")]
    public class SpriteColorGradient : MonoBehaviour {
		public SpriteRenderer target;
		public Gradient gradient;

		[Range(0f, 1f)]
		public float initialScale;

		public float scale {
			get { return mScale; }
			set {
				var val = Mathf.Clamp01(value);
				if(mScale != val) {
					mScale = val;
					ApplyGradient();
				}
			}
		}

		private float mScale;

		void OnEnable() {
			ApplyGradient();
		}

		void Awake() {
			mScale = initialScale;
		}

		private void ApplyGradient() {
			if(target == null)
				target = GetComponent<SpriteRenderer>();

			target.color = gradient.Evaluate(mScale);
		}
	}
}
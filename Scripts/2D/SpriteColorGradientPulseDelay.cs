using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Sprite/ColorGradientPulseDelay")]
    public class SpriteColorGradientPulseDelay : MonoBehaviour {
		public SpriteRenderer sprite;

		public Gradient gradient;

		public float startDelay;
		public float delay;
		public float pauseDelay;

		public bool squared;

		private WaitForFixedUpdate mDoUpdate;
		private WaitForSeconds mWaitSecondsStart;
		private WaitForSeconds mWaitSecondsUpdate;

		private bool mStarted = false;

		void OnEnable() {
			if(mStarted) {
				StartCoroutine(DoPulseUpdate());
			}
		}

		void OnDisable() {
			if(mStarted) {
				StopAllCoroutines();
				sprite.color = gradient.Evaluate(0f);
			}
		}

		void Awake() {
			if(sprite == null)
				sprite = GetComponent<SpriteRenderer>();

			mDoUpdate = new WaitForFixedUpdate();
			mWaitSecondsStart = new WaitForSeconds(startDelay);
			mWaitSecondsUpdate = new WaitForSeconds(pauseDelay);
		}

		// Use this for initialization
		void Start() {
			mStarted = true;
			StartCoroutine(DoPulseUpdate());
		}

		IEnumerator DoPulseUpdate() {
			sprite.color = gradient.Evaluate(0f);

			if(startDelay > 0.0f)
				yield return mWaitSecondsStart;
			else
				yield return mDoUpdate;

			float t = 0.0f;

			while(true) {
				t += Time.fixedDeltaTime;

				if(t >= delay) {
					sprite.color = gradient.Evaluate(0f);
					t = 0.0f;

					if(pauseDelay > 0.0f) {
						yield return mWaitSecondsUpdate;
						continue;
					}
				}
				else {
					float s = Mathf.Sin(Mathf.PI * (t / delay));
					sprite.color = gradient.Evaluate(squared ? s * s : s);
				}

				yield return mDoUpdate;
			}
		}
	}
}
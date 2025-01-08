using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace M8 {
	[AddComponentMenu("M8/Sprite/ColorGradientGroupPulseDelay")]
	public class SpriteColorGradientGroupPulseDelay : SpriteColorGradientGroup {
		public float startDelay;
		public float delay;
		public float pauseDelay;

		public bool squared;

		private WaitForFixedUpdate mDoUpdate;
		private WaitForSeconds mWaitSecondsStart;
		private WaitForSeconds mWaitSecondsUpdate;

		private bool mStarted = false;
		protected override void OnEnable() {
			base.OnEnable();

			if(mStarted)
				StartCoroutine(DoPulseUpdate());
		}

		protected override void OnDisable() {
			base.OnDisable();

			if(mStarted)
				StopAllCoroutines();
		}

		void Awake() {
			mDoUpdate = new WaitForFixedUpdate();
			mWaitSecondsStart = new WaitForSeconds(startDelay);
			mWaitSecondsUpdate = new WaitForSeconds(pauseDelay);
		}

		// Use this for initialization
		void Start() {
			if(Application.isPlaying) {
				mStarted = true;
				StartCoroutine(DoPulseUpdate());
			}
		}

		IEnumerator DoPulseUpdate() {
			scale = 0f;

			if(startDelay > 0.0f)
				yield return mWaitSecondsStart;
			else
				yield return mDoUpdate;

			float t = 0.0f;

			while(true) {
				t += Time.fixedDeltaTime;

				if(t >= delay) {
					scale = 0f;
					t = 0.0f;

					if(pauseDelay > 0.0f) {
						yield return mWaitSecondsUpdate;
						continue;
					}
				}
				else {
					float s = Mathf.Sin(Mathf.PI * (t / delay));
					scale = squared ? s * s : s;
				}

				yield return mDoUpdate;
			}
		}
	}
}
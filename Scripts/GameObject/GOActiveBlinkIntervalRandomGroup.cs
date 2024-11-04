using System.Collections;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Game Object/Active Blink Interval Random Group")]
	public class GOActiveBlinkIntervalRandomGroup : MonoBehaviour {
		public GameObject[] activeGOs;
		public GameObject[] inactiveGOs;

		public bool activeStart = true;

		public RangeFloat delayRange;

		public RangeInt blinkCount;
		public RangeFloat blinkDelayRange;

		public bool isRealTime;

		private Coroutine mRout;

		private IEnumerator mWaitDelay;
		private IEnumerator mWaitBlinkDelay;

		void OnEnable() {
			mRout = StartCoroutine(DoInterval());
		}

		void OnDisable() {
			if(mRout != null) {
				StopCoroutine(mRout);
				mRout = null;
			}

			ApplyActive(activeStart);
		}

		void Awake() {
			if(isRealTime) {
				mWaitDelay = new WaitForSecondsRealtimeRandom(delayRange.min, delayRange.max);
				mWaitBlinkDelay = new WaitForSecondsRealtimeRandom(blinkDelayRange.min, blinkDelayRange.max);
			}
			else {
				mWaitDelay = new WaitForSecondsRandom(delayRange.min, delayRange.max);
				mWaitBlinkDelay = new WaitForSecondsRandom(blinkDelayRange.min, blinkDelayRange.max);
			}

			ApplyActive(activeStart);
		}

		IEnumerator DoInterval() {
			var _active = activeStart;

			ApplyActive(_active);

			while(true) {
				yield return mWaitDelay;

				var count = blinkCount.random;
				if(count < 0) count = 1;

				for(int i = 0; i < count; i++) {
					ApplyActive(!_active);

					yield return mWaitBlinkDelay;

					ApplyActive(_active);

					yield return mWaitBlinkDelay;
				}
			}
		}

		private void ApplyActive(bool active) {
			for(int i = 0; i < activeGOs.Length; i++) {
				var go = activeGOs[i];
				if(go) go.SetActive(active);
			}

			for(int i = 0; i < inactiveGOs.Length; i++) {
				var go = inactiveGOs[i];
				if(go) go.SetActive(!active);
			}
		}
	}
}
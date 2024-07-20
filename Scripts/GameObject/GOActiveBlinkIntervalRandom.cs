using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Game Object/Active Blink Interval Random")]
	public class GOActiveBlinkIntervalRandom : MonoBehaviour {		
		public GameObject activeGO;
		public GameObject inactiveGO;

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
		}

		IEnumerator DoInterval() {
			var _active = activeStart;

			if(activeGO) activeGO.SetActive(_active);
			if(inactiveGO) inactiveGO.SetActive(!_active);

			while(true) {
				yield return mWaitDelay;

				var count = blinkCount.random;
				if(count < 0) count = 1;

				for(int i = 0; i < count; i++) {
					if(activeGO) activeGO.SetActive(!_active);
					if(inactiveGO) inactiveGO.SetActive(_active);

					yield return mWaitBlinkDelay;

					if(activeGO) activeGO.SetActive(_active);
					if(inactiveGO) inactiveGO.SetActive(!_active);
				}
			}
		}
	}
}
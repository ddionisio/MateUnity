using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Game Object/Active Interval Random")]
	public class GOActiveIntervalRandom : MonoBehaviour {		
		public GameObject activeGO;
		public GameObject inactiveGO;

		public bool activeStart = true;

		public RangeFloat delayRange;

		public RangeFloat delayBlinkRange;

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
				mWaitBlinkDelay = new WaitForSecondsRealtimeRandom(delayBlinkRange.min, delayBlinkRange.max);
			}
			else {
				mWaitDelay = new WaitForSecondsRandom(delayRange.min, delayRange.max);
				mWaitBlinkDelay = new WaitForSecondsRandom(delayBlinkRange.min, delayBlinkRange.max);
			}
		}

		IEnumerator DoInterval() {
			var _active = activeStart;

			while(true) {
				if(activeGO) activeGO.SetActive(_active);
				if(inactiveGO) inactiveGO.SetActive(!_active);

				yield return mWaitDelay;

				if(activeGO) activeGO.SetActive(!_active);
				if(inactiveGO) inactiveGO.SetActive(_active);

				yield return mWaitBlinkDelay;
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	public class WaitForSecondsRandom : CustomYieldInstruction {
		private float mMin;
		private float mMax;

		private bool mIsGenerated;
		private float mDelay;
		private float mCurTime;

		public WaitForSecondsRandom(float min, float max) {
			mMin = min;
			mMax = max;
			mIsGenerated = false;
		}

		public override bool keepWaiting {
			get {
				if(!mIsGenerated) {
					mDelay = Random.Range(mMin, mMax);
					mCurTime = 0f;
					mIsGenerated = true;
				}

				mCurTime += Time.deltaTime;
				if(mCurTime >= mDelay) {
					mIsGenerated = false;
					return false;
				}

				return true;
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace M8 {
    [AddComponentMenu("M8/Sprite/SpriteColorGradientGroupLerp")]
    public class SpriteColorGradientGroupLerp : SpriteColorGradientGroup {
		public enum LerpType {
			Once,
			Saw,
			SeeSaw,
			Repeat
		}

		public LerpType lerpType;

		public float delay;

		public bool useRealTime;

		private float mCurTime = 0;
		private float mLastTime;

		private bool mStarted = false;
		private bool mActive = false;
		private bool mReverse = false;

		public void SetCurTime(float time) {
			mCurTime = time;
		}

		protected override void OnEnable() {
			//base.OnEnable();

			if(mStarted) {
				mActive = true;
				mReverse = false;
				mCurTime = 0;
				scale = 0f;

				mLastTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
			}
		}

		// Use this for initialization
		void Start() {
			mStarted = true;
			mActive = true;
			mReverse = false;
			scale = 0f;

			mLastTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
		}

		// Update is called once per frame
		void Update() {
			if(mActive) {
				float time = useRealTime ? Time.realtimeSinceStartup : Time.time;
				float delta = time - mLastTime;
				mLastTime = time;

				mCurTime = mCurTime + (mReverse ? -delta : delta);

				scale = Mathf.Clamp01(mCurTime / delay);

				switch(lerpType) {
					case LerpType.Once:
						if(mCurTime >= delay)
							mActive = false;
						break;

					case LerpType.Repeat:
						if(mCurTime >= delay)
							mCurTime = 0f;
						break;

					case LerpType.Saw:
						if(mCurTime >= delay) {
							if(mReverse)
								mCurTime = 0f;
							else
								mCurTime = delay;

							mReverse = !mReverse;
						}
						else if(mReverse && mCurTime <= 0.0f) {
							mActive = false;
						}
						break;

					case LerpType.SeeSaw:
						if(mReverse && mCurTime <= 0f) {
							mCurTime = 0f;
							mReverse = false;
						}
						else if(mCurTime >= delay) {
							mCurTime = delay;
							mReverse = true;
						}
						break;
				}
			}
		}
	}
}
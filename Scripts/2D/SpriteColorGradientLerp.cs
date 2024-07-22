using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Sprite/ColorGradientLerp")]
	public class SpriteColorGradientLerp : MonoBehaviour {
		public enum Type {
			Once,
			Saw,
			SeeSaw,
			Repeat
		}

		public SpriteRenderer sprite;

		public Type type;

		public float delay;

		public bool useRealTime;

		public Gradient gradient;

		private float mCurTime = 0;
		private float mLastTime;

		private bool mStarted = false;
		private bool mActive = false;
		private bool mReverse = false;

		public void SetCurTime(float time) {
			mCurTime = time;
		}

		void OnEnable() {
			if(mStarted) {
				mActive = true;
				mReverse = false;
				mCurTime = 0;
				sprite.color = gradient.Evaluate(0f);

				mLastTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
			}
		}

		void Awake() {
			if(sprite == null)
				sprite = GetComponent<SpriteRenderer>();
		}

		// Use this for initialization
		void Start() {
			mStarted = true;
			mActive = true;
			mReverse = false;
			sprite.color = gradient.Evaluate(0f);

			mLastTime = useRealTime ? Time.realtimeSinceStartup : Time.time;
		}

		// Update is called once per frame
		void Update() {
			if(mActive) {
				float time = useRealTime ? Time.realtimeSinceStartup : Time.time;
				float delta = time - mLastTime;
				mLastTime = time;

				mCurTime = mCurTime + (mReverse ? -delta : delta);

				sprite.color = gradient.Evaluate(Mathf.Clamp01(mCurTime / delay));

				switch(type) {
					case Type.Once:
						if(mCurTime >= delay)
							mActive = false;
						break;

					case Type.Repeat:
						if(mCurTime >= delay)
							mCurTime = 0f;
						break;

					case Type.Saw:
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

					case Type.SeeSaw:
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
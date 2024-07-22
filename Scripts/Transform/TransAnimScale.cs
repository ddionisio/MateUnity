using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Transform/Anim Scale")]
	public class TransAnimScale : MonoBehaviour {
		public enum UpdateType {
			Update,
			FixedUpdate,
			Realtime
		}

		private enum State {
			Pause,
			Pulse
		}

		public Transform target;

		public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public UpdateType updateType = UpdateType.Update;
		public bool startPause;

		public float pauseDelay;
		public float delay;

		public Vector3 startScale = Vector3.one;
		public Vector3 endScale = Vector3.one;

		private State mState;
		private float mLastTime;

		void OnEnable() {
			target.localScale = startScale;
			mState = startPause ? State.Pause : State.Pulse;
			mLastTime = GetTime();
		}

		void Awake() {
			if(!target) target = transform;
		}

		void Update() {
			var time = GetTime();

			switch(mState) {
				case State.Pause:
					if(time - mLastTime >= pauseDelay) {
						mState = State.Pulse;
						mLastTime = time;
					}
					break;

				case State.Pulse:
					var delta = time - mLastTime;
					if(delta < delay) {
						var t = curve.Evaluate(Mathf.Clamp01(delta / delay));

						target.localScale = Vector3.Lerp(startScale, endScale, t);
					}
					else {
						target.localScale = endScale;
												
						mLastTime = time;

						if(pauseDelay > 0.0f)
							mState = State.Pause;
					}
					break;
			}
		}

		float GetTime() {
			switch(updateType) {
				case UpdateType.Update:
					return Time.time;
				case UpdateType.FixedUpdate:
					return Time.fixedTime;
				case UpdateType.Realtime:
					return Time.realtimeSinceStartup;
			}
			return 0.0f;
		}
	}
}
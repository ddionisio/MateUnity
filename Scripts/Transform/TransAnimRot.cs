using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Transform/Anim Rotation")]
    public class TransAnimRot : MonoBehaviour {
		public enum UpdateType {
			Update,
			FixedUpdate,
			Realtime
		}

		private enum State {
			Pause,
			Play
		}

		public Transform target;

		public AnimationCurve curve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

		public UpdateType updateType = UpdateType.Update;
		public bool startPause;

		public float pauseDelay;
		public float delay;

		public Vector3 startRotation = Vector3.zero;
		public Vector3 endRotation = Vector3.zero;
		public bool isLocal = true;

		private State mState;
		private float mLastTime;

		private Quaternion mRotS;
		private Quaternion mRotE;

		void OnEnable() {
			ApplyRotation(mRotS);
			mState = startPause ? State.Pause : State.Play;
			mLastTime = GetTime();
		}

		void Awake() {
			if(!target) target = transform;

			mRotS = Quaternion.Euler(startRotation);
			mRotE = Quaternion.Euler(endRotation);
		}

		void Update() {
			var time = GetTime();

			switch(mState) {
				case State.Pause:
					if(time - mLastTime >= pauseDelay) {
						mState = State.Play;
						mLastTime = time;
					}
					break;

				case State.Play:
					var delta = time - mLastTime;
					if(delta < delay) {
						var t = Mathf.Clamp01(curve.Evaluate(delta / delay));

						ApplyRotation(Quaternion.Slerp(mRotS, mRotE, t));
					}
					else {
						ApplyRotation(mRotE);

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

		void ApplyRotation(Quaternion rot) {
			if(isLocal)
				target.localRotation = rot;
			else
				target.rotation = rot;
		}
	}
}
#if !M8_UNITY_ANIMATOR_DISABLED
using System;
using System.Collections;
using UnityEngine;

namespace M8 {
	public abstract class AnimatorParam {
		[SerializeField]
		int _nameID;

		public abstract AnimatorControllerParameterType paramType { get; }

		public int nameID { get { return _nameID; } }
	}

	public abstract class AnimatorParamValue<T> : AnimatorParam {
		public abstract T Get(UnityEngine.Animator animator);
		public abstract void Set(UnityEngine.Animator animator, T val);
	}

	[Serializable]
	public class AnimatorParamTrigger : AnimatorParam {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Trigger; } }

		public void Set(UnityEngine.Animator animator) {
			animator.SetTrigger(nameID);
		}

		public void Reset(UnityEngine.Animator animator) {
			animator.ResetTrigger(nameID);
		}

		public IEnumerator WaitTrigger(UnityEngine.Animator animator) {
			while(true) {
				yield return null;

				if(animator.GetBool(nameID))
					break;
			}

			while(true) {
				yield return null;

				if(!animator.GetBool(nameID))
					break;
			}
		}
	}

	[Serializable]
	public class AnimatorParamBool : AnimatorParamValue<bool> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Bool; } }

		public override bool Get(UnityEngine.Animator animator) {
			return animator.GetBool(nameID);
		}

		public override void Set(UnityEngine.Animator animator, bool val) {
			animator.SetBool(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorParamInt : AnimatorParamValue<int> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Int; } }

		public override int Get(UnityEngine.Animator animator) {
			return animator.GetInteger(nameID);
		}

		public override void Set(UnityEngine.Animator animator, int val) {
			animator.SetInteger(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorParamFloat : AnimatorParamValue<float> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Float; } }

		public override float Get(UnityEngine.Animator animator) {
			return animator.GetFloat(nameID);
		}

		public override void Set(UnityEngine.Animator animator, float val) {
			animator.SetFloat(nameID, val);
		}
	}

	//target version

	public abstract class AnimatorTargetParam {
		[SerializeField]
		UnityEngine.Animator _target;

		[SerializeField]
		int _nameID;

		public abstract AnimatorControllerParameterType paramType { get; }

		public UnityEngine.Animator target { get { return _target; } }

		public int nameID { get { return _nameID; } }
	}

	public abstract class AnimatorTargetParamValue<T> : AnimatorTargetParam {
		public abstract T Get();
		public abstract void Set(T val);
	}

	[Serializable]
	public class AnimatorTargetParamTrigger : AnimatorTargetParam {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Trigger; } }

		public void Set() {
			if(target)
				target.SetTrigger(nameID);
		}

		public void Reset() {
			if(target)
				target.ResetTrigger(nameID);
		}
	}

	[Serializable]
	public class AnimatorTargetParamBool : AnimatorTargetParamValue<bool> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Bool; } }

		public override bool Get() {
			return target ? target.GetBool(nameID) : false;
		}

		public override void Set(bool val) {
			if(target)
				target.SetBool(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorTargetParamInt : AnimatorTargetParamValue<int> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Int; } }

		public override int Get() {
			return target ? target.GetInteger(nameID) : 0;
		}

		public override void Set(int val) {
			if(target)
				target.SetInteger(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorTargetParamFloat : AnimatorTargetParamValue<float> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Float; } }

		public override float Get() {
			return target ? target.GetFloat(nameID) : 0f;
		}

		public override void Set(float val) {
			if(target)
				target.SetFloat(nameID, val);
		}
	}
}
#endif
using System;
using UnityEngine;

namespace M8 {
	public abstract class AnimatorParam {
		[SerializeField]
		int _nameID;

		public abstract AnimatorControllerParameterType paramType { get; }

		public int nameID { get { return _nameID; } }
	}

	public abstract class AnimatorParamValue<T> : AnimatorParam {
		public abstract T Get(Animator animator);
		public abstract void Set(Animator animator, T val);
	}

	[Serializable]
	public class AnimatorParamTrigger : AnimatorParam {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Trigger; } }

		public void Set(Animator animator) {
			animator.SetTrigger(nameID);
		}

		public void Reset(Animator animator) {
			animator.ResetTrigger(nameID);
		}
	}

	[Serializable]
	public class AnimatorParamBool : AnimatorParamValue<bool> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Bool; } }

		public override bool Get(Animator animator) {
			return animator.GetBool(nameID);
		}

		public override void Set(Animator animator, bool val) {
			animator.SetBool(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorParamInt : AnimatorParamValue<int> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Int; } }

		public override int Get(Animator animator) {
			return animator.GetInteger(nameID);
		}

		public override void Set(Animator animator, int val) {
			animator.SetInteger(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorParamFloat : AnimatorParamValue<float> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Float; } }

		public override float Get(Animator animator) {
			return animator.GetFloat(nameID);
		}

		public override void Set(Animator animator, float val) {
			animator.SetFloat(nameID, val);
		}
	}

	//target version

	public abstract class AnimatorTargetParam {
		[SerializeField]
		Animator _target;

		[SerializeField]
		int _nameID;

		public abstract AnimatorControllerParameterType paramType { get; }

		public Animator target { get { return _target; } }

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
			target.SetTrigger(nameID);
		}

		public void Reset() {
			target.ResetTrigger(nameID);
		}
	}

	[Serializable]
	public class AnimatorTargetParamBool : AnimatorTargetParamValue<bool> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Bool; } }

		public override bool Get() {
			return target.GetBool(nameID);
		}

		public override void Set(bool val) {
			target.SetBool(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorTargetParamInt : AnimatorTargetParamValue<int> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Int; } }

		public override int Get() {
			return target.GetInteger(nameID);
		}

		public override void Set(int val) {
			target.SetInteger(nameID, val);
		}
	}

	[Serializable]
	public class AnimatorTargetParamFloat : AnimatorTargetParamValue<float> {
		public override AnimatorControllerParameterType paramType { get { return AnimatorControllerParameterType.Float; } }

		public override float Get() {
			return target.GetFloat(nameID);
		}

		public override void Set(float val) {
			target.SetFloat(nameID, val);
		}
	}
}
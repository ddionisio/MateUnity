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
}
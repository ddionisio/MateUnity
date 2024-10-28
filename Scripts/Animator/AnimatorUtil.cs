using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

namespace M8 {
    public struct AnimatorUtil {
        public static IEnumerator WaitNextState(UnityEngine.Animator animator) {
			var curState = animator.GetCurrentAnimatorStateInfo(0);

			while(true) {
				yield return null;

				var state = animator.GetCurrentAnimatorStateInfo(0);

				if(state.fullPathHash != curState.fullPathHash && state.normalizedTime >= 1f)
						break;
			}
		}

		public static AnimatorController GetAnimatorController(UnityEngine.Animator animator) {
			RuntimeAnimatorController rAnimCtrl = animator.runtimeAnimatorController;
			while(rAnimCtrl is AnimatorOverrideController)
				rAnimCtrl = ((AnimatorOverrideController)rAnimCtrl).runtimeAnimatorController;

			return rAnimCtrl as AnimatorController;
		}
	}
}
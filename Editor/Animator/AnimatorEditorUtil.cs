#if !M8_UNITY_ANIMATOR_DISABLED
using UnityEditor.Animations;

namespace M8 {
	public struct AnimatorEditorUtil {
		public static AnimatorController GetAnimatorController(UnityEngine.Animator animator) {
			UnityEngine.RuntimeAnimatorController rAnimCtrl = animator.runtimeAnimatorController;
			while(rAnimCtrl is UnityEngine.AnimatorOverrideController)
				rAnimCtrl = ((UnityEngine.AnimatorOverrideController)rAnimCtrl).runtimeAnimatorController;

			return rAnimCtrl as AnimatorController;
		}
	}
}
#endif
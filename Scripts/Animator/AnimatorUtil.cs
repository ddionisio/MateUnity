#if !M8_UNITY_ANIMATOR_DISABLED
using System.Collections;

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
	}
}
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public struct AnimatorUtil {
        public static IEnumerator WaitNextState(Animator animator) {
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
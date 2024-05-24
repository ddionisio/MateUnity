using System.Collections;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Perform modal animation when they open/close. Ensure clips used by states do not loop indefinitely.
    /// Ideally the state setup would be: Idle -triggerOpen-> Opening -triggerClose-> Closing -> Idle
    /// </summary>
    [AddComponentMenu("M8/Animator Unity/ModalTransition")]
    [RequireComponent(typeof(Animator))]
    public class AnimatorModalTransition : MonoBehaviour, IModalOpening, IModalClosing {
		public AnimatorParamTrigger triggerOpen;
		public AnimatorParamTrigger triggerClose;

		public Animator animator { get; private set; }

        void Awake() {
			animator = GetComponent<Animator>();
		}

		IEnumerator IModalOpening.Opening() {
			triggerOpen.Set(animator);

			while(true) {
				yield return null;

				var state = animator.GetCurrentAnimatorStateInfo(0);
				if(state.normalizedTime >= 1f)
					break;
			}
		}

        IEnumerator IModalClosing.Closing() {
			triggerClose.Set(animator);

			while(true) {
				yield return null;

				var state = animator.GetCurrentAnimatorStateInfo(0);
				if(state.normalizedTime >= 1f)
					break;
			}
		}
	}
}
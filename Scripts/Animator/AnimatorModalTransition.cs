using System.Collections;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Perform modal animation when they open/close. Ensure clips used by states do not loop indefinitely.
    /// Ideally the state setup would be: Idle -triggerOpen-> Opening -triggerClose-> Closing -> Idle
    /// </summary>
    [AddComponentMenu("M8/Animator Unity/ModalTransition")]
    [RequireComponent(typeof(UnityEngine.Animator))]
    public class AnimatorModalTransition : MonoBehaviour, IModalOpening, IModalClosing {
		public AnimatorParamTrigger triggerOpen;
		public AnimatorParamTrigger triggerClose;

		public UnityEngine.Animator animator { get; private set; }

        void Awake() {
			animator = GetComponent<UnityEngine.Animator>();
		}

		IEnumerator IModalOpening.Opening() {
			triggerOpen.Set(animator);

			yield return AnimatorUtil.WaitNextState(animator);
		}

        IEnumerator IModalClosing.Closing() {
			triggerClose.Set(animator);

			yield return AnimatorUtil.WaitNextState(animator);
		}
	}
}
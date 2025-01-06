#if !M8_UNITY_ANIMATOR_DISABLED
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
		public enum WaitMode {
			Animation,
			Delay,
			DelayRealtime
		}

		public WaitMode waitMode = WaitMode.Animation;

		public AnimatorParamTrigger triggerOpen;		
		public float openDelay;

		public AnimatorParamTrigger triggerClose;
		public float closeDelay;

		public UnityEngine.Animator animator { get; private set; }

		void Awake() {
			animator = GetComponent<UnityEngine.Animator>();
		}

		IEnumerator IModalOpening.Opening() {
			triggerOpen.Set(animator);

			switch(waitMode) {
				case WaitMode.Animation:
					yield return AnimatorUtil.WaitNextState(animator);
					break;

				case WaitMode.Delay:
					yield return new WaitForSeconds(openDelay);
					break;

				case WaitMode.DelayRealtime:
					yield return new WaitForSecondsRealtime(openDelay);
					break;
			}
		}

        IEnumerator IModalClosing.Closing() {
			triggerClose.Set(animator);

			switch(waitMode) {
				case WaitMode.Animation:
					yield return AnimatorUtil.WaitNextState(animator);
					break;

				case WaitMode.Delay:
					yield return new WaitForSeconds(closeDelay);
					break;

				case WaitMode.DelayRealtime:
					yield return new WaitForSecondsRealtime(closeDelay);
					break;
			}
		}
	}
}
#endif
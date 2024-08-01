using System.Collections;
using UnityEngine;

namespace M8 {
	/// <summary>
	/// Perform scene transition animation. Ensure clips used by states do not loop indefinitely.
	/// Ideally the state setup would be: Idle -triggerExit-> Exiting -triggerEnter-> Entering -> Idle
	/// </summary>
	[AddComponentMenu("M8/Animator Unity/SceneTransition")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSceneTransition : MonoBehaviour, SceneManager.ITransition {		
		public AnimatorParamTrigger triggerEnter;
		public AnimatorParamTrigger triggerExit;

		public Animator animator { get; private set; }

		void OnDestroy() {
			if(SceneManager.isInstantiated)
				SceneManager.instance.RemoveTransition(this);
		}

		void Awake() {
			SceneManager.instance.AddTransition(this);

			animator = GetComponent<Animator>();
		}

		int SceneManager.ITransition.priority { get { return 0; } }

		IEnumerator SceneManager.ITransition.Out() {
			triggerExit.Set(animator);

			yield return AnimatorUtil.WaitNextState(animator);
		}

		IEnumerator SceneManager.ITransition.In() {
			triggerEnter.Set(animator);

			yield return AnimatorUtil.WaitNextState(animator);
		}
	}
}
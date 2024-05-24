using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Animator Unity/SceneTransition")]
	[RequireComponent(typeof(Animator))]
	public class AnimatorSceneTransition : MonoBehaviour, SceneManager.ITransition {		
		public AnimatorParamTrigger triggerEnter;
		public AnimatorParamTrigger triggerExit;
		public float waitDelay = 0.2f;

		public Animator animator { get; private set; }

		private WaitForSeconds mWait;

		void OnDestroy() {
			if(SceneManager.isInstantiated)
				SceneManager.instance.RemoveTransition(this);
		}

		void Awake() {
			SceneManager.instance.AddTransition(this);

			animator = GetComponent<Animator>();

			if(waitDelay > 0f)
				mWait = new WaitForSeconds(waitDelay);
		}

		int SceneManager.ITransition.priority { get { return 0; } }

		IEnumerator SceneManager.ITransition.Out() {
			triggerExit.Set(animator);

			yield return mWait;
		}

		IEnumerator SceneManager.ITransition.In() {
			triggerEnter.Set(animator);

			yield return mWait;
		}
	}
}
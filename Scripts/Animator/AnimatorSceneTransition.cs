using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
	[AddComponentMenu("M8/Animator Unity/SceneTransition")]
	public class AnimatorSceneTransition : MonoBehaviour, SceneManager.ITransition {
		public Animator animator;
		public string triggerEnter;
		public string triggerExit;
		public float waitDelay = 0.2f;

		private int mTriggerEnterInd = -1;
		private int mTriggerExitInd = -1;

		void OnDestroy() {
			if(SceneManager.isInstantiated)
				SceneManager.instance.RemoveTransition(this);
		}

		void Awake() {
			SceneManager.instance.AddTransition(this);

			//grab trigger indices
			var parms = animator.parameters;
			for(int i = 0; i < parms.Length; i++) {
				if(parms[i].name == triggerEnter)
					mTriggerEnterInd = i;
				else if(parms[i].name == triggerExit)
					mTriggerExitInd = i;
			}
		}

		int SceneManager.ITransition.priority { get { return 0; } }

		IEnumerator SceneManager.ITransition.Out() {
			if(mTriggerExitInd != -1)
				animator.SetTrigger(mTriggerExitInd);

			if(waitDelay > 0f)
				yield return new WaitForSeconds(waitDelay);
		}

		IEnumerator SceneManager.ITransition.In() {			

			animator.SetTrigger(mTriggerEnterInd);

			if(waitDelay > 0f)
				yield return new WaitForSeconds(waitDelay);
		}
	}
}
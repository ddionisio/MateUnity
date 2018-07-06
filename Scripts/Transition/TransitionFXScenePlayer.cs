using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Add this component with ScreenTrans to play when M8.SceneManager changes to new scene
    /// </summary>
    [AddComponentMenu("M8/TransitionFX/Scene Player")]
    public class TransitionFXScenePlayer : MonoBehaviour, SceneManager.ITransition {
        int SceneManager.ITransition.priority { get { return 0; } }

        public TransitionFX transitionOut;
        public TransitionFX transitionIn;
        
        void OnDestroy() {
            if(SceneManager.instance)
                SceneManager.instance.RemoveTransition(this);
        }

        void Awake() {
            SceneManager.instance.AddTransition(this);
        }

        IEnumerator SceneManager.ITransition.Out() {
            if(transitionOut) {
                transitionOut.Play();

                while(transitionOut.isPlaying)
                    yield return null;
            }
            else if(transitionIn) {
                transitionIn.Prepare();
                transitionIn.isRenderActive = true;
            }
        }

        IEnumerator SceneManager.ITransition.In() {
            if(transitionOut) {
                yield return new WaitForEndOfFrame(); //wait one render

                transitionOut.End();
            }

            if(transitionIn) {
                transitionIn.Play();

                while(transitionIn.isPlaying)
                    yield return null;

                transitionIn.End();
            }
        }
    }
}
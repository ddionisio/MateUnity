using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Add this component with ScreenTrans to play when M8.SceneManager changes to new scene
    /// </summary>
    [AddComponentMenu("M8/TransitionFX/Scene Player")]
    public class TransitionFXScenePlayer : MonoBehaviour, SceneManager.ITransition {
        int SceneManager.ITransition.priority { get { return 0; } }

        [Tooltip("Transition to play when current scene is exiting out.")]
        public TransitionFX transitionOut;
        [Tooltip("Transition to play when next scene is entering in.")]
        public TransitionFX transitionIn;

        public float delay = 0.333f;

        public bool isTimeScaled = false;

        void OnDestroy() {
            if(SceneManager.instance)
                SceneManager.instance.RemoveTransition(this);

            if(transitionOut)
                transitionOut.Deinit();
            if(transitionIn)
                transitionIn.Deinit();
        }

        void Awake() {
            SceneManager.instance.AddTransition(this);
        }
        
        IEnumerator SceneManager.ITransition.Out() {
            if(transitionOut) {
                float curTime = 0f;
                while(curTime < delay) {
                    yield return null;
                    curTime += isTimeScaled ? Time.deltaTime : Time.unscaledDeltaTime;
                    transitionOut.UpdateTime(curTime / delay);
                }
            }
            else if(transitionIn) {
                transitionIn.UpdateTime(0f);
            }
        }

        IEnumerator SceneManager.ITransition.In() {
            if(transitionOut) {
                yield return null; //wait one frame
                transitionOut.End();
            }

            if(transitionIn) {
                float curTime = 0f;
                while(curTime < delay) {                    
                    curTime += isTimeScaled ? Time.deltaTime : Time.unscaledDeltaTime;
                    transitionIn.UpdateTime(curTime / delay);
                    yield return null;
                }

                transitionIn.End();
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Add this to a modal object to pause the game on Push, then unpause on Pop
    /// </summary>
    [AddComponentMenu("M8/Modal/Helpers/Pause On Push")]
    public class ModalPause : MonoBehaviour, IModalPush, IModalPop {

        private bool mIsPaused;

        public void Pause(bool pause) {
            if(mIsPaused != pause) {
                mIsPaused = pause;

                if(SceneManager.instance) {
                    if(mIsPaused)
                        SceneManager.instance.Pause();
                    else
                        SceneManager.instance.Resume();
                }
            }
        }

        void IModalPush.Push(GenericParams parms) {
            Pause(true);
        }

        void IModalPop.Pop() {
            Pause(false);
        }

        void OnDestroy() {
            Pause(false); //fail-safe
        }
    }
}
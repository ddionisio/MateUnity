using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8.UIModal.Helpers {
    /// <summary>
    /// Add this to a modal object to pause the game on Push, then unpause on Pop
    /// </summary>
    [AddComponentMenu("M8/UI Modal/Helpers/Pause On Push")]
    public class PauseOnPush : MonoBehaviour, Interface.IPush, Interface.IPop {

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

        void Interface.IPush.Push(GenericParams parms) {
            Pause(true);
        }

        void Interface.IPop.Pop() {
            Pause(false);
        }
    }
}
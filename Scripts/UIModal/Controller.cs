using UnityEngine;
using System.Collections;

namespace M8.UIModal {
    [AddComponentMenu("M8/UI Modal/Controller")]
    [DisallowMultipleComponent]
    public class Controller : MonoBehaviour {
        public bool exclusive = true; //hide modals behind if this is the top

        public delegate void ActiveCallback(bool active);
        public delegate void Callback();

        public bool isActive { get { return mActive; } }

        public event ActiveCallback onActiveCallback;

        /// <summary>
        /// Called whenever this controller becomes the top modal (active) or a new modal is pushed (inactive), once everything has opened/closed.
        /// </summary>
        protected virtual void OnActive(bool active) { }

        /// <summary>
        /// Called when modal needs to show up, use this to refresh ui stuff.
        /// </summary>
        public virtual void Open() { }

        /// <summary>
        /// Called when modal needs to hide.
        /// </summary>
        public virtual void Close() { }

        private bool mActive;

        //don't call these, only uimodalmanager

        public void _active(bool active) {
            if(mActive != active) {
                mActive = active;

                if(active) {
                    OnActive(true);

                    if(onActiveCallback != null)
                        onActiveCallback(true);
                }
                else {
                    OnActive(false);

                    if(onActiveCallback != null)
                        onActiveCallback(false);
                }
            }
        }
    }
}
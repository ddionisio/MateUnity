using UnityEngine;
using System.Collections;

namespace M8.UIModal {
    [AddComponentMenu("M8/UI Modal/Controller")]
    [DisallowMultipleComponent]
    public class Controller : MonoBehaviour, Interface.IActive {
        [Tooltip("Hide modals behind if this is the top")]
        public bool exclusive = true;

        [Tooltip("Root to deactive after close.  Leave empty to use self.")]
        [SerializeField]
        GameObject _root;

        public delegate void ActiveCallback(bool active);

        public bool isActive { get { return mActive; } }

        public GameObject root { get { return _root ? _root : gameObject; } }

        public event ActiveCallback onActiveCallback;
        
        private bool mActive;

        //don't call these, only uimodalmanager

        void Interface.IActive.SetActive(bool aActive) {
            if(mActive != aActive) {
                mActive = aActive;

                if(aActive) {
                    if(onActiveCallback != null)
                        onActiveCallback(true);
                }
                else {
                    if(onActiveCallback != null)
                        onActiveCallback(false);
                }
            }
        }
    }
}
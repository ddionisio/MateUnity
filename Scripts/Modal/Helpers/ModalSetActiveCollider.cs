using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Enable/disable colliders during Modal.SetActive.
    /// </summary>
    [AddComponentMenu("M8/Modal/Helpers/Set Active Collider")]
    public class ModalSetActiveCollider : MonoBehaviour, IModalActive {
#if !M8_PHYSICS_DISABLED
        [Tooltip("Colliders to enable/disable.")]
        public Collider[] colliders;
#endif

        void IModalActive.SetActive(bool aActive) {
#if !M8_PHYSICS_DISABLED
            for(int i = 0; i < colliders.Length; i++) {
                if(colliders[i])
                    colliders[i].enabled = aActive;
            }
#endif
        }
    }
}
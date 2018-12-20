using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Enable/disable colliders during Modal.SetActive.
    /// </summary>
    [AddComponentMenu("M8/Modal/Helpers/Set Active Collider")]
    public class ModalSetActiveCollider : MonoBehaviour, IModalActive {
        [Tooltip("Colliders to enable/disable.")]
        public Collider[] colliders;

        void IModalActive.SetActive(bool aActive) {
            for(int i = 0; i < colliders.Length; i++) {
                if(colliders[i])
                    colliders[i].enabled = aActive;
            }
        }
    }
}
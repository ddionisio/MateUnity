using UnityEngine;
using UnityEngine.EventSystems;

namespace M8 {
    /// <summary>
    /// Add this along UIController to set given 'select' as selected in Unity's EventSystem
    /// </summary>
    [AddComponentMenu("M8/Modal/Helpers/Set Active Select")]
    public class ModalSetActiveSelect : MonoBehaviour, IModalActive {
        public GameObject select;

        void IModalActive.SetActive(bool aActive) {
            if(aActive) {
                var es = EventSystem.current;
                if(es)
                    es.SetSelectedGameObject(select);
            }
        }
    }
}
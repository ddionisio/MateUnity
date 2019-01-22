using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/SelectGOSetActive")]
    public class SelectGOSetActive : MonoBehaviour, ISelectHandler, IDeselectHandler {

        public GameObject target;

        void OnEnable() {
            EventSystem es = EventSystem.current;
            if(es)
                target.SetActive(es.currentSelectedGameObject == gameObject);
        }

        void OnDisable() {
            target.SetActive(false);
        }

        void ISelectHandler.OnSelect(BaseEventData eventData) {
            if(!isActiveAndEnabled) return;

            target.SetActive(eventData.selectedObject == gameObject);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData) {
            if(!isActiveAndEnabled) return;

            target.SetActive(false);
        }
    }
}
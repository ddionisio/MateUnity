using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/Click")]
    public class EventClick : MonoBehaviour, IPointerClickHandler {
        public UnityEvent clickEvent;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            clickEvent.Invoke();
        }
    }
}
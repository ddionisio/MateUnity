using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/Click")]
    public class EventClick : MonoBehaviour, IPointerClickHandler {
        public UnityEvent clickEvent;
        public Signal clickSignal;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            clickEvent.Invoke();

            if(clickSignal)
                clickSignal.Invoke();
        }
    }
}
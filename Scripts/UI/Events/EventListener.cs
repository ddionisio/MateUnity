using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [System.Serializable]
    public class EventListenerPointer : UnityEvent<PointerEventData> {

    }

    [System.Serializable]
    public class EventListenerBase : UnityEvent<BaseEventData> {

    }

    [AddComponentMenu("M8/UI/Events/EventListener")]
    public class EventListener : UIBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler {

        public EventListenerPointer onPointerDown;
        public EventListenerPointer onPointerUp;
        public EventListenerPointer onPointerEnter;
        public EventListenerPointer onPointerExit;
        public EventListenerBase onSelect;
        public EventListenerBase onDeselect;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(onPointerDown != null)
                onPointerDown.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if(onPointerUp != null)
                onPointerUp.Invoke(eventData);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if(onPointerEnter != null)
                onPointerEnter.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if(onPointerExit != null)
                onPointerExit.Invoke(eventData);
        }

        void ISelectHandler.OnSelect(BaseEventData eventData) {
            if(onSelect != null)
                onSelect.Invoke(eventData);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData) {
            if(onDeselect != null)
                onDeselect.Invoke(eventData);
        }
    }
}
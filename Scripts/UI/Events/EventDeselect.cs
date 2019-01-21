using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/Deselect")]
    public class EventDeselect : MonoBehaviour, IDeselectHandler {
        public UnityEvent deselectEvent;

        void IDeselectHandler.OnDeselect(BaseEventData eventData) {
            deselectEvent.Invoke();
        }
    }
}
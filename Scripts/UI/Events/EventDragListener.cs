using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
	[AddComponentMenu("M8/UI/Events/EventDragListener")]
	public class EventDragListener : UIBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler {
		public EventListenerPointer onDragBegin;
		public EventListenerPointer onDrag;
		public EventListenerPointer onDragEnd;

		void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
			onDragBegin?.Invoke(eventData);
		}

		void IDragHandler.OnDrag(PointerEventData eventData) {
			onDrag?.Invoke(eventData);
		}

		void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
			onDragEnd?.Invoke(eventData);
		}
	}
}
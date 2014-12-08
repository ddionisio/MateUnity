using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/HoverGOSetActive")]
    public class HoverGOSetActive : UIBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler {

        public GameObject target;

        private bool mIsPointerDown;
        private bool mIsPointerInside;

        protected override void OnEnable() {
            base.OnEnable();

            EventSystem es = EventSystem.current;
            if(es)
                target.SetActive(es.currentSelectedGameObject == gameObject);
        }

        protected override void OnDisable() {
            mIsPointerDown = false;
            mIsPointerInside = false;

            target.SetActive(false);

            base.OnDisable();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(!IsActive()) return;

            mIsPointerDown = true;

            target.SetActive(mIsPointerInside || eventData.pointerPress == gameObject);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if(!IsActive()) return;

            mIsPointerDown = false;

            target.SetActive(mIsPointerInside);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if(!IsActive()) return;

            mIsPointerInside = true;

            target.SetActive(mIsPointerDown || eventData.pointerPress == null);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if(!IsActive()) return;

            mIsPointerInside = false;

            target.SetActive(mIsPointerDown && eventData.pointerPress == gameObject);
        }

        void ISelectHandler.OnSelect(BaseEventData eventData) {
            if(!IsActive()) return;

            target.SetActive(eventData.selectedObject == gameObject);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData) {
            if(!IsActive()) return;

            target.SetActive(false);
        }
    }
}
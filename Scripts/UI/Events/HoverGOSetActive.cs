﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/HoverGOSetActive")]
    public class HoverGOSetActive : MonoBehaviour,
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler,
        ISelectHandler, IDeselectHandler {

        public GameObject target;
        [Tooltip("If true, target remains active if this gameObject is selected.")]
        public bool selectSticky;

        private bool mIsPointerDown;
        private bool mIsPointerInside;

        private Selectable mSelectable;
        private CanvasGroup mCanvasGroup;

        void OnEnable() {
            if(IsNotInteractable()) {
                target.SetActive(false);
            }
            else {
                EventSystem es = EventSystem.current;
                if(es)
                    target.SetActive(es.currentSelectedGameObject == gameObject);
            }
        }

        void OnDisable() {
            mIsPointerDown = false;
            mIsPointerInside = false;

            target.SetActive(false);
        }

        void Awake() {
            mSelectable = GetComponent<Selectable>();
            mCanvasGroup = GetComponentInParent<CanvasGroup>(true);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(!isActiveAndEnabled) return;

            mIsPointerDown = true;

            if(IsNotInteractable())
                target.SetActive(false);
            else if(selectSticky && eventData.selectedObject == gameObject)
                target.SetActive(true);
            else
                target.SetActive(mIsPointerInside || eventData.pointerPress == gameObject);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if(!isActiveAndEnabled) return;

            mIsPointerDown = false;

            if(IsNotInteractable())
                target.SetActive(false);
            else if(selectSticky && eventData.selectedObject == gameObject)
                target.SetActive(true);
            else
                target.SetActive(mIsPointerInside);
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            if(!isActiveAndEnabled) return;


            mIsPointerInside = true;

            if(IsNotInteractable())
                target.SetActive(false);
            else if(selectSticky && eventData.selectedObject == gameObject)
                target.SetActive(true);
            else
                target.SetActive(mIsPointerDown || eventData.pointerPress == null);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if(!isActiveAndEnabled) return;
                        
            mIsPointerInside = false;

            if(IsNotInteractable())
                target.SetActive(false);
            else if(selectSticky && eventData.selectedObject == gameObject)
                target.SetActive(true);
            else
                target.SetActive(mIsPointerDown && eventData.pointerPress == gameObject);
        }

        void ISelectHandler.OnSelect(BaseEventData eventData) {
            if(!isActiveAndEnabled) return;

            if(IsNotInteractable())
                target.SetActive(false);
            else
                target.SetActive(eventData.selectedObject == gameObject);
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData) {
            if(!isActiveAndEnabled) return;

            target.SetActive(false);
        }

        private bool IsNotInteractable() {
            if(mCanvasGroup && !mCanvasGroup.interactable)
                return true;
            else if(mSelectable && !mSelectable.interactable)
                return true;
            return false;
        }
    }
}
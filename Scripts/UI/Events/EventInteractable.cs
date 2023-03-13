using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace M8.UI.Events {
    [AddComponentMenu("M8/UI/Events/Interactable")]
    public class EventInteractable : MonoBehaviour {
        public Selectable target;

        public UnityEvent enableEvent;
        public UnityEvent disableEvent;

        private bool mIsInteractable;

        private CanvasGroup mCanvasGroup;

        void OnEnable() {
            mIsInteractable = IsInteractable();
            if(mIsInteractable)
                enableEvent.Invoke();
            else
                disableEvent.Invoke();
        }

        void Awake() {
            if(!target)
                target = GetComponent<Selectable>();

            mCanvasGroup = GetComponentInParent<CanvasGroup>(true);
        }

        void Update() {
            var isInteractable = IsInteractable();
            if(mIsInteractable != isInteractable) {
                mIsInteractable = isInteractable;
                if(mIsInteractable)
                    enableEvent.Invoke();
                else
                    disableEvent.Invoke();
            }
        }

        private bool IsInteractable() {
            if(mCanvasGroup && !mCanvasGroup.interactable)
                return false;

            if(target && target.interactable)
                return true;

            return false;
        }
    }
}
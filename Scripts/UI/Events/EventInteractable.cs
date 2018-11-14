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

        void OnEnable() {
            mIsInteractable = target.interactable;
            if(mIsInteractable)
                enableEvent.Invoke();
            else
                disableEvent.Invoke();
        }

        void Update() {
            if(mIsInteractable != target.interactable) {
                mIsInteractable = target.interactable;
                if(mIsInteractable)
                    enableEvent.Invoke();
                else
                    disableEvent.Invoke();
            }
        }
    }
}
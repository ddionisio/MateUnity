using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    /// <summary>
    /// Use this to allow calling of Signal.Invoke through hookups
    /// </summary>
    [AddComponentMenu("M8/Signals/Listener")]
    public class SignalListener : MonoBehaviour {
        public Signal signal;

        [Tooltip("If true, only listen when this behaviour is enabled.")]
        [SerializeField]
        bool _activeOnly;

        public UnityEvent onSignal;

        void OnDisable() {
            if(_activeOnly) {
                if(signal)
                    signal.callback -= OnSignal;
            }
        }

        void OnEnable() {
            if(_activeOnly) {
                if(signal)
                    signal.callback += OnSignal;
            }
        }

        void OnDestroy() {
            if(!_activeOnly) {
                if(signal)
                    signal.callback -= OnSignal;
            }
        }

        void Awake() {
            if(!_activeOnly) {
                if(signal)
                    signal.callback += OnSignal;
            }
        }

        void OnSignal() {
            onSignal.Invoke();
        }
    }
}
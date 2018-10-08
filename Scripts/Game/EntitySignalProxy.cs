using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    /// <summary>
    /// Simple signal proxy for when entity is active.
    /// </summary>
    [AddComponentMenu("M8/Entity/Signal Proxy")]
    public class EntitySignalProxy : MonoBehaviour {
        [SerializeField]
        EntityBase _target;
        [SerializeField]
        Signal _signal;
        [SerializeField]
        UnityEvent _event;

        void OnDestroy() {
            if(_target) {
                _target.spawnCallback -= OnEntitySpawned;
                _target.releaseCallback -= OnEntityRelease;
            }
        }

        void Awake() {
            if(!_target) _target = GetComponent<EntityBase>();

            if(_target) {
                _target.spawnCallback += OnEntitySpawned;
                _target.releaseCallback += OnEntityRelease;
            }
        }

        void OnEntitySpawned(EntityBase ent) {
            if(_signal) _signal.callback += OnSignal;
        }

        void OnEntityRelease(EntityBase ent) {
            if(_signal) _signal.callback -= OnSignal;
        }

        void OnSignal() {
            _event.Invoke();
        }
    }
}
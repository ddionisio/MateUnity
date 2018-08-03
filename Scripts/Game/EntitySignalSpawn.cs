using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace M8 {
    [AddComponentMenu("M8/Entity/Signal Spawn")]
    public class EntitySignalSpawn : MonoBehaviour {
        [SerializeField]
        EntityBase _target;
        [SerializeField]
        SignalEntity _signal;
        [SerializeField]
        UnityEvent<EntityBase> _event;

        void OnDestroy() {
            if(_target)
                _target.spawnCallback -= OnEntitySpawn;
        }

        void Awake() {
            if(!_target) _target = GetComponent<EntityBase>();

            if(_target)
                _target.spawnCallback += OnEntitySpawn;
        }

        void OnEntitySpawn(EntityBase ent) {
            if(_signal)
                _signal.Invoke(ent);

            _event.Invoke(ent);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to set the state of StateController during spawning, after OnSpawn.
    /// </summary>
    [AddComponentMenu("M8/Game/State Controller Spawn")]
    public class StateControllerSpawn : MonoBehaviour, IPoolSpawn, IPoolSpawnComplete, IPoolDespawn {
        public const string parmState = "stateOnSpawn"; //use this to override spawnState

        public StateController controller;

        public State spawnDefaultState;

        public bool isDespawnClearState = true;

        private State mToState;

        void Awake() {
            if(!controller)
                controller = GetComponent<StateController>();
        }

        void IPoolSpawn.OnSpawned(GenericParams parms) {
            if(parms != null && parms.ContainsKey(parmState)) {
                mToState = parms.GetValue<State>(parmState);
            }
            else
                mToState = spawnDefaultState;
        }

        void IPoolSpawnComplete.OnSpawnComplete() {
            controller.state = mToState;
        }

        void IPoolDespawn.OnDespawned() {
            if(isDespawnClearState)
                controller.state = null;
        }
    }
}
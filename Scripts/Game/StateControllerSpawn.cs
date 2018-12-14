using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Use this to set the state of StateController during OnSpawn, state is changed on update to allow others to initialize during OnSpawned
    /// </summary>
    [AddComponentMenu("M8/Game/State Controller Spawn")]
    public class StateControllerSpawn : MonoBehaviour, IPoolSpawn, IPoolDespawn {
        public const string parmState = "stateCtrlSpawnState"; //use this to override spawnState

        public StateController controller;

        public State spawnState;

        public bool isDespawnClearState = true;

        private State mToState;
        private bool mIsSpawning;

        void Awake() {
            if(!controller)
                controller = GetComponent<StateController>();
        }

        void Update() {
            if(mIsSpawning) {
                mIsSpawning = false;
                controller.state = mToState;
            }
        }

        void IPoolSpawn.OnSpawned(GenericParams parms) {
            if(parms != null && parms.ContainsKey(parmState)) {
                mToState = parms.GetValue<State>(parmState);
            }
            else
                mToState = spawnState;

            mIsSpawning = true; //allow others to initialize during OnSpawned, wait for Update to happen before applying state
        }

        void IPoolDespawn.OnDespawned() {
            mIsSpawning = false;

            if(isDespawnClearState)
                controller.state = null;
        }
    }
}
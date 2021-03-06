﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Game/State Controller")]
    public class StateController : MonoBehaviour, IPoolDespawn {
        public UnityEventState stateChangedEvent;

        public State state {
            get { return mState; }

            set {
                if(mState != value) {
                    if(mState != null)
                        mPrevState = mState;

                    mState = value;

                    stateChangedEvent.Invoke(mState);
                }
            }
        }

        public State prevState {
            get { return mPrevState; }
        }

        private State mState = null;
        private State mPrevState = null;

        public void SetState(State state) {
            this.state = state;
        }

        /// <summary>
        /// Force change to the same state, will also set prevState to the current state
        /// </summary>
        public void RestartState() {
            mPrevState = mState;

            stateChangedEvent.Invoke(mState);
        }

        void IPoolDespawn.OnDespawned() {
            mState = null;
        }
    }
}
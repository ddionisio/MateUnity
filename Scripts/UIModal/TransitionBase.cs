using UnityEngine;
using System.Collections;

namespace M8.UIModal {
    [AddComponentMenu("")]
    [RequireComponent(typeof(Controller))]
    [DisallowMultipleComponent]
    public abstract class TransitionBase : MonoBehaviour {
        public enum State {
            None,
            Open,
            Close
        }

        private State mState = State.None;

        public State state { get { return mState; } }

        public IEnumerator Open() {
            mState = State.Open;
            yield return StartCoroutine(Opening());
            mState = State.None;
        }

        public IEnumerator Close() {
            mState = State.Close;
            yield return StartCoroutine(Closing());
            mState = State.None;
        }
       
        protected abstract IEnumerator Opening();
        protected abstract IEnumerator Closing();

        void OnDisable() {
            mState = State.None;
        }
    }
}
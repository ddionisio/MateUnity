using UnityEngine;
using System.Collections;

namespace M8.Auxiliary {
    [AddComponentMenu("M8/Auxiliary/Visible")]
    [RequireComponent(typeof(Renderer))]
    public class AuxVisible : MonoBehaviour {
        public delegate void Callback();

        public event Callback visibleCallback;
        public event Callback invisibleCallback;

        void OnDestroy() {
            visibleCallback = null;
            invisibleCallback = null;
        }

        void OnBecameInvisible() {
            if(invisibleCallback != null)
                invisibleCallback();
        }

        void OnBecameVisible() {
            if(visibleCallback != null)
                visibleCallback();
        }
    }
}
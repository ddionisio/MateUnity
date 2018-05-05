using UnityEngine;

namespace M8 {
    /// <summary>
    /// Very simple invokable signal with no parameters.
    /// </summary>
    [CreateAssetMenu(fileName = "signal", menuName = "Signals/Signal")]
    public class Signal : ScriptableObject {
        public event System.Action callback;

        public void Invoke() {
            if(callback != null)
                callback();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    /// <summary>
    /// Very simple invokable signal with 1 parameter
    /// </summary>
    public abstract class SignalParam<T> : ScriptableObject {
        public event System.Action<T> callback;

        public void Invoke(T parm) {
            if(callback != null)
                callback(parm);
        }
    }

    /// <summary>
    /// Very simple invokable signal with 2 parameters
    /// </summary>
    public abstract class SignalParam<T1, T2> : ScriptableObject {
        public event System.Action<T1, T2> callback;

        public void Invoke(T1 parm1, T2 parm2) {
            if(callback != null)
                callback(parm1, parm2);
        }
    }

    /// <summary>
    /// Very simple invokable signal with 3 parameters
    /// </summary>
    public abstract class SignalParam<T1, T2, T3> : ScriptableObject {
        public event System.Action<T1, T2, T3> callback;

        public void Invoke(T1 parm1, T2 parm2, T3 parm3) {
            if(callback != null)
                callback(parm1, parm2, parm3);
        }
    }

    /// <summary>
    /// Very simple invokable signal with 4 parameters
    /// </summary>
    public abstract class SignalParam<T1, T2, T3, T4> : ScriptableObject {
        public event System.Action<T1, T2, T3, T4> callback;

        public void Invoke(T1 parm1, T2 parm2, T3 parm3, T4 parm4) {
            if(callback != null)
                callback(parm1, parm2, parm3, parm4);
        }
    }
}
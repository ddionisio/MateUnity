using UnityEngine;

[AddComponentMenu("M8/Auxiliary/FixedUpdate")]
public class AuxFixedUpdate : MonoBehaviour {
    public delegate void Callback();

    public event Callback callback;

    void OnDestroy() {
        callback = null;
    }

    void FixedUpdate() {
        if(callback != null)
            callback();
    }
}

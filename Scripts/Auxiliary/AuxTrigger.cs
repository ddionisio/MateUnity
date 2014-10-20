using UnityEngine;

[AddComponentMenu("M8/Auxiliary/Trigger")]
public class AuxTrigger : MonoBehaviour {
    public delegate void Callback(Collider other);

    public event Callback enterCallback;
    public event Callback stayCallback;
    public event Callback exitCallback;

    void OnDestroy() {
        enterCallback = null;
        stayCallback = null;
        exitCallback = null;
    }

    void OnTriggerEnter(Collider other) {
        if(enterCallback != null)
            enterCallback(other);
    }

    void OnTriggerStay(Collider other) {
        if(stayCallback != null)
            stayCallback(other);
    }

    void OnTriggerExit(Collider other) {
        if(exitCallback != null)
            exitCallback(other);
    }
}

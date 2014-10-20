using UnityEngine;

[AddComponentMenu("M8/Auxiliary/Collision")]
public class AuxCollision : MonoBehaviour {
    public delegate void Callback(Collision coll);

    public event Callback enterCallback;
    public event Callback stayCallback;
    public event Callback exitCallback;

    void OnDestroy() {
        enterCallback = null;
        stayCallback = null;
        exitCallback = null;
    }

    void OnCollisionEnter(Collision coll) {
        if(enterCallback != null)
            enterCallback(coll);
    }

    void OnCollisionStay(Collision coll) {
        if(stayCallback != null)
            stayCallback(coll);
    }

    void OnCollisionExit(Collision coll) {
        if(exitCallback != null)
            exitCallback(coll);
    }
}

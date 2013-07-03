using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/2D/TransUpLock")]
public class TransUpLock : MonoBehaviour {
    public Vector2 up = Vector2.up;

    public Transform target;

    void Awake() {
        if(target == null)
            target = transform;
    }

    void Update() {
        target.up = up;
    }
}

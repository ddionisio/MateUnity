using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/UpLockFixedUpdate")]
public class TransUpLockFixedUpdate : MonoBehaviour {
    public Vector3 up = Vector3.up;

    public Transform target;

    void Awake() {
        if(target == null)
            target = transform;
    }

    void FixedUpdate() {
        target.up = up;
    }
}

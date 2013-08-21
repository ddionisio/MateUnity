using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Transform/AnimSpinner")]
public class TransAnimSpinner : MonoBehaviour {

    public Vector3 rotatePerSecond;
    public bool local = true;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(local) {
            transform.localEulerAngles = transform.localEulerAngles + rotatePerSecond * Time.deltaTime;
        }
        else {
            transform.eulerAngles = transform.eulerAngles + rotatePerSecond * Time.deltaTime;
        }
    }
}

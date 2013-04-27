using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Debug/ColliderDebug")]
public class ColliderDebug : MonoBehaviour {
    public bool logCollisionStay = false;
    public bool logTriggerStay = false;

    void OnTriggerEnter(Collider c) {
        Debug.Log(gameObject.name + " trigger enter with: " + c.gameObject.name);
    }

    void OnTriggerExit(Collider c) {
        Debug.Log(gameObject.name + " trigger exit with: " + c.gameObject.name);
    }

    void OnTriggerStay(Collider c) {
        if(logTriggerStay)
            Debug.Log(gameObject.name + " trigger stay with: " + c.gameObject.name);
    }

    void OnCollisionEnter(Collision c) {
        foreach(ContactPoint contact in c.contacts) {
            Debug.Log(gameObject.name + " collision enter contact with: " + contact.otherCollider.gameObject.name);
        }
    }

    void OnCollisionExit(Collision c) {
        foreach(ContactPoint contact in c.contacts) {
            Debug.Log(gameObject.name + " collision enter contact with: " + contact.otherCollider.gameObject.name);
        }
    }

    void OnCollisionStay(Collision c) {
        if(logCollisionStay) {
            foreach(ContactPoint contact in c.contacts) {
                Debug.Log(gameObject.name + " collision enter contact with: " + contact.otherCollider.gameObject.name);
            }
        }
    }
}

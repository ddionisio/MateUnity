using UnityEngine;

[AddComponentMenu("M8/Physics/ColliderBounce")]
public class ColliderBounce : MonoBehaviour {
    public float force = 30.0f;
    public bool atPoint = false;

    void OnCollisionEnter(Collision col) {
        foreach(ContactPoint contact in col.contacts) {
            if(atPoint)
                contact.otherCollider.rigidbody.AddForceAtPosition(-contact.normal * force, contact.point, ForceMode.Impulse);
            else
                contact.otherCollider.rigidbody.AddForce(-contact.normal * force, ForceMode.Impulse);
        }
    }
}

using UnityEngine;

//just to help visual circular area in the scene
[AddComponentMenu("M8/Gizmo Helpers/Wire Sphere")]
public class GizmoHelperWireSphere : MonoBehaviour {
    public Color color = Color.white;
    public float radius = 1.0f;

    void OnDrawGizmos() {
        if(radius > 0.0f) {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
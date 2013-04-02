using UnityEngine;

//just to help visual circular area in the scene
[AddComponentMenu("M8/Gizmo Helpers/Sphere")]
public class GizmoHelperSphere : MonoBehaviour {
    public Color color = Color.white;
    public float radius = 1.0f;
    public bool solid = false;

    void OnDrawGizmos() {
        if(radius > 0.0f) {
            Gizmos.color = color;

            if(solid)
                Gizmos.DrawSphere(transform.position, radius);
            else
                Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
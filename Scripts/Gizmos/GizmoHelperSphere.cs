using UnityEngine;

//just to help visual circular area in the scene
[AddComponentMenu("M8/Gizmo Helpers/Sphere")]
public class GizmoHelperSphere : MonoBehaviour {
    public Color color = Color.white;
    public float radius = 1.0f;
    public bool solid = false;
    public bool useCollider = false;

    void OnDrawGizmos() {
        Vector3 ofs = Vector3.zero;

        if(useCollider) {
            SphereCollider sc = collider != null ? collider as SphereCollider : null;
            if(sc != null) {
                radius = sc.radius;
                ofs = sc.center;
            }
        }

        if(radius > 0.0f) {
            Gizmos.color = color;

            if(solid)
                Gizmos.DrawSphere(transform.position + ofs, radius);
            else
                Gizmos.DrawWireSphere(transform.position + ofs, radius);
        }
    }
}
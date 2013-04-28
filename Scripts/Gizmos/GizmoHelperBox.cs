using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Gizmo Helpers/Box")]
public class GizmoHelperBox : MonoBehaviour {
    public Bounds bound = new Bounds(Vector3.zero, Vector3.one);
    public Color color = Color.white;
    public bool solid = false;

    void OnDrawGizmos() {
        if(bound.size.x > 0 && bound.size.y > 0 && bound.size.z > 0) {
            Gizmos.color = color;

            if(solid)
                Gizmos.DrawCube(transform.position + bound.center, bound.size);
            else
                Gizmos.DrawWireCube(transform.position + bound.center, bound.size);
        }
    }
}

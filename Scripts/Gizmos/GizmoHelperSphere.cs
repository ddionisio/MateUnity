using UnityEngine;

namespace M8 {
    //just to help visual circular area in the scene
    [AddComponentMenu("M8/Gizmo Helpers/Sphere")]
    public class GizmoHelperSphere : MonoBehaviour {
        public Vector3 offset;
        public Color color = Color.white;
        public float radius = 1.0f;
        public bool solid = false;
        public bool useCollider = false;

        void OnDrawGizmos() {
            Vector3 ofs = offset;

            if(useCollider) {
                SphereCollider sc = collider != null ? collider as SphereCollider : null;
                if(sc != null) {
                    radius = sc.radius;
                    ofs += sc.center;
                }
            }

            ofs = transform.rotation * ofs;

            if(radius > 0.0f) {
                Gizmos.color = color;

                if(solid)
                    Gizmos.DrawSphere(transform.position + ofs, radius);
                else
                    Gizmos.DrawWireSphere(transform.position + ofs, radius);
            }
        }
    }
}
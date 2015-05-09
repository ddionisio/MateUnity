using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Gizmo Helpers/Box")]
    public class GizmoHelperBox : MonoBehaviour {
        public Bounds bound = new Bounds(Vector3.zero, Vector3.one);
        public Color color = Color.white;
        public bool solid = false;
        public bool useCollider = false;

        void OnDrawGizmos() {

            if(useCollider) {
                BoxCollider bc = GetComponent<BoxCollider>();
                if(bc != null) {
                    bound.center = bc.center;
                    bound.extents = new Vector3(bc.size.x*transform.localScale.x, bc.size.y*transform.localScale.y, bc.size.z*transform.localScale.z) * 0.5f;
                }
            }

            if(bound.size.x > 0 && bound.size.y > 0 && bound.size.z > 0) {
                Gizmos.color = color;

                if(solid)
                    Gizmos.DrawCube(transform.position + bound.center, bound.size);
                else
                    Gizmos.DrawWireCube(transform.position + bound.center, bound.size);
            }
        }
    }
}
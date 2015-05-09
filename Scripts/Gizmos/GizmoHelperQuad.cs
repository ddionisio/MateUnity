using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Gizmo Helpers/Quad")]
    public class GizmoHelperQuad : MonoBehaviour {
        public Bounds bound = new Bounds(Vector3.zero, Vector3.one);
        public Color color = Color.white;
        public bool useCollider = false;

        void OnDrawGizmos() {

            if(useCollider) {
                BoxCollider bc = GetComponent<BoxCollider>();
                if(bc != null) {
                    bound.center = bc.center;
                    bound.extents = new Vector3(bc.size.x * transform.localScale.x, bc.size.y * transform.localScale.y, bc.size.z * transform.localScale.z) * 0.5f;
                }
            }

            if(bound.size.x > 0 && bound.size.y > 0 && bound.size.z > 0) {
                Gizmos.color = color;

                Vector3 ul = transform.localToWorldMatrix.MultiplyPoint(bound.center + new Vector3(-bound.extents.x, bound.extents.y));
                Vector3 ur = transform.localToWorldMatrix.MultiplyPoint(bound.center + new Vector3(bound.extents.x, bound.extents.y));
                Vector3 ll = transform.localToWorldMatrix.MultiplyPoint(bound.center + new Vector3(-bound.extents.x, -bound.extents.y));
                Vector3 lr = transform.localToWorldMatrix.MultiplyPoint(bound.center + new Vector3(bound.extents.x, -bound.extents.y));

                Gizmos.DrawLine(ul, ur);
                Gizmos.DrawLine(ur, lr);
                Gizmos.DrawLine(lr, ll);
                Gizmos.DrawLine(ll, ul);
            }
        }
    }
}
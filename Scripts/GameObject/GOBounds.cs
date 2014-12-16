using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Convenience for having a fixed bound for object, use for tiling
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/Game Object/Bounds")]
    public class GOBounds : MonoBehaviour {
        public Bounds bounds;

        //For gizmos
        [SerializeField]
        Color gizmoColor = Color.white;
        [SerializeField]
        bool gizmoSolid;

        void OnDrawGizmos() {
            if(bounds.size.x > 0 && bounds.size.y > 0 && bounds.size.z > 0) {
                Gizmos.color = gizmoColor;

                if(gizmoSolid)
                    Gizmos.DrawCube(transform.position + bounds.center, bounds.size);
                else
                    Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
            }
        }
    }
}
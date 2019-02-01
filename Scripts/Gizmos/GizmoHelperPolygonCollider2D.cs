using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Gizmo Helpers/PolygonCollider2D")]
    public class GizmoHelperPolygonCollider2D : MonoBehaviour {
        public Color color = Color.green;

        void OnDrawGizmos() {
            var polyColl = GetComponent<PolygonCollider2D>();
            if(polyColl) {
                Gizmos.color = color;

                var ofs = polyColl.offset;

                for(int pathInd = 0; pathInd < polyColl.pathCount; pathInd++) {
                    var pts = polyColl.GetPath(pathInd);
                    for(int i = 0; i < pts.Length - 1; i++) {
                        var p1 = transform.TransformPoint(pts[i] + ofs);
                        var p2 = transform.TransformPoint(pts[i + 1] + ofs);

                        Gizmos.DrawLine(p1, p2);
                    }

                    if(pts.Length > 2) {
                        var p1 = transform.TransformPoint(pts[pts.Length - 1] + ofs);
                        var p2 = transform.TransformPoint(pts[0] + ofs);

                        Gizmos.DrawLine(p1, p2);
                    }
                }
            }
        }
    }
}
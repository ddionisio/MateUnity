using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Gizmo Helpers/Rect")]
    public class GizmoHelperRect : MonoBehaviour {
        public Rect rect;
        public Color color = Color.white;
        public bool useRectTransform;

        void OnDrawGizmos() {
            if(useRectTransform) {
                var rectT = transform as RectTransform;
                if(rectT) {
                    rect = rectT.rect;
                }
            }

            //grab corners and draw wire
            var t = transform;

            var p0 = t.TransformPoint(new Vector2(rect.xMin, rect.yMin));
            var p1 = t.TransformPoint(new Vector2(rect.xMax, rect.yMin));
            var p2 = t.TransformPoint(new Vector2(rect.xMax, rect.yMax));
            var p3 = t.TransformPoint(new Vector2(rect.xMin, rect.yMax));

            Gizmos.color = color;

            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p1, p2);
            Gizmos.DrawLine(p2, p3);
            Gizmos.DrawLine(p3, p0);
        }
    }
}
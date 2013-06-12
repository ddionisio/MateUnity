using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//helper stuff
namespace M8 {
    public struct NGUIExtUtil {
        /// <summary>
        /// Set the given widget's size to contain given bound. Sets widget's position, size, and pivot to topleft.
        /// </summary>
        public static void WidgetEncapsulateBoundsLocal(UIWidget widget, Vector2 padding, Bounds bounds) {
            widget.pivot = UIWidget.Pivot.TopLeft;

            Transform t = widget.cachedTransform;
            Vector3 pos = t.localPosition;
            Vector3 s = t.localScale;

            pos.x = bounds.min.x - padding.x;
            s.x = bounds.size.x + padding.x * 2.0f;
            pos.y = bounds.max.y + padding.y;
            s.y = bounds.size.y + padding.y * 2.0f;

            t.localPosition = pos;
            t.localScale = s;
        }
    }
}

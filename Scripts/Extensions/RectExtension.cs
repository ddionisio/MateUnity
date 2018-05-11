using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    public static class RectExtension {
        /// <summary>
        /// Returns a clamped center based on its given extension (ext)
        /// </summary>
        public static Vector2 Clamp(this Rect rect, Vector2 center, Vector2 ext) {
            Vector2 min = (Vector2)rect.min + ext;
            Vector2 max = (Vector2)rect.max - ext;

            float extX = rect.width * 0.5f;
            float extY = rect.height * 0.5f;

            if(extX > ext.x)
                center.x = Mathf.Clamp(center.x, min.x, max.x);
            else
                center.x = rect.center.x;

            if(extY > ext.y)
                center.y = Mathf.Clamp(center.y, min.y, max.y);
            else
                center.y = rect.center.y;

            return center;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8.UI.Layouts {
    /// <summary>
    /// Attach to target's position and dimension
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("M8/UI/Layouts/Anchor Stretch")]
    public class LayoutAnchorStretch : MonoBehaviour {
        public RectTransform source;
        public RectTransform target;

        [Header("Adjustments")]
        public Vector2 offset;
        public float paddingLeft;
        public float paddingRight;
        public float paddingTop;
        public float paddingBottom;

        private Vector2 mTargetLastSizeDelta;
        private Vector3 mTargetLastPos;
        private Quaternion mTargetLastRotate;
        
        public void Apply() {
            if(!target)
                return;

            source.pivot = target.pivot;            
            source.anchorMin = new Vector2(0.5f, 0.5f);
            source.anchorMax = new Vector2(0.5f, 0.5f);

            Vector2 ofs = offset;

            ofs.x += paddingRight * source.pivot.x - paddingLeft * (1.0f - source.pivot.x);
            ofs.y += paddingTop * source.pivot.y - paddingBottom * (1.0f - source.pivot.y);

            source.position = target.TransformPoint(ofs);
            source.rotation = target.rotation;
            source.localScale = target.localScale;

            Vector2 size = target.sizeDelta;
            size.x += paddingLeft + paddingRight;
            size.y += paddingTop + paddingBottom;

            source.sizeDelta = size;
                        
            mTargetLastPos = target.position;
            mTargetLastRotate = target.rotation;
            mTargetLastSizeDelta = target.sizeDelta;
        }

        void Awake() {
            if(!source)
                source = transform as RectTransform;
        }

        void Update() {
            if(!source || !target)
                return;

            if(mTargetLastSizeDelta != target.sizeDelta || mTargetLastPos != target.position || mTargetLastRotate != target.rotation)
                Apply();
        }
    }
}
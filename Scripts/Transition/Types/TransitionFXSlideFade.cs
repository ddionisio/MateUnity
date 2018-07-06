using UnityEngine;
using System.Collections;

namespace M8 {
    [CreateAssetMenu(fileName = "slideFade", menuName = "M8/TransitionFX/Slide Fade")]
    public class TransitionFXSlideFade : TransitionFX {
        public SourceType source = SourceType.CameraSnapShot;
        public Texture sourceTexture; //if source = SourceType.Texture

        public Texture alphaMask;

        public AnimationCurve slideCurve;

        public Anchor anchor = Anchor.Left;

        private Vector4 mParam;

        protected override void OnPrepare() {
            SetSourceTexture(source, sourceTexture);

            material.SetTexture("_AlphaMaskTex", alphaMask);

            //Vector2 anchorPt = GetAnchorPoint(anchor);
            //mParam.x = anchorPt.x;
            //mParam.y = anchorPt.y;
        }

        protected override void OnUpdate() {
            material.SetFloat("_t", curCurveValue);

            Vector2 scroll = GetUVScroll(anchor, slideCurve.Evaluate(curT));

            material.SetVector("_Scroll", scroll);
        }
    }
}
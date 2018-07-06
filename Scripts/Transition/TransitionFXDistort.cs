using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/TransitionFX/Type - Distort")]
    public class TransitionFXDistort : TransitionFX {
        public SourceType source = SourceType.CameraSnapShot;
        public Texture sourceTexture; //if source = SourceType.Texture

        public Texture distortTexture;
        public float distortTime = 0.35f; //[0, 2]
        public AnimationCurve distortMag;
        public bool distortMagNormalized;

        public Vector2 force = new Vector2(0.2f, 0.2f); //[0, 2]

        private Vector4 mParam;

        protected override void OnPrepare() {
            SetSourceTexture(source, sourceTexture);

            material.SetTexture("_DistortTex", distortTexture);

            mParam.x = force.x;
            mParam.y = force.y;
            mParam.z = distortTime;
            material.SetVector("_Params", mParam);
        }

        protected override void OnUpdate() {
            material.SetFloat("_t", curCurveValue);

            material.SetFloat("_distortT", distortMag.Evaluate(distortMagNormalized ? curTimeNormalized : curTime));
        }
    }
}
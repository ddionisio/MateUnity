using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/TransitionFX/Type - Dissolve")]
    public class TransitionFXDissolve : TransitionFX {
        public SourceType source = SourceType.CameraSnapShot;
        public Texture sourceTexture; //if source = SourceType.Texture

        public Texture dissolveTexture;
        public AnimationCurve dissolvePower;
        public bool dissolvePowerNormalized;

        public Texture emissionTexture;
        public float emissionThickness = 0.03f; //[0, 0.1 max?]

        private Vector4 mParam;

        protected override void OnPrepare() {
            SetSourceTexture(source, sourceTexture);

            material.SetTexture("_DissolveTex", dissolveTexture);

            material.SetTexture("_EmissionTex", emissionTexture);

            mParam.y = emissionThickness;
        }

        protected override void OnUpdate() {
            material.SetFloat("_t", curCurveValue);

            mParam.x = dissolvePower.Evaluate(dissolvePowerNormalized ? curTimeNormalized : curTime);
            material.SetVector("_Params", mParam);
        }
    }
}
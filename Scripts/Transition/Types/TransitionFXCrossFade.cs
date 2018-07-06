using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Transition from source to destination
    /// </summary>
    [CreateAssetMenu(fileName = "crossFade", menuName = "M8/TransitionFX/Cross Fade")]
    public class TransitionFXCrossFade : TransitionFX {
        public SourceType source = SourceType.CameraSnapShot;
        public Texture sourceTexture; //if source = SourceType.Texture

        protected override void OnPrepare() {
            base.OnPrepare();

            SetSourceTexture(source, sourceTexture);
        }

        protected override void OnUpdate() {
            material.SetFloat("_t", curCurveValue);
        }
    }
}
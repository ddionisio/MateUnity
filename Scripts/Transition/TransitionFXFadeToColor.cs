using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Transition from source to solid color
    /// </summary>
    [AddComponentMenu("M8/TransitionFX/Type - Fade To Color")]
    public class TransitionFXFadeToColor : TransitionFX {
        public Color color = Color.black;

        protected override void OnPrepare() {
            base.OnPrepare();

            material.SetColor("_Color", color);
        }

        protected override void OnUpdate() {
            material.SetFloat("_t", curCurveValue);
        }
    }
}
using UnityEngine;
using System.Collections;

namespace M8 {
    /// <summary>
    /// Transition from source to solid color
    /// </summary>
    [AddComponentMenu("M8/Screen Transition/Type - Fade To Color")]
    public class STFadeToColor : ScreenTrans {
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
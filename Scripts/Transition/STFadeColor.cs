using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Screen Transition/Type - Fade Color")]
public class STFadeColor : ScreenTrans {
    public Color color = Color.black;

    protected override void OnPrepare() {
        base.OnPrepare();

        material.SetColor("_Color", color);
    }

    protected override void OnUpdate() {
        material.SetFloat("_t", curCurveValue);
    }
}

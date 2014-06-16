using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Screen Transition/Type - Scale Fade")]
public class STScaleFade : ScreenTrans {
    public SourceType source = SourceType.CameraSnapShot;
    public Texture sourceTexture; //if source = SourceType.Texture

    public Texture alphaMask;

    public AnimationCurve scaleCurveX;
    public AnimationCurve scaleCurveY;

    public Anchor anchor = Anchor.Center;

    private Vector4 mParam;

    protected override void OnPrepare() {
        SetSourceTexture(source, sourceTexture);

        material.SetTexture("_AlphaMaskTex", alphaMask);

        Vector2 anchorPt = GetAnchorPoint(anchor);
        mParam.x = anchorPt.x;
        mParam.y = anchorPt.y;
    }

    protected override void OnUpdate() {
        material.SetFloat("_t", curCurveValue);

        mParam.z = scaleCurveX.Evaluate(curTime);
        mParam.w = scaleCurveY.Evaluate(curTime);

        material.SetVector("_Params", mParam);
    }
}

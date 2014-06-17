using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Screen Transition/Type - Distort")]
public class STDistort : ScreenTrans {
    public SourceType source = SourceType.CameraSnapShot;
    public Texture sourceTexture; //if source = SourceType.Texture

    public Texture distortTexture;
    public float distortTime; //[0, 2]

    public Vector2 force;

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
    }
}

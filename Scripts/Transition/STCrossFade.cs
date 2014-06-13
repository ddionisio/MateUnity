using UnityEngine;
using System.Collections;

/// <summary>
/// Transition from source to destination
/// </summary>
[AddComponentMenu("M8/Screen Transition/Type - Cross Fade")]
public class STCrossFade : ScreenTrans {
    public enum SourceType {
        CameraSnapShot,
        Texture
    }

    public SourceType source = SourceType.CameraSnapShot;
    public Texture sourceTexture; //if source = SourceType.Texture

    protected override void OnPrepare() {
        base.OnPrepare();

        switch(source) {
            case SourceType.CameraSnapShot:
                switch(cameraType) {
                    case CameraType.Main:
                        material.SetTexture("_SourceTex", ScreenTransManager.instance.CameraSnapshot(Camera.main));
                        break;
                    case CameraType.Target:
                        material.SetTexture("_SourceTex", ScreenTransManager.instance.CameraSnapshot(cameraTarget ? cameraTarget : Camera.main));
                        break;
                    case CameraType.All:
                        material.SetTexture("_SourceTex", ScreenTransManager.instance.CameraSnapshot(M8.Util.GetAllCameraDepthSorted()));
                        break;
                }
                break;

            case SourceType.Texture:
                material.SetTexture("_SourceTex", sourceTexture);
                break;
        }
    }

    protected override void OnUpdate() {
        material.SetFloat("_t", curCurveValue);
    }
}

using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Camera/ForceOrthoSort")]
[ExecuteInEditMode]
public class CameraForceOrthoSort : MonoBehaviour {

    void OnEnable() {
        camera.transparencySortMode = TransparencySortMode.Orthographic;
    }

    void OnPreCull() {
        camera.transparencySortMode = TransparencySortMode.Orthographic;
    }

#if UNITY_EDITOR
    void LateUpdate() {
        if(!Application.isPlaying) {
            camera.transparencySortMode = TransparencySortMode.Orthographic;
        }
    }
#endif
}

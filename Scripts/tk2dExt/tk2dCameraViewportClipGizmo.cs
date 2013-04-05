using UnityEngine;
using System.Collections;

//help to visualize the viewport clipping of a tk2dCamera
[AddComponentMenu("M8/tk2D/CameraViewportClipGizmo")]
public class tk2dCameraViewportClipGizmo : MonoBehaviour {
    public Color color = Color.white;

    void OnDrawGizmos() {
        tk2dCamera cam = GetComponent<tk2dCamera>();
        if(cam != null && cam.viewportClippingEnabled) {
            Gizmos.color = color;

            Vector3 center = transform.position;
            center.x += cam.viewportRegion.x + cam.viewportRegion.z * 0.5f;
            center.y += cam.viewportRegion.y + cam.viewportRegion.w * 0.5f;

            Gizmos.DrawWireCube(center, new Vector3(cam.viewportRegion.z, cam.viewportRegion.w, 1.0f));
        }
    }
}

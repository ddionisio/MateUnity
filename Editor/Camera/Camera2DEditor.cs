using UnityEngine;
using UnityEditor;
using System.Collections;

namespace M8 {
    [CustomEditor(typeof(Camera2D))]
    public class Camera2DEditor : Editor {
        public override void OnInspectorGUI() {
            Camera2D cam = target as Camera2D;
            Camera unityCam = cam.GetComponent<Camera>();

            //resolution
            GUILayout.BeginHorizontal();
            Vector2 res = new Vector2(cam.fixedResolutionWidth, cam.fixedResolutionHeight);
            res = EditorGUILayout.Vector2Field("Resolution", res);
            cam.fixedResolutionWidth = Mathf.RoundToInt(res.x);
            cam.fixedResolutionHeight = Mathf.RoundToInt(res.y);
            GUILayout.EndHorizontal();

            int camProjInd = unityCam.orthographic ? 0 : 1;
            camProjInd = EditorGUILayout.IntPopup("Projection", camProjInd, new string[] { "Orthographic", "Perspective" }, new int[] { 0, 1 });

            switch(camProjInd) {
                case 0:
                    unityCam.orthographic = true;

                    cam.usePixelPerMeter = EditorGUILayout.Toggle("Use Pixel Per Meter", cam.usePixelPerMeter);

                    if(cam.usePixelPerMeter) {
                        float ppm = EditorGUILayout.FloatField("Pixel/Meter", cam.pixelPerMeter);
                        cam.pixelPerMeter = Mathf.Max(0.001f, ppm);
                    }
                    else {
                        float o = EditorGUILayout.FloatField("Size", cam.orthographicSize);
                        cam.orthographicSize = Mathf.Max(0.001f, o);
                    }
                    break;

                case 1:
                    unityCam.orthographic = false;
                    cam.fov = EditorGUILayout.Slider("FOV", cam.fov, 1.0f, 179.0f);
                    cam.transparencySortMode = (TransparencySortMode)EditorGUILayout.EnumPopup("Sort Mode", cam.transparencySortMode);
                    break;
            }

            cam.viewRect = EditorGUILayout.RectField("Viewport Rect", cam.viewRect);

            if(unityCam.orthographic) {
                cam.scaleMode = (Camera2D.ScaleMode)EditorGUILayout.EnumPopup("Scale Mode", cam.scaleMode);

                if(cam.scaleMode == Camera2D.ScaleMode.None) {
                    float ps = EditorGUILayout.FloatField("Pixel Scale", cam.pixelScale);
                    cam.pixelScale = Mathf.Max(0.001f, ps);
                }

                cam.origin = (Camera2D.Origin)EditorGUILayout.EnumPopup("Origin", cam.origin);

                cam.usePixelOffset = EditorGUILayout.Toggle("Use Pixel Offset", cam.usePixelOffset);
                if(cam.usePixelOffset) {
                    cam.pixelOffset = EditorGUILayout.Vector2Field("Pixel Offset", cam.pixelOffset);
                }

                cam.useClipping = EditorGUILayout.Toggle("Use Clipping", cam.useClipping);
                if(cam.useClipping) {
                    cam.clipRegion = EditorGUILayout.RectField("Clip Region", cam.clipRegion);
                }
            }

            cam.zoom = EditorGUILayout.FloatField("Zoom", cam.zoom);
        }
    }
}
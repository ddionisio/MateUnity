using UnityEngine;
using UnityEditor;
using System.Collections;

public class SceneCamAxisRotator : EditorWindow {
    private float r = 0.0f;

    [MenuItem("M8/Tools/Camera Axis Rotator")]
    static void DoIt() {
        EditorWindow.GetWindow(typeof(SceneCamAxisRotator));
    }
    
    void OnGUI() {
        GUILayout.BeginHorizontal();

        float prevR = r;

        if(GUILayout.Button("R")) {
            r = 0.0f;
        }

        r = EditorGUILayout.Slider("Z-Axis", r, 0, 360);
        if(prevR != r) {
            SceneView.currentDrawingSceneView.rotation = Quaternion.AngleAxis(r, Vector3.forward);
            SceneView.currentDrawingSceneView.Repaint();
        }

        GUILayout.EndHorizontal();
    }
}

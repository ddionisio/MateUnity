using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class BoxCollider2DEditHelper {
        [MenuItem("CONTEXT/BoxCollider2D/Generate From Mesh")]
        private static void GenerateFromMesh() {
            var selected = Selection.activeObject as GameObject;
            if(!selected)
                return;

            var go = selected as GameObject;
            if(!go)
                return;

            var boxColl = go.GetComponent<BoxCollider2D>();
            if(!boxColl)
                return;

            var meshFilter = go.GetComponent<MeshFilter>();
            if(!meshFilter || !meshFilter.sharedMesh)
                return;
            
            var meshBounds = meshFilter.sharedMesh.bounds;
            
            boxColl.size = meshBounds.max - meshBounds.min;
            boxColl.offset = Vector2.Lerp(meshBounds.min, meshBounds.max, 0.5f);
        }
    }
}
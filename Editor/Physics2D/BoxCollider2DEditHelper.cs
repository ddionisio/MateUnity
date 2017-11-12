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
            if(!meshFilter)
                return;

            var vertices = meshFilter.sharedMesh.vertices;
            if(vertices == null || vertices.Length == 0)
                return;

            //get the min and max x/y of vertices
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for(int i = 0; i < vertices.Length; i++) {
                Vector2 vert = vertices[i];

                if(vert.x < min.x)
                    min.x = vert.x;
                if(vert.x > max.x)
                    max.x = vert.x;

                if(vert.y < min.y)
                    min.y = vert.y;
                if(vert.y > max.y)
                    max.y = vert.y;
            }

            boxColl.size = max - min;
            boxColl.offset = Vector2.Lerp(min, max, 0.5f);
        }
    }
}
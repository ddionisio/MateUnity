using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    public class PolygonCollider2DEditHelper {
        class Edge {
            public int vert1;
            public int vert2;

            public Edge(int Vert1, int Vert2) {
                vert1 = Vert1;
                vert2 = Vert2;
            }
        }

        [MenuItem("CONTEXT/PolygonCollider2D/Generate From Mesh")]
        private static void GenerateFromMesh() {
            var selected = Selection.activeObject as GameObject;
            if(!selected)
                return;

            var go = selected as GameObject;
            if(!go)
                return;

            var polygonColl = go.GetComponent<PolygonCollider2D>();
            if(!polygonColl)
                return;

            polygonColl.pathCount = 0;

            var meshFilter = go.GetComponent<MeshFilter>();
            if(!meshFilter)
                return;

            Reconstruct(polygonColl, meshFilter);
        }

        public static void Reconstruct(PolygonCollider2D polygonColl, MeshFilter meshFilter) {
            var vertices = meshFilter.sharedMesh.vertices;

            //grab edges
            var edges = new List<Edge>();
            var triangles = meshFilter.sharedMesh.triangles;
            for(int i = 0; i < triangles.Length; i += 3) {
                edges.Add(new Edge(triangles[i], triangles[i + 1]));
                edges.Add(new Edge(triangles[i + 1], triangles[i + 2]));
                edges.Add(new Edge(triangles[i + 2], triangles[i]));
            }

            //remove duplicates
            var edgesToRemove = new List<Edge>();
            foreach(Edge edge1 in edges) {
                foreach(Edge edge2 in edges) {
                    if(edge1 != edge2) {
                        if(edge1.vert1 == edge2.vert1 && edge1.vert2 == edge2.vert2 || edge1.vert1 == edge2.vert2 && edge1.vert2 == edge2.vert1) {
                            edgesToRemove.Add(edge1);
                        }
                    }
                }
            }

            foreach(Edge edge in edgesToRemove)
                edges.Remove(edge);

            int currentPathIndex = 0;
            var points = new List<Vector2>();

            EdgeTrace(edges[0], polygonColl, vertices, points, edges, ref currentPathIndex);
        }

        private static void EdgeTrace(Edge edge, PolygonCollider2D polygonCollider, Vector3[] vertices, List<Vector2> points, List<Edge> edges, ref int currentPathIndex) {
            // Add this edge's vert1 coords to the point list
            points.Add(vertices[edge.vert1]);

            // Store this edge's vert2
            int vert2 = edge.vert2;

            // Remove this edge
            edges.Remove(edge);

            // Find next edge that contains vert2
            foreach(Edge nextEdge in edges) {
                if(nextEdge.vert1 == vert2) {
                    EdgeTrace(nextEdge, polygonCollider, vertices, points, edges, ref currentPathIndex);
                    return;
                }
            }

            // No next edge found, create a path based on these points
            polygonCollider.pathCount = currentPathIndex + 1;
            polygonCollider.SetPath(currentPathIndex, points.ToArray());

            // Empty path
            points.Clear();

            // Increment path index
            currentPathIndex++;

            // Start next edge trace if there are edges left
            if(edges.Count > 0) {
                EdgeTrace(edges[0], polygonCollider, vertices, points, edges, ref currentPathIndex);
            }
        }
    }
}
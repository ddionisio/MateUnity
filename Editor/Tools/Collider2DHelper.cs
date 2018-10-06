using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace M8.EditorExt {
    public class Collider2DHelper {
        [MenuItem("M8/Collider2D/Reconstruct From Mesh")]
        public static void ReconstructFromMesh() {
            var gos = Selection.gameObjects;
            for(int i = 0; i < gos.Length; i++) {
                var go = gos[i];

                //ensure this is not a parent of any of the other gos
                bool isChild = false;
                for(int j = 0; j < gos.Length; j++) {
                    if(j != i && go.transform.IsChildOf(gos[j].transform)) {
                        isChild = true;
                        break;
                    }
                }

                if(isChild)
                    continue;

                //grab Collider2D's and update them based on their mesh
                Collider2D[] colls = go.GetComponentsInChildren<Collider2D>();
                Undo.RecordObjects(colls, "Game Map Root Reconstruct Colliders");

                for(int c = 0; c < colls.Length; c++) {
                    var meshFilter = colls[c].GetComponent<MeshFilter>();
                    if(!meshFilter)
                        continue;

                    if(colls[c] is PolygonCollider2D)
                        PolygonCollider2DEditHelper.Reconstruct((PolygonCollider2D)colls[c], meshFilter);
                    else if(colls[c] is BoxCollider2D)
                        BoxCollider2DEditHelper.Reconstruct((BoxCollider2D)colls[c], meshFilter);
                }
            }
        }
    }
}
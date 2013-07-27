using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityFieldUpDir")]
public class GravityFieldUpDir : GravityFieldBase {

    protected override Vector3 GetUpVector(Transform entity) {
        return transform.up;
    }
}

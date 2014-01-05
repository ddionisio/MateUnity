using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityFieldSpherical")]
public class GravityFieldSpherical : GravityFieldBase {

    public bool inward = false;

    public override Vector3 GetUpVector(GravityController entity) {
        Vector3 position = entity.transform.position;

        Vector3 dir = inward ? transform.position - position : position - transform.position;
        return dir.normalized;
    }
}

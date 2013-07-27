using UnityEngine;
using System.Collections;

[AddComponentMenu("M8/Physics/GravityFieldSurfaceNormal")]
public class GravityFieldSurfaceNormal : GravityFieldBase {
    public LayerMask checkLayer;
    public float checkDistance = 20.0f;
    public float checkEntityAngle = 45.0f; //this is the angle limit to check for surface with given entity, otherwise use dir towards center.
    public float checkSurfaceAngle = 45.0f; //this is the angle limit to allow a surface to be the gravity up vector.
    private Vector3 mCenter;

    protected override Vector3 GetUpVector(Transform entity) {
        Vector3 entPos = entity.position;
        Vector3 entUp = entity.up;

        Vector3 dir = entPos - mCenter;
        dir.Normalize();

        //check if we are within reasonable angle between entity's up and dir from center
        if(Vector3.Angle(dir, entUp) < checkEntityAngle) {
            //check downward and see if we collide
            RaycastHit hit;

            if(Physics.Raycast(entPos, entUp, out hit, checkDistance, checkLayer)) {
                if(Vector3.Angle(dir, hit.normal) < checkSurfaceAngle) {
                    return hit.normal;
                }
            }
        }

        return dir;
    }

    void Awake() {
        mCenter = Vector3.zero;

        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach(Collider col in cols) {
            mCenter += col.bounds.center;
        }

        mCenter /= cols.Length;
    }
}

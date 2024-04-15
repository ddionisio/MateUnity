using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    [AddComponentMenu("M8/Physics2D/ForceFieldSurface")]
    public class ForceFieldSurface2D : ForceFieldBase2D {
        public const int startCapacity = 8;

        public Transform center;

        public bool checkDirInverse = false;
        public bool checkUseEntityUp = false; //for when checking the surface

        public bool dirInverse = false;
        public float dirAngleChangeThreshold = 5f;

        public LayerMask checkLayer;
        public float checkRadius = 0.5f; //this is the sphere-cast radius when checking
        public float checkDistance = 20.0f;
        //public float checkEntityAngle = 45.0f; //this is the angle limit to check for surface with given entity, otherwise use dir towards center.
        public float checkSurfaceAngle = 45.0f; //this is the angle limit to allow a surface to be the gravity up vector.
        public float changeDirDelay = 0.2f;
        
        public override Vector2 GetDir(ForceController2D entity) {
            Vector2 center = this.center ? this.center.position : transform.position;

            Vector2 toDir;

            Vector2 entPos = entity.transform.position;
            Vector2 entUp = entity.transform.up;

            Vector2 dir = checkDirInverse ? entPos - center : center - entPos;
            dir.Normalize();

            //check if we are within reasonable angle between entity's up and dir from center
            //if(Vector3.Angle(dir, entUp) < checkEntityAngle) {
            //check downward and see if we collide

            Vector2 checkDir = checkUseEntityUp ? -entUp : -dir;

            RaycastHit2D hit;

            if(checkRadius > 0f)
                hit = Physics2D.CircleCast(entPos, checkRadius, checkDir, checkDistance, checkLayer);
            else
                hit = Physics2D.Raycast(entPos, checkDir, checkDistance, checkLayer);

            if(hit.collider) {
                if(Vector2.Angle(dir, hit.normal) < checkSurfaceAngle && Vector2.Angle(entUp, hit.normal) > dirAngleChangeThreshold) {
                    toDir = hit.normal;
                }
                else
                    toDir = entUp;
            }
            else
                toDir = entUp;
            //}

            //var toDir = info != null ? info.curDir : entUp;

            return dirInverse ? -toDir : toDir;
        }
        
    }
#endif
}
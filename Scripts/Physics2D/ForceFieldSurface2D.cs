using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
    [AddComponentMenu("M8/Physics2D/ForceFieldSurface")]
    public class ForceFieldSurface2D : ForceFieldBase2D {
        public const int startCapacity = 8;

        public Transform center;

        public bool inverse = false;
        public bool useEntityUp = false; //for when checking the surface

        public LayerMask checkLayer;
        public float checkRadius = 0.5f; //this is the sphere-cast radius when checking
        public float checkDistance = 20.0f;
        //public float checkEntityAngle = 45.0f; //this is the angle limit to check for surface with given entity, otherwise use dir towards center.
        public float checkSurfaceAngle = 45.0f; //this is the angle limit to allow a surface to be the gravity up vector.
        public float changeDirDelay = 0.2f;

        private class DirInfo {
            public Vector2 curDir;
            public Vector2 targetDir;
            public Vector2 curDirVel;

            public DirInfo(Vector2 dir, Vector2 target) {
                curDir = dir;
                targetDir = target;
            }
        }
        
        private Dictionary<ForceController2D, DirInfo> mCurDirs = new Dictionary<ForceController2D, DirInfo>(startCapacity);
        private CacheList<ForceController2D> mRemoveDirs = new CacheList<ForceController2D>(startCapacity);

        public override Vector2 GetDir(ForceController2D entity) {
            Vector2 center = this.center ? this.center.position : transform.position;

            DirInfo info = null;
            mCurDirs.TryGetValue(entity, out info);

            Vector2 entPos = entity.bodyCollider ? entity.bodyCollider.bounds.center : entity.transform.position;
            Vector2 entUp = entity.transform.up;

            Vector2 dir = inverse ? entPos - center : center - entPos;
            dir.Normalize();

            //check if we are within reasonable angle between entity's up and dir from center
            //if(Vector3.Angle(dir, entUp) < checkEntityAngle) {
            //check downward and see if we collide
            var hit = Physics2D.CircleCast(entPos, checkRadius, useEntityUp ? -entUp : -dir, checkDistance, checkLayer);
            if(hit) {
                if(Vector3.Angle(dir, hit.normal) < checkSurfaceAngle) {
                    if(info == null) {
                        if(hit.normal != entUp) //TODO: tolerance value?
                            mCurDirs.Add(entity, new DirInfo(entUp, hit.normal));
                    }
                    else {
                        info.targetDir = hit.normal;
                    }
                }
            }
            //}

            return info != null ? info.curDir : entUp;
        }

        public override void ItemRemoved(ForceController2D ctrl) {
            mCurDirs.Remove(ctrl);
        }

        protected override void OnDisable() {
            mCurDirs.Clear();
            mRemoveDirs.Clear();

            base.OnDisable();
        }

        void FixedUpdate() {
            if(mCurDirs.Count > 0) {
                foreach(var pair in mCurDirs) {
                    if(pair.Key == null || !pair.Key.gameObject.activeInHierarchy) {
                        mRemoveDirs.Add(pair.Key);
                        continue;
                    }

                    DirInfo dat = pair.Value;

                    dat.curDir = Vector2.SmoothDamp(dat.curDir, dat.targetDir, ref dat.curDirVel, changeDirDelay, Mathf.Infinity, Time.fixedDeltaTime);

                    if(dat.curDir == dat.targetDir) {
                        mRemoveDirs.Add(pair.Key);
                    }
                }

                for(int i = 0; i < mRemoveDirs.Count; i++)
                    mCurDirs.Remove(mRemoveDirs[i]);

                mRemoveDirs.Clear();
            }
        }
    }
}
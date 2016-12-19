using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Physics/GravityFieldSurfaceNormal")]
    public class GravityFieldSurfaceNormal : GravityFieldBase {
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
            public Vector3 curDir;
            public Vector3 targetDir;
            public Vector3 curDirVel;

            public DirInfo(Vector3 dir, Vector3 target) {
                curDir = dir;
                targetDir = target;
            }
        }

        private Vector3 mCenter;

        private bool mUpdateActive = false;

        private Dictionary<GravityController, DirInfo> mCurDirs = new Dictionary<GravityController, DirInfo>(startCapacity);
        private List<GravityController> mRemoveDirs = new List<GravityController>(startCapacity);

        public override Vector3 GetUpVector(GravityController entity) {
            DirInfo info = null;
            mCurDirs.TryGetValue(entity, out info);

            Vector3 entPos = entity.coll ? entity.coll.bounds.center : entity.transform.position;
            Vector3 entUp = entity.up;

            Vector3 dir = inverse ? entPos - mCenter : mCenter - entPos;
            dir.Normalize();

            //check if we are within reasonable angle between entity's up and dir from center
            //if(Vector3.Angle(dir, entUp) < checkEntityAngle) {
            //check downward and see if we collide
            RaycastHit hit;

            if(Physics.SphereCast(entPos, checkRadius, useEntityUp ? -entUp : -dir, out hit, checkDistance, checkLayer)) {
                if(Vector3.Angle(dir, hit.normal) < checkSurfaceAngle) {
                    if(info == null) {
                        if(hit.normal != entUp) //TODO: tolerance value?
                            mCurDirs.Add(entity, new DirInfo(entUp, hit.normal));
                    }
                    else {
                        info.targetDir = hit.normal;
                    }

                    if(mCurDirs.Count > 0 && !mUpdateActive)
                        StartCoroutine(DoUpdate());
                }
            }
            //}

            return info != null ? info.curDir : entUp;
        }

        protected override void OnDisable() {
            mCurDirs.Clear();
            mRemoveDirs.Clear();
            mUpdateActive = false;

            base.OnDisable();
        }

        public override void ItemRemoved(GravityController ctrl) {
            mCurDirs.Remove(ctrl);
        }

        void Awake() {
            mCenter = center ? center.position : transform.position;
        }

        IEnumerator DoUpdate() {
            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            mUpdateActive = true;

            mRemoveDirs.Clear();

            while(mCurDirs.Count > 0) {
                foreach(KeyValuePair<GravityController, DirInfo> pair in mCurDirs) {
                    if(pair.Key == null || !pair.Key.gameObject.activeInHierarchy) {
                        mRemoveDirs.Add(pair.Key);
                        continue;
                    }

                    DirInfo dat = pair.Value;

                    dat.curDir = Vector3.SmoothDamp(dat.curDir, dat.targetDir, ref dat.curDirVel, changeDirDelay, Mathf.Infinity, Time.fixedDeltaTime);

                    if(dat.curDir == dat.targetDir) {
                        mRemoveDirs.Add(pair.Key);
                    }
                }

                foreach(GravityController t in mRemoveDirs)
                    mCurDirs.Remove(t);

                mRemoveDirs.Clear();

                yield return wait;
            }

            mUpdateActive = false;
        }
    }
}
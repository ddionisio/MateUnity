using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
#if !M8_PHYSICS2D_DISABLED
    /// <summary>
    /// Default is essentially gravity.
    /// </summary>
    [AddComponentMenu("M8/Physics2D/ForceController")]
    public class ForceController2D : MonoBehaviour {
        //Orient towards force dir
        public enum OrientMode {
            None,
            Up,
            Down,
            Left,
            Right
        }

        public Rigidbody2D body;

        public float defaultForce = 0f;
        public float defaultForceOrient = 0f; //relative to Vector2.up
        public float forceScale = 1f;

        public OrientMode orient = OrientMode.None;
        //public float orientAngleSpeed = 180.0f;
        public float orientDelay = 0.3f;

        public bool ignoreFields = false;

        [TagSelector]
        public string[] fieldTagFilter; //if not empty, which tags to consider for fields
        
        public bool forceLocked { get; set; }

        public Vector2 force { get; private set; }

        public Vector2 orientDir { get { return mOrientDestDir; } }

        private const int forceFieldCapacity = 4;
        private CacheList<ForceFieldBase2D> mForceFields = new CacheList<ForceFieldBase2D>(forceFieldCapacity);

        private Vector2 mOrientDestDir;
        private float mOrientAngleToUp;
        private float mOrientChangeVel;

        private Vector2 mDefaultForce;
        private Vector2 mDefaultForceDir;

        protected virtual void OnTriggerEnter2D(Collider2D col) {
            if(mForceFields.IsFull)
                return;

            if(fieldTagFilter.Length > 0) {
                bool isTagFound = false;
                for(int i = 0; i < fieldTagFilter.Length; i++) {
                    if(col.CompareTag(fieldTagFilter[i])) {
                        isTagFound = true;
                        break;
                    }
                }

                if(!isTagFound)
                    return;
            }

            var forceField = col.GetComponent<ForceFieldBase2D>();
            if(!forceField)
                return;

            if(!mForceFields.Exists(forceField))
                mForceFields.Add(forceField);
        }

        protected virtual void OnTriggerExit2D(Collider2D col) {
            if(mForceFields.Count == 0)
                return;

            if(fieldTagFilter.Length > 0) {
                bool isTagFound = false;
                for(int i = 0; i < fieldTagFilter.Length; i++) {
                    if(col.CompareTag(fieldTagFilter[i])) {
                        isTagFound = true;
                        break;
                    }
                }

                if(!isTagFound)
                    return;
            }

            var forceField = col.GetComponent<ForceFieldBase2D>();
            if(!forceField)
                return;

            if(mForceFields.Remove(forceField))
                forceField.ItemRemoved(this);
        }

        protected virtual void OnEnable() {
            mOrientDestDir = GetOrientDir();
            mOrientAngleToUp = Vector2.SignedAngle(mOrientDestDir, Vector2.up);
            mOrientChangeVel = 0f;
        }

        protected virtual void OnDisable() {
            ApplyOrientDir(mOrientDestDir);

            mForceFields.Clear();
        }

        protected virtual void Awake() {
            if(!body)
                body = GetComponent<Rigidbody2D>();

            if(body) {
                body.gravityScale = 0f;
            }

            mDefaultForceDir = MathUtil.RotateAngle(Vector2.up, defaultForceOrient);
            mDefaultForce = defaultForce * mDefaultForceDir;
        }

        protected virtual void FixedUpdate() {
#if UNITY_EDITOR
            mDefaultForceDir = MathUtil.RotateAngle(Vector2.up, defaultForceOrient);
            mDefaultForce = defaultForce * mDefaultForceDir;
#endif

            //apply fields
            if(!ignoreFields) {
                var newOrient = Vector2.zero;
                var newForce = mDefaultForce;
                int fieldCount = 0;

                //global
                var forceField = ForceFieldBase2D.global;
                if(forceField && forceField.gameObject.activeSelf && forceField.enabled) {
                    var dir = forceField.GetDir(this);

                    newOrient += dir;
                    newForce += forceField.GetForce(this) * dir;

                    fieldCount++;
                }

                //field contacts
                for(int i = mForceFields.Count - 1; i >= 0; i--) {
                    forceField = mForceFields[i];
                    if(forceField && forceField.gameObject.activeSelf && forceField.enabled) {
                        var dir = forceField.GetDir(this);

                        newOrient += dir;
                        newForce += forceField.GetForce(this) * dir;

                        fieldCount++;
                    }
                    else //not active, remove it
                        mForceFields.RemoveAt(i);
                }

                if(fieldCount > 0) {
                    if(fieldCount > 1)
                        newOrient.Normalize();

                    mOrientDestDir = newOrient;
                    mOrientAngleToUp = Vector2.SignedAngle(mOrientDestDir, Vector2.up);
                    force = newForce;
                }
            }
            else {
                mOrientDestDir = mDefaultForceDir;
                mOrientAngleToUp = defaultForceOrient;
                force = mDefaultForce;
            }

            //apply force
            if(!forceLocked)
                body.AddForce(force * forceScale, ForceMode2D.Force);

            //apply orient
            if(orient != OrientMode.None) {
                var curOrient = GetOrientDir();
                if(curOrient != mOrientDestDir) {
                    if(orientDelay > 0f) {
                        float curAngleToUp = Vector2.SignedAngle(curOrient, Vector2.up);

                        float angleToUp = Mathf.SmoothDampAngle(curAngleToUp, mOrientAngleToUp, ref mOrientChangeVel, orientDelay, float.MaxValue, Time.fixedDeltaTime);

                        ApplyOrientDir(MathUtil.RotateAngle(Vector2.up, angleToUp));
                    }
                    else
                        ApplyOrientDir(mOrientDestDir);

                    /*float angle = Vector2.SignedAngle(curOrient, mOrientDestDir);
                    float deltaAngle = Time.fixedDeltaTime * orientAngleSpeed;

                    if(angle > 0f) {
                        if(angle - deltaAngle > 0f)
                            ApplyOrientDir(MathUtil.RotateAngle(curOrient, -deltaAngle));
                        else
                            ApplyOrientDir(mOrientDestDir);
                    }
                    else {
                        if(angle + deltaAngle < 0f)
                            ApplyOrientDir(MathUtil.RotateAngle(curOrient, deltaAngle));
                        else
                            ApplyOrientDir(mOrientDestDir);
                    }*/
                }
            }
        }

        private Vector2 GetOrientDir() {
            var up = transform.up;

            switch(orient) {
                case OrientMode.Down:
                    return -up;
                case OrientMode.Right:
                    return new Vector2(up.y, -up.x);
                case OrientMode.Left:
                    return new Vector2(-up.y, up.x);
                default:
                    return up;
            }
        }

        private void ApplyOrientDir(Vector2 dir) {
            Vector2 toDir;

            switch(orient) {
                case OrientMode.Down:
                    toDir = -dir;
                    break;
                case OrientMode.Right:
                    toDir = new Vector2(-dir.y, dir.x);
                    break;
                case OrientMode.Left:
                    toDir = new Vector2(dir.y, -dir.x);
                    break;
                default:
                    toDir = dir;
                    break;
            }

            transform.eulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.up, toDir));
        }

        void OnDrawGizmos() {
            if(defaultForce > 0f) {
                Gizmos.color = Color.yellow;

                Vector2 pos = transform.position;

                Gizmo.ArrowLine2D(pos, pos + MathUtil.RotateAngle(Vector2.up, defaultForceOrient) * 0.5f);
            }
        }
    }
#endif
}
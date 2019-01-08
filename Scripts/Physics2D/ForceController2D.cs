using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace M8 {
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

        public Vector2 defaultForce = Vector2.zero;
        public float forceScale = 1f;

        public OrientMode orient = OrientMode.None;
        public float orientAngleSpeed = 180.0f;

        public bool ignoreFields = false;

        [TagSelector]
        public string[] fieldTagFilter; //if not empty, which tags to consider for fields
        
        public bool forceLocked { get; set; }

        public Vector2 force { get; private set; }

        private const int forceFieldCapacity = 4;
        private CacheList<ForceFieldBase2D> mForceFields = new CacheList<ForceFieldBase2D>(forceFieldCapacity);

        private Vector2 mOrientDestDir;

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
        }

        protected virtual void FixedUpdate() {
            //apply fields
            if(!ignoreFields) {
                var newOrient = Vector2.zero;
                var newForce = defaultForce;
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
                    force = newForce;
                }
            }

            //apply force
            if(!forceLocked)
                body.AddForce(force * forceScale, ForceMode2D.Force);

            //apply orient
            var curOrient = GetOrientDir();
            if(curOrient != mOrientDestDir) {
                float angle = Vector2.SignedAngle(curOrient, mOrientDestDir);
                float deltaAngle = Time.fixedDeltaTime * orientAngleSpeed;

                if(angle > 0f) {
                    if(angle - deltaAngle > 0f)
                        ApplyOrientDir(MathUtil.RotateAngle(curOrient, -deltaAngle));
                    else
                        ApplyOrientDir(mOrientDestDir);
                }
                else {
                    if(angle + deltaAngle > 0f)
                        ApplyOrientDir(MathUtil.RotateAngle(curOrient, deltaAngle));
                    else
                        ApplyOrientDir(mOrientDestDir);
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
            switch(orient) {
                case OrientMode.Down:
                    transform.up = -dir;
                    break;
                case OrientMode.Right:
                    transform.up = new Vector2(-dir.y, dir.x);
                    break;
                case OrientMode.Left:
                    transform.up = new Vector2(dir.y, -dir.x);
                    break;
                default:
                    transform.up = dir;
                    break;
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Physics2D/ForceField")]
    public class ForceField2D : MonoBehaviour {
        public enum Axis {
            Up,
            Right
        }

        public enum Mode {
            Dir,
            Center,
        }

        [SerializeField]
        Mode _mode = Mode.Dir;
        [SerializeField]
        Axis _dir = Axis.Up;
        [SerializeField]
        Vector2 _forceOfs = Vector2.zero;
        [SerializeField]
        float _maxSpeed = 15.0f;
        public bool inverse;
        public float force;
        public float impulse;
        public bool setDrag = false;
        public float drag = 0.0f;
        [SerializeField]
        float _updateDelay = 0.2f;
        [SerializeField]
        string[] _tagFilters = new string[0];
        private bool mModeRunning = false;
        private YieldInstruction mWait;
        private Vector2 mCenterLocal;
        private HashSet<Rigidbody2D> mBodies = new HashSet<Rigidbody2D>();

        void OnTriggerEnter2D(Collider2D t) {
            if(_tagFilters.Length > 0) {
                bool tagMatched = false;
                for(int i = 0; i < _tagFilters.Length; i++) {
                    if(t.CompareTag(_tagFilters[i])) {
                        tagMatched = true;
                        break;
                    }
                }

                if(!tagMatched)
                    return;
            }

            Rigidbody2D body = t.attachedRigidbody;
            if(body != null && !mBodies.Contains(body)) {
                if(impulse != 0.0f) {
                    Vector2 dir = Vector2.zero;
                    switch(_mode) {
                        case Mode.Dir:
                            switch(_dir) {
                                case Axis.Right:
                                    dir = Vector2.right;
                                    break;
                                default:
                                    dir = Vector2.up;
                                    break;
                            }
                            break;

                        case Mode.Center:
                            Vector2 pos = transform.localToWorldMatrix.MultiplyPoint(mCenterLocal);
                            dir = inverse ? pos - body.position : body.position - pos;
                            dir.Normalize();
                            break;
                    }

                    body.AddForce(dir * impulse, ForceMode2D.Impulse);
                }

                mBodies.Add(body);

                StartRoutine();
            }
        }

        void OnTriggerExit2D(Collider2D t) {
            Rigidbody2D body = t.attachedRigidbody;
            if(body != null) {
                mBodies.Remove(body);
            }
        }

        void OnDisable() {
            mBodies.Clear();
            mModeRunning = false;
            StopAllCoroutines();
        }

        void Awake() {
            if(_updateDelay > 0.0f)
                mWait = new WaitForSeconds(_updateDelay);
            else
                mWait = new WaitForFixedUpdate();

            //compute center
            mCenterLocal = Vector2.zero;

            Collider2D[] cols = GetComponentsInChildren<Collider2D>();
            if(cols != null && cols.Length > 0) {
                foreach(Collider2D col in cols) {
                    mCenterLocal += (Vector2)col.bounds.center;
                }

                mCenterLocal /= cols.Length;

                mCenterLocal = transform.worldToLocalMatrix.MultiplyPoint(mCenterLocal);
            }
        }

        void StartRoutine() {
            if(!mModeRunning) {
                switch(_mode) {
                    case Mode.Center:
                        StartCoroutine(DoModeCenter());
                        break;

                    case Mode.Dir:
                        StartCoroutine(DoModeDir());
                        break;
                }
            }
        }

        IEnumerator DoModeDir() {
            mModeRunning = true;

            while(mBodies.Count > 0) {
                Vector2 dir;
                switch(_dir) {
                    case Axis.Right:
                        dir = Vector2.right;
                        break;
                    default:
                        dir = Vector2.up;
                        break;
                }

                if(inverse)
                    dir *= -1;

                dir += _forceOfs;

                dir = transform.rotation * dir;

                foreach(Rigidbody2D body in mBodies) {
                    if(setDrag)
                        body.drag = drag;

                    Vector2 vel = body.velocity;
                    if(_maxSpeed <= 0f || vel.sqrMagnitude < _maxSpeed * _maxSpeed || Vector2.Angle(dir, vel) >= 90.0f) {
                        body.AddForce(dir * force, ForceMode2D.Force);
                    }
                }

                yield return mWait;
            }

            mModeRunning = false;
        }

        IEnumerator DoModeCenter() {
            mModeRunning = true;

            while(mBodies.Count > 0) {
                Vector2 pos = transform.localToWorldMatrix.MultiplyPoint(mCenterLocal);

                foreach(Rigidbody2D body in mBodies) {
                    if(setDrag)
                        body.drag = drag;

                    Vector2 vel = body.velocity;
                    if(_maxSpeed <= 0f || vel.sqrMagnitude < _maxSpeed * _maxSpeed) {
                        Vector2 dir = inverse ? pos - body.position : body.position - pos;
                        dir.Normalize();

                        body.AddForce(dir * force, ForceMode2D.Force);
                    }
                }

                yield return mWait;
            }

            mModeRunning = false;
        }
    }
}
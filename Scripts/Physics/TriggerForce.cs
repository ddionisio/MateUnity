using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace M8 {
    [AddComponentMenu("M8/Physics/TriggerForce")]
    public class TriggerForce : MonoBehaviour {
        public enum Axis {
            Up,
            Forward,
            Right
        }

        public enum Mode {
            Dir,
            Center,
            Velocity,
            Reflect
        }

        [SerializeField]
        Transform _dirRef;

        [SerializeField]
        Mode _dirMode = Mode.Dir;

        [SerializeField]
        Axis _dir;

        [SerializeField]
        string[] _tags;

        public bool inverse;
        public bool resetVelocityByAxis;

        public float force = 30.0f;
        public ForceMode mode = ForceMode.Impulse;

        public float nudgeOfs;

        public float forceLingerDelay;
        public float forceLinger;

        public bool lingerDragOverride = true;
        public float lingerDrag = 0.0f;
        public bool lingerUpdateDir = true;

        private Collider mColl;

        private HashSet<Collider> mColliders = new HashSet<Collider>();

        void OnDisable() {
            mColliders.Clear();
            StopAllCoroutines();
        }

        void Awake() {
            mColl = GetComponent<Collider>();
        }

        Vector3 GetWorldDir() {
            Vector3 dir;

            switch(_dir) {
                case Axis.Right:
                    dir = Vector3.right;
                    break;
                case Axis.Forward:
                    dir = Vector3.forward;
                    break;
                default:
                    dir = Vector3.up;
                    break;
            }

            return (_dirRef ? _dirRef : transform).rotation * (inverse ? -dir : dir);
        }

        bool CheckTag(string aTag) {
            for(int i = 0, max = _tags.Length; i < max; i++) {
                if(_tags[i] == aTag)
                    return true;
            }

            return false;
        }

        void OnTriggerEnter(Collider col) {

            if(!mColliders.Contains(col)) {
                Rigidbody body = col.attachedRigidbody;

                if(body != null && !body.isKinematic && (_tags.Length == 0 || CheckTag(col.gameObject.tag))) {
                    //check tags
                    Vector3 bodyPos = col.bounds.center;

                    Vector3 dir;

                    switch(_dirMode) {
                        case Mode.Center:
                            dir = inverse ? transform.position - bodyPos : bodyPos - transform.position;
                            dir.Normalize();

                            if(resetVelocityByAxis)
                                body.velocity = Vector3.zero;
                            break;

                        case Mode.Dir:
                            switch(_dir) {
                                case Axis.Right:
                                    dir = Vector3.right;

                                    if(resetVelocityByAxis) {
                                        Vector3 localVel = body.transform.worldToLocalMatrix.MultiplyVector(body.velocity);
                                        localVel.x = 0.0f;
                                        body.velocity = body.transform.localToWorldMatrix.MultiplyVector(localVel);
                                    }
                                    break;
                                case Axis.Forward:
                                    dir = Vector3.forward;

                                    if(resetVelocityByAxis) {
                                        Vector3 localVel = body.transform.worldToLocalMatrix.MultiplyVector(body.velocity);
                                        localVel.z = 0.0f;
                                        body.velocity = body.transform.localToWorldMatrix.MultiplyVector(localVel);
                                    }
                                    break;
                                default:
                                    dir = Vector3.up;

                                    if(resetVelocityByAxis) {
                                        Vector3 localVel = body.transform.worldToLocalMatrix.MultiplyVector(body.velocity);
                                        localVel.y = 0.0f;
                                        body.velocity = body.transform.localToWorldMatrix.MultiplyVector(localVel);
                                    }
                                    break;
                            }

                            dir = (_dirRef ? _dirRef : transform).rotation * (inverse ? -dir : dir);
                            break;

                        case Mode.Velocity:
                            dir = inverse ? -body.velocity.normalized : body.velocity.normalized;

                            if(resetVelocityByAxis)
                                body.velocity = Vector3.zero;
                            break;

                        case Mode.Reflect:
                            dir = Vector3.Reflect(body.velocity, GetWorldDir());
                            dir.Normalize();

                            if(resetVelocityByAxis)
                                body.velocity = Vector3.zero;
                            break;

                        default:
                            dir = Vector3.zero;
                            break;
                    }

                    if(mColl.enabled && col.enabled && col.gameObject.activeSelf) {
                        StartCoroutine(DoForceLinger(col, dir));
                    }
                }
            }
        }

        IEnumerator DoForceLinger(Collider col, Vector3 dir) {
            mColliders.Add(col);

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            float t = 0;

            Rigidbody body = col.attachedRigidbody;

            RigidBodyController ctrl = col.GetComponent<RigidBodyController>();
            if(ctrl) { ctrl.enabled = false; }

            if(nudgeOfs != 0.0f)
                col.transform.position = col.transform.position + dir*nudgeOfs;

            if(lingerDragOverride)
                body.drag = lingerDrag;

            yield return wait;

            if(force != 0.0f)
                body.AddForce(dir * force, mode);


            while(t < forceLingerDelay) {
                yield return wait;

                if(col == null || !col.enabled || !col.gameObject.activeSelf)
                    break;

                if(lingerDragOverride)
                    body.drag = lingerDrag;

                if(lingerUpdateDir) {
                    Vector3 bodyPos = col.bounds.center;

                    switch(_dirMode) {
                        case Mode.Center:
                            dir = inverse ? transform.position - bodyPos : bodyPos - transform.position;
                            dir.Normalize();

                            //if(resetVelocityByAxis)
                            //body.velocity = Vector3.zero;
                            break;
                    }
                }

                body.AddForce(dir * forceLinger);

                t += Time.fixedDeltaTime;
            }

            if(ctrl) { ctrl.enabled = true; }

            mColliders.Remove(col);
            //Debug.Log("removed: " + col.gameObject.name);
        }
    }
}
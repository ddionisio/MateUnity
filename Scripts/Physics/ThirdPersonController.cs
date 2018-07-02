using UnityEngine;
using System.Collections;

namespace M8 {
    [AddComponentMenu("M8/Physics/Third-Person Controller")]
    public class ThirdPersonController : RigidBodyController {
        [SerializeField]
        Transform _eye; //optional, can be set at runtime for cutscene, etc.

        public float eyeDistance = 5;

        public float eyeMoveSpeed = 30;
        public float eyeLookAtSpeed = 15;
        public float eyeCollisionRadius = 0.3f;
        public LayerMask eyeCollisionMask;

        public float lookPitchSpeed = 8.0f;
        public bool lookPitchInvert = false;
        public float lookPitchMin = -60.0f; //angle
        public float lookPitchMax = 70.0f;

        public float lookYawSpeed = 8.0f;
        public bool lookYawInvert = false;

        public bool moveXCircular = true; //if true, X movement is relative to the circumference of the origin

        public InputAction moveInputX;
        public InputAction moveInputY;
        public InputAction lookInputPitch;
        public InputAction lookInputYaw;

        public bool startInputEnabled = false;

        private bool mInputEnabled = false;

        private float mCurLookPitch;

        private Vector3 mOrigin; //camera stand

        private Vector3 mEyePos;
        private Vector3 mEyeLookAtPos;

        private Coroutine mEyeFollowRoutine;

        public bool inputEnabled {
            get { return mInputEnabled; }
            set {
                mInputEnabled = value;
                if(mInputEnabled) {

                }
                else {

                }
            }
        }

        public Transform eye {
            get { return _eye; }
            set {
                _eye = value;

                InitEyeFollow();
            }
        }

        public Vector3 curMoveForward {
            get { return dirHolder.forward; }
        }

        public void ResetEye() {
            mCurLookPitch = 0f;

            mEyeLookAtPos = mOrigin = dirHolder.position - curMoveForward*eyeDistance;
        }

        protected override void OnDestroy() {
            inputEnabled = false;

            base.OnDestroy();
        }

        protected override void OnDisable() {
            if(mEyeFollowRoutine != null) {
                StopCoroutine(mEyeFollowRoutine);
                mEyeFollowRoutine = null;
            }

            base.OnDisable();
        }

        void Start() {
            inputEnabled = startInputEnabled;

            //initialize current eye origin
            ResetEye();

            InitEyeFollow();
        }

        protected override void FixedUpdate() {
            Vector3 outer = dirHolder.localPosition;
            Vector3 center = dirHolder.worldToLocalMatrix.MultiplyPoint3x4(mOrigin);
            Vector2 ndir = new Vector2(outer.x - center.x, outer.z - center.z);
            bool updateOrigin = false, updateEye = false;

            float lookYaw, lookPitch;

            //determine dir rotation

            if(mInputEnabled) {
                float moveX, moveY;

                moveX = moveInputX ? moveInputX.GetAxis() : 0.0f;
                moveY = moveInputY ? moveInputY.GetAxis() : 0.0f;

                if(!isSlopSlide) {
                    moveForward = moveY;
                    moveSide = moveX;
                }

                lookYaw = lookInputYaw ? lookInputYaw.GetAxis()*lookYawSpeed : 0.0f;
                if(lookYawInvert)
                    lookYaw *= -1;

                lookPitch = lookInputPitch ? lookInputPitch.GetAxis()*lookPitchSpeed : 0.0f;
                if(lookPitchInvert)
                    lookPitch *= -1;

                if(lookYaw != 0f) {
                    ndir = MathUtil.Rotate(ndir, lookYaw * Mathf.Deg2Rad);
                    updateOrigin = true;
                }

                if(lookPitch != 0f) {
                    mCurLookPitch = Mathf.Clamp(mCurLookPitch + lookPitch, lookPitchMin, lookPitchMax);
                    updateEye = true;
                }
            }
            else {
                moveForward = 0.0f;
                moveSide = 0.0f;

                lookYaw = 0.0f;
                lookPitch = 0.0f;
            }

            //grab current velocity
            ComputeLocalVelocity(false);

            //rotate dir based on eye origin
            if(moveXCircular || updateOrigin) {
                dirHolder.forward = dirHolder.localToWorldMatrix.MultiplyVector(new Vector3(ndir.x, 0f, ndir.y));

                //refresh current velocty based on rotation
                mBody.velocity = dirHolder.localToWorldMatrix.MultiplyVector(localVelocity);
            }

            base.FixedUpdate();

            Vector3 dirPos = dirHolder.position;
            Vector3 dirForward = dirHolder.forward;

            //update origin
            if(updateOrigin || ndir.sqrMagnitude != eyeDistance*eyeDistance) {
                mOrigin = dirPos - dirForward*eyeDistance;
                updateEye = true;
            }

            if(updateEye) {
                //apply pitch
                Vector3 dpos = mOrigin - dirPos;

                dpos = dirHolder.worldToLocalMatrix.MultiplyVector(dpos);
                dpos = Quaternion.AngleAxis(mCurLookPitch, Vector3.right)*dpos;

                mEyePos = dirPos + dirHolder.localToWorldMatrix.MultiplyVector(dpos);
            }

            Vector3 forwardCheck = mEyePos - mEyeLookAtPos;
            float forwardCheckDist = forwardCheck.magnitude;
            if(forwardCheckDist > 0.0f) {
                forwardCheck /= forwardCheckDist;
                RaycastHit hit;
                if(Physics.SphereCast(mEyeLookAtPos - forwardCheck*eyeCollisionRadius, eyeCollisionRadius, forwardCheck, out hit, forwardCheckDist + eyeCollisionRadius, eyeCollisionMask))
                    mEyePos = hit.point + hit.normal*eyeCollisionRadius;
            }
        }

        void OnDrawGizmosSelected() {
            Gizmos.color = new Color(0f, 0.7f, 0f, 0.3f);
            Gizmos.DrawSphere(mEyePos, 0.25f);
        }

        private void InitEyeFollow() {
            if(_eye) {
                if(mEyeFollowRoutine == null)
                    mEyeFollowRoutine = StartCoroutine(DoEyeFollow());
            }
        }

        IEnumerator DoEyeFollow() {
            //WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while(_eye) {
                float dt = Time.deltaTime;

                Vector3 pos = dirHolder.position;
                Vector3 eyePos = _eye.position;
                                
                //move
                eyePos = new Vector3(
                    Mathf.SmoothStep(eyePos.x, mEyePos.x, dt*eyeMoveSpeed),
                    Mathf.SmoothStep(eyePos.y, mEyePos.y, dt*eyeMoveSpeed),
                    Mathf.SmoothStep(eyePos.z, mEyePos.z, dt*eyeMoveSpeed)
                    );

                _eye.position = eyePos;

                //look
                mEyeLookAtPos.Set(
                    Mathf.SmoothStep(mEyeLookAtPos.x, pos.x, dt*eyeLookAtSpeed),
                    Mathf.SmoothStep(mEyeLookAtPos.y, pos.y, dt*eyeLookAtSpeed),
                    Mathf.SmoothStep(mEyeLookAtPos.z, pos.z, dt*eyeLookAtSpeed)
                    );

                _eye.forward = mEyeLookAtPos - eyePos;

                yield return null;
            }

            mEyeFollowRoutine = null;
        }
    }
}